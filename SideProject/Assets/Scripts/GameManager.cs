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
        DetectNpcConversation();
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
        usableObjects.Clear();
        foreach (var obj in FindObjectsOfType<UsableObjectController>())
        {
            usableObjects.Add(obj);
        }
    }

    public string GetActiveScene()
    {
        return currentScene;
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