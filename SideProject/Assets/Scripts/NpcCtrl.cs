using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class NpcCtrl : CharacterController
{
    [Space(25)]
    public bool isWandering = false;
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
        if (isInteracting || isApproachingObject || isWandering) return;
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
        isWandering = true;

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
        while (!isReached)
        {
            yield return null;
        }
        yield return new WaitForSeconds(wanderingCooldown);
        isWandering = false; 
        DecideNextAction(); 
    }

    private void AttempToInteract()
    {
        isApproachingObject = true;

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
            isApproachingObject = false;
            DecideNextAction();
        }
    }

    private IEnumerator InteractingRoutine()
    {
        //Before reached to target pos
        while (!isReached)
        {
            yield return null;
        }
        isApproachingObject = false;

        //During interaction
        isInteracting = true;
        Debug.Log("正在與" + currentInteractingObj.objectData.objectNameChi + "互動");
        yield return new WaitForSeconds(currentInteractingObj.objectData.duration);

        //Finish interaction
        currentInteractingObj.interactionState = InteractObjectController.InteractionState.idle;
        Debug.Log("結束與" + currentInteractingObj.objectData.objectNameChi + "互動");
        currentInteractingObj = null;
        yield return new WaitForSeconds(interactionCooldown);
        isInteracting = false;
        DecideNextAction();
    }
}

//private IEnumerator WanderingAround()
//{
//    while (true)
//    {
//        currentTargetPos = new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0);
//        base.MoveTo(currentTargetPos);
//        while (!isReached)
//        {
//            yield return null;
//        }
//        yield return new WaitForSeconds(waitTime); 
//        isReached = false;
//    }
//}