using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewNpcButton : MonoBehaviour
{
    private Button button;
    private NpcData viewNpc;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void InitializeViewNpc(NpcData npc)
    {
        viewNpc = npc;
    }

    private void PassNpcTransform()
    {
        //GameManager.Instance.CameraFollowNpc(np)
    }
}
