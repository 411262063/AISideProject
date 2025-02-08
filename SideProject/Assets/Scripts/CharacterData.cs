using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterData : ScriptableObject
{
    [Header("全身立繪")]
    public Sprite bodyVisual;
    [Header("頭像")]
    public Sprite headVisual;
    [Header("中文名")]
    public string charNameChi;
    [Header("英文名")]
    public string charNameEng;
    [Header("年齡")]
    public int age;
    [Header("職業")]
    public string occupation;
    [Header("興趣")]
    public string interest;
    [Header("個性")]
    public string personality;
    [Header("聊天意願")]
    public int chatIntent;
    [Header("聊天冷卻時間")]
    public float chatCoolDown;
    [Header("使用物件時間")]
    public float objectUsageTime;
    [Header("描述"),TextArea(3, 20)]
    public string description;
    [Header("人際關係")]
    public List<CharacterData> relationship;
    [SerializeField, Header("記憶"), TextArea(5,20)]
    private string AgentMemory;
    [Header("")]
    public GameObject characterAgent;
    public Vector3 currentLocation;

    public void AddRelationship(CharacterData charToAdd)
    {
        if (relationship.Contains(charToAdd) || charToAdd == null) return;
        relationship.Add(charToAdd);
    }

    public void SaveNewAgentMemory(string label, string memoryContent)
    {
        string timeStamp = DateTime.Now.ToString("HH:mm");
        AgentMemory += $"[{label}]於{timeStamp} {memoryContent}\n";
    }

    public void SaveAgentMemoryToFile()
    {
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "_Meee/AISideProject/AgentsMemory");
        string fileName = $"{charNameEng}.txt";
        string filePath = Path.Combine(folderPath, fileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            File.AppendAllText(filePath, AgentMemory);
        }
        else
        {
            File.WriteAllText(filePath, AgentMemory);
        }

        AgentMemory = "";
    }
}

//之後做 從上次場景、上次位置生成人物
//public string lastStayedScene;
//public Vector3 lastLocation;