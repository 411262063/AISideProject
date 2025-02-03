using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCameraButton : MonoBehaviour
{
    public CharacterData followCharacter;
    public Button button;
    public TextMeshProUGUI charNameText;
    public Image charHeadImage;
    

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if(followCharacter != null)
            button.onClick.AddListener(() => PassCharacterTransform());
    }

    public void InitializeNpcAndUi(CharacterData character)
    {
        followCharacter = character;
        button.onClick.AddListener(() => PassCharacterTransform());
        charNameText.text = character.charNameEng; //之後改成中文
        //charHeadImage.sprite = character.headVisual;
    }

    public void PassCharacterTransform()
    {
        Transform charTransform;
        GameObject charAgent;

        if (followCharacter == GameManager.Instance.player)
        {
            charAgent = FindAnyObjectByType<PlayerController>().gameObject;
            Debug.Log("following player");
        }
        else
        {
            charAgent = GameManager.Instance.activeNpcAgents.Find(npc => npc.character == followCharacter).gameObject;
        }

        charTransform = charAgent.gameObject.transform;
        PlayerInterfaceUi.Instance.CameraFollowCharater(charTransform);
    }
}
