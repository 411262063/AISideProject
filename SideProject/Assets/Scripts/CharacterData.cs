using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterData : ScriptableObject
{
    public Sprite bodyVisual;
    public Sprite headVisual;
    public string charNameChi;
    public string charNameEng;
    public int age;
    public string occupation;
    public string interest;
    public string personality;
    [TextArea(3,10)]
    public string description;
    public List<CharacterData> relationship;

    //schedule
    //memory

    public GameObject characterPrefab;
    public Vector3 currentLocation;
    
}

//之後做 從上次場景、上次位置生成人物
//public string lastStayedScene;
//public Vector3 lastLocation;