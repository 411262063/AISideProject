using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using System;

public class NpcController : AgentController
{
    [Space(25)]
    public float moveInterval = 3f;
    private Vector2 movementRange = new Vector2(20f, 15f);
    private float moveTimer = 0f;
    public float nextActionCD = 2f;

    private Action pausedAction;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if( currentAction == ActionState.idle &&
            currentMovement == MovementState.none)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveInterval)
            {
                moveTimer = 0f;
                DecideNextAction();
            }
        }
    }

    private void DecideNextAction()
    {
        if (GameManager.Instance.usableObjects.Count > 0 && UnityEngine.Random.value > 0.5)
        {
            AttempToUseObject();
        }
        else
        {
            Wandering();
        }
    }

    private void Wandering()
    {
        SetActionState(ActionState.wandering);
        Debug.Log(character.charNameChi + "隨便亂走");
        Vector3 randomPos;
        float minDistance = 3f;
        do
        {
            randomPos = new Vector3(
            UnityEngine.Random.Range(-movementRange.x, movementRange.x),
            UnityEngine.Random.Range(-movementRange.y, movementRange.y),
            0f);
        }
        while (GameManager.Instance.usableObjects.Exists(obj => Vector3.Distance(randomPos, obj.transform.position) < minDistance));
        //查找是否有物件距離randomPos過近並避開
        currentUsingObj = null; //使MoveTo將狀態設為Moving 
        MoveTo(randomPos);
        StartCoroutine(WanderingProcess());
    }

    private IEnumerator WanderingProcess()
    {
        while (currentMovement != MovementState.reachOrAtObject)
        {
            yield return null;
        }
        yield return new WaitForSeconds(nextActionCD);
        DecideNextAction(); 
    }

    public override void Chatting()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        switch (currentMovement)
        {
            case MovementState.none:
                pausedAction = () => DecideNextAction();
                break;

            case MovementState.moving:
                pausedAction = () => MoveTo(targetPos);
                break;

            case MovementState.approachingToObject:
                pausedAction = () => MoveTo(currentUsingObj.transform.position);
                break;

            case MovementState.reachOrAtObject:
                if (currentAction == ActionState.usingObject)
                    pausedAction = () => StartCoroutine(UsingCurrentObjectProcess());
                else
                    pausedAction = () => DecideNextAction();
                break;

        }

        base.Chatting();
    }

    public override void SummarizeConversation(string convRecord)
    {
        base.SummarizeConversation(convRecord);
        pausedAction?.Invoke();
        pausedAction = null;
    }

    private void AttempToUseObject()
    {
        currentUsingObj = null;
        UsableObjectController targetObj = GameManager.Instance.usableObjects[UnityEngine.Random.Range(0, GameManager.Instance.usableObjects.Count)];

        if (!targetObj.CanBeUse())
        {
            if (targetObj.agentInUse) 
                Debug.Log(character.charNameChi + " 想要使用，但 " + targetObj.agentInUse.character.charNameChi + " 已經在使用 "); //物件進入冷卻前正在使用的人agentInUse已經被清空，先檢查null
            else
                Debug.Log(character.charNameChi + " 想要使用 " + targetObj.objectData.objectNameChi + " 但 還在冷卻中 ");
            
            SetMovementState(MovementState.none);
            DecideNextAction();
        }
        else
        {
            Debug.Log(character.charNameChi + " 即將使用 " + targetObj.objectData.objectNameChi);
            currentUsingObj = targetObj;
            MoveTo(targetObj.transform.position);
            StartCoroutine(UsingCurrentObjectProcess());
        }
    }
}