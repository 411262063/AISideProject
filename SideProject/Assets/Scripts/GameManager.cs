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
    public List<NpcData> allNpcData;
    public List<AgentController> activeAgents;
    public List<NpcController> activeNpcAgents;
    public List<UsableObjectController> activeUsableObjects;
    private static string currentScene;

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

    private void Update()
    {
        //DetectConversation();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        UpdateActiveAgents();
        UpdateActiveUsableObjects();
        PlayerInterfaceUi.Instance?.UpdateAllUiElements();
    }

    private void UpdateActiveAgents()
    {
        activeAgents.Clear();
        activeNpcAgents.Clear();
        activeAgents.Add(player.characterAgent.GetComponent<AgentController>());
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
        activeUsableObjects.Clear();
        foreach (var obj in FindObjectsOfType<UsableObjectController>())
        {
            activeUsableObjects.Add(obj);
        }
    }

    public string GetActiveScene()
    {
        return currentScene;
    }

    public void DetectConversation()
    {
        for(int i = 0; i < activeAgents.Count; i++)
        {
            for (int j = 0; j < activeAgents.Count; j++)
            {
                if (activeAgents[i] == activeAgents[j]) continue; //avoid same members
                
                float distance = Vector3.Distance(activeAgents[i].transform.position, activeAgents[j].transform.position);
                if(distance < ChatManager.Instance.chatTriggerDistance)
                {
                    ChatManager.Instance.StartConversation(activeAgents[i], activeAgents[j]);
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