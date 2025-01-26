using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class NpcCtrl : CharacterController
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
        if (currentAction != ActionState.idle ||
            currentMovement != MovementState.none) 
            return;

        moveTimer += Time.deltaTime;
        if(moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            DecideNextAction();
        }
    }

    private void DecideNextAction()
    {
        if (interactableObjects.Count > 0 && Random.value > 0.5)
        {
            Debug.Log("嘗試互動");
            AttempToInteract();
        }
        else
        {
            Debug.Log("隨便亂走");
            Wandering();
        }
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
            Debug.Log("即將與" + currentInteractingObj.objectData.objectNameChi + "互動");
            MoveTo(targetObj.transform.position);
            StartCoroutine(InteractingRoutine());
        }
        else
        {
            currentMovement = MovementState.none;
            DecideNextAction();
        }
    }

    private IEnumerator InteractingRoutine()
    {
        //Before reached to target pos
        while (currentMovement == MovementState.moving)
        {
            yield return null;
        }

        //During interaction
        currentAction = ActionState.usingObject;
        Debug.Log("正在與" + currentInteractingObj.objectData.objectNameChi + "互動");
        yield return new WaitForSeconds(currentInteractingObj.objectData.duration);

        //Finish interaction
        currentInteractingObj.interactionState = InteractObjectController.InteractionState.idle;
        Debug.Log("結束與" + currentInteractingObj.objectData.objectNameChi + "互動");
        currentInteractingObj = null;
        yield return new WaitForSeconds(interactionCooldown);
        currentAction = ActionState.idle;
        DecideNextAction();
    }
}