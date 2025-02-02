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
    private CharacterData interactingCharacter;
    private Coroutine interactionRoutine;

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

    public void StartInteraction(CharacterData character, float usageTime)
    {
        if (interactionState == InteractionState.interacting) return;
        interactingCharacter = character;
        interactionState = InteractionState.interacting;
        interactionRoutine = StartCoroutine(OnInteractionRoutine(usageTime));
    }

    private IEnumerator OnInteractionRoutine(float duration)
    {
        Debug.Log(interactingCharacter.charNameChi + "正在與" + objectData.objectNameChi + "互動");
        yield return new WaitForSeconds(duration);
        EndInteraction();
    }

    public void EndInteraction()
    {
        Debug.Log(interactingCharacter.charNameChi + "結束與" + objectData.objectNameChi + "互動");
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
