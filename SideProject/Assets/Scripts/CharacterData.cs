using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [Header("使用物件時間")]
    public float objectUsageTime;
    [Header("描述"),TextArea(3,10)]
    public string description;
    [Header("人物關係")]
    public List<CharacterData> relationship;

    //schedule
    //memory

    public GameObject characterAgent;
    public Vector3 currentLocation;

    public string agentMemory;
    
    public void AddRelationship(CharacterData charToAdd)
    {
        if (relationship.Contains(charToAdd)) return;
        relationship.Add(charToAdd);
    }
}

//之後做 從上次場景、上次位置生成人物
//public string lastStayedScene;
//public Vector3 lastLocation;