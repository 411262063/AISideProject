using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class InteractObjectController : MonoBehaviour
{
    public InteractObjectData objectData;
    
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;
    
    private bool isInteracting = false;
    private CharacterData interactingCharacter;

    public enum InteractionState
    {
        idle,
        interacting,
    }
    public InteractionState interactionState;

    private void Start()
    {
        HideHintPanel();
    }

    public bool CanInteract()
    {
        return !isInteracting;
    }

    public void StartInteraction(CharacterData character)
    {
        isInteracting = true;
        interactingCharacter = character;
    }

    public void EndInteraction()
    {
        isInteracting = false;
        interactingCharacter = null;
        HideHintPanel();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            ShowHintPanel();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        HideHintPanel();
    }

    private void ShowHintPanel()
    {
        hintPanel?.SetActive(true);

        switch (interactionState)
        {
            case InteractionState.idle:
                hintText.text = "Press E to interact";
                break;

            case InteractionState.interacting:
                hintText.text = interactingCharacter.charNameChi + " is using " + this.objectData.objectNameEng;
                break;
        }
    }

    private void HideHintPanel()
    {
        hintPanel?.SetActive(false);
    }
}
