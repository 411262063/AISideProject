using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;


public class PlayerInterfaceUi : MonoBehaviour
{
    public static PlayerInterfaceUi Instance; 
    public Transform npcCamButtonsGrp;
    public GameObject npcCamButtonObj;

    public CinemachineVirtualCamera dynamicCamera;
    public CinemachineVirtualCamera topViewCamera;

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

    public void UpdateAllUiElements()
    {
        UpdateCameraButtonForNpc();
    }

    private void UpdateCameraButtonForNpc()
    {
        for (int i = 0; i < GameManager.Instance.activeNpcAgents.Count; i++)
        {
            GameObject npcCamButton = Instantiate(npcCamButtonObj, npcCamButtonsGrp);
            npcCamButton.GetComponent<CharacterCameraButton>().InitializeNpcAndUi(GameManager.Instance.activeNpcAgents[i].character);
        }
    }

    public void CameraFollowCharater(Transform followTransform)
    {
        dynamicCamera.Follow = followTransform;
    }

    public void CameraFullView()
    {

    }

    //private IEnumerator SmoothCamera()
    //{
    //    Vector3 startPos = dynamicCamera.tr
    //}
}
