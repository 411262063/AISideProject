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

    private int higherCamPriority = 10;
    private int lowerCamPriority = 5;

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

    private void Start()
    {
        dynamicCamera.Priority = higherCamPriority;
        topViewCamera.Priority = lowerCamPriority;
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
        SetVCamPriority(dynamicCamera, topViewCamera);
    }

    public void CameraFullView()
    {
        topViewCamera.Follow = null;
        SetVCamPriority(topViewCamera, dynamicCamera);
    }

    private void SetVCamPriority(CinemachineVirtualCamera activeCam, CinemachineVirtualCamera inactiveCam)
    {
        activeCam.Priority = higherCamPriority;
        inactiveCam.Priority = lowerCamPriority;
    }
}
