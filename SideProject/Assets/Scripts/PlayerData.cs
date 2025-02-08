using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Character/Player", order = 0)]
public class PlayerData : CharacterData
{
    [Header("天數")]
    public int day;
}
