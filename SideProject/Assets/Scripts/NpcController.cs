using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class NpcController : AgentController
{
    [Space(25)]
    public float moveInterval = 3f;
    private Vector2 movementRange = new Vector2(20f, 15f);
    private float moveTimer = 0f;
    public float wanderingCooldown = 2f;
    public float interactionCooldown = 2f;


    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        //if (currentAction != ActionState.idle ||
        //    currentMovement != MovementState.none) 
        //    return;

        //moveTimer += Time.deltaTime;
        //if(moveTimer >= moveInterval)
        //{
        //    moveTimer = 0f;
        //    DecideNextAction();
        //}
    }

    private void DecideNextAction()
    {
        //if (interactableObjects.Count > 0 && Random.value > 0.5)
        //{
        //    Debug.Log(character.charNameChi+ "嘗試互動");
        //    AttempToInteract();
        //}
        //else
        //{
        //    Debug.Log(character.charNameChi + "隨便亂走");
        //    Wandering();
        //}
    }

    private void Wandering()
    {
        //movementState = MovementState.wandering;

        Vector3 randomPos = new Vector3(
            Random.Range(-movementRange.x, movementRange.x),
            Random.Range(-movementRange.y, movementRange.y),
            0f
        );

        MoveTo(randomPos);
        StartCoroutine(OnWanderingComplete());
    }

    private IEnumerator OnWanderingComplete()
    {
        while (currentMovement == MovementState.moving)
        {
            yield return null;
        }
        yield return new WaitForSeconds(wanderingCooldown);
        currentMovement = MovementState.none;
        DecideNextAction(); 
    }

    private void AttempToInteract()
    {
        currentMovement = MovementState.approachingToObject;

        InteractObjectController targetObj = interactableObjects[Random.Range(0, interactableObjects.Count)];

        if(targetObj.interactionState == InteractObjectController.InteractionState.idle)
        {
            currentInteractingObj = targetObj;
            targetObj.interactionState = InteractObjectController.InteractionState.interacting;
            Debug.Log(character.charNameChi + "即將與" + currentInteractingObj.objectData.objectNameChi + "互動");
            MoveTo(targetObj.transform.position);
            StartCoroutine(InteractingRoutine());
            DecideNextAction();
        }
        else
        {
            currentMovement = MovementState.none;
            DecideNextAction();
        }
    }
}