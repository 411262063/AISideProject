using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData player;
    //public List<NpcData> allNpcData;
    public List<AgentController> activeAgents;
    public List<NpcController> activeNpcAgents;
    public List<UsableObjectController> usableObjects;
    private static string currentScene;

    private float elapsedTime;
    private const float realSecPerGameDay = 300f; //5 min per day in game
    private const float gameSecPerDay = 86400f; //24 hour
    private int currentDay;
    private int currentHour;
    private int currentMinute;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = gameSecPerDay / realSecPerGameDay; //加速時間86400/300 = 288倍
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        currentDay = player.day;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        UpdateActiveAgents();
        UpdateActiveUsableObjects();
        PlayerInterfaceUi.Instance?.UpdateAllUiElements();
    }
    public string GetActiveScene() => currentScene;

    private void Update()
    {
        UpdateGameTime();
        DetectNpcConversation();
    }

    private void UpdateGameTime()
    {
        elapsedTime += Time.deltaTime; //經過時間(deltaTime已是加速288倍後的時間)
        float totalGameSeconds = (elapsedTime / realSecPerGameDay) * gameSecPerDay;
        currentDay = player.day + Mathf.FloorToInt(totalGameSeconds / gameSecPerDay);
        currentHour = (int)(totalGameSeconds / 3600) % 24;
        currentMinute = (int)(totalGameSeconds / 60) % 60;
        PlayerInterfaceUi.Instance?.UpdateHUD();
        if (currentDay != player.day) DayEndSettlement();
    }

    public string GetGameTime()
    {
        return $"Day {currentDay}, {currentHour:D2}:{currentMinute:D2}";
    }

    private void DayEndSettlement()
    {
        player.day = currentDay;
        elapsedTime = 0f;
        PlayerInterfaceUi.Instance.UpdateHUD();
        SaveGlobalConvAndAgentsMemory();
    }

    private void SaveGlobalConvAndAgentsMemory()
    {
        ChatManager.Instance?.SaveGlobalConvRecordToFile();
        
        foreach (var agent in activeAgents)
        {
            agent.character.SaveAgentMemoryToFile();
        }
    }

    private void UpdateActiveAgents()
    {
        activeAgents.Clear();
        activeNpcAgents.Clear();
        activeAgents.Add(FindAnyObjectByType<PlayerController>());
        foreach (var npc in FindObjectsOfType<NpcController>())
        {
            if (npc.character != null)
            {
                activeNpcAgents.Add(npc);
                activeAgents.Add(npc);
            }
        }
    }

    private void UpdateActiveUsableObjects()
    {
        usableObjects.Clear();
        foreach (var obj in FindObjectsOfType<UsableObjectController>())
        {
            usableObjects.Add(obj);
        }
    }

    public void DetectNpcConversation()
    {
        for(int i = 0; i < activeNpcAgents.Count; i++)
        {
            for (int j = 0; j < activeNpcAgents.Count; j++)
            {
                if (activeNpcAgents[i] == activeNpcAgents[j]) continue; //avoid same members
                
                float distance = Vector3.Distance(activeNpcAgents[i].transform.position, activeNpcAgents[j].transform.position);
                if (distance < ChatManager.Instance.chatTriggerDistance)
                {
                    if (activeNpcAgents[i].CanStartNewChat() && activeNpcAgents[j].CanStartNewChat())
                    {
                        ChatManager.Instance.StartConversation(activeNpcAgents[i], activeNpcAgents[j]);
                    }
                    break;
                }
            }
        }
    }
}


//private void BuildNpcLastStayedInScene()
//{
//    foreach(NpcData npc in allNpcData)
//    {
//        if(npc.lastStayedScene == currentScene)
//        {
//            GameObject npcObject = Instantiate(npc.characterPrefab, npc.lastLocation, Quaternion.identity);
//            npcObject.name = npc.charNameEng;
//        }
//    }
//}