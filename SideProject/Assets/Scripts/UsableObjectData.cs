using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object_Name", menuName = "ScriptableObjects/UsableObject/NewObject", order = 2)]
public class UsableObjectData : ScriptableObject
{
    public Sprite objectVisual;
    public string objectNameChi;
    public string objectNameEng;
    [TextArea(3,10)]
    public string description;
    public float coolDown; 
    public AudioClip sound;
}
