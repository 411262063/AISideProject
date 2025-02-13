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
    public float realMinutesPerDay = 5f; //5 min per day in game
    public float timeScale => 1440f / realMinutesPerDay;
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
        InitializeGameplayTime();
    }

    private void InitializeGameplayTime()
    {
        currentDay = player.day;
        currentHour = player.hour;
        currentMinute = player.minute;
        elapsedTime = currentHour * 60f + currentMinute;
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
        elapsedTime += Time.unscaledDeltaTime * (timeScale / 60f);
        currentDay = player.day + Mathf.FloorToInt(elapsedTime / 1440f);
        currentHour = (int)(elapsedTime / 60) % 24;
        currentMinute = (int)elapsedTime % 60;
        player.day = currentDay;
        player.hour = currentHour;
        player.minute = currentMinute;
        PlayerInterfaceUi.Instance?.UpdateHUD();
        if (elapsedTime >= 1440f) DayEndSettlement();
    }

    public string GetGameTime()
    {
        return $"Day {player.day}, {player.hour:D2}:{player.minute:D2}";
    }

    private void DayEndSettlement()
    {
        elapsedTime = 0f;
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