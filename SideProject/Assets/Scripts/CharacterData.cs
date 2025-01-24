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

    public Vector3 currentLocation;
}
