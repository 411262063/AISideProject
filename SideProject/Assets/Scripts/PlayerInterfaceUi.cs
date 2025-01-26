using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerInterfaceUi : MonoBehaviour
{
    public static PlayerInterfaceUi Instance; 
    public Transform npcButtonsGrp;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateViewNpcButton()
    {
        for (int i = 0; i < GameManager.Instance.activeNpcs.Count; i++)
        {
            //npcButtonsGrp.GetChild(i).GetComponent<Button>().
        }
    }

    private void PassNpcTransform()
    {

    }
}
