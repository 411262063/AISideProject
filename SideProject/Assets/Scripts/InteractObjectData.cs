using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object_Name", menuName = "ScriptableObjects/InteractObjects/Object", order = 2)]
public class InteractObjectData : ScriptableObject
{
    public Sprite objectVisual;
    public string objectNameChi;
    public string objectNameEng;
    [TextArea(3,10)]
    public string description;
    public float duration = 2f;
    public AudioClip sound;
}
