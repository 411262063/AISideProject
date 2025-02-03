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

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if( currentAction == ActionState.idle ||
            currentAction != ActionState.chatting ||
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
        if (GameManager.Instance.activeUsableObjects.Count > 0 && Random.value > 0.5)
        {
            AttempToInteract();
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
            Random.Range(-movementRange.x, movementRange.x),
            Random.Range(-movementRange.y, movementRange.y),
            0f);
        }
        while (GameManager.Instance.activeUsableObjects.Exists(obj => Vector3.Distance(randomPos, obj.transform.position) < minDistance));

        MoveTo(randomPos);
        StartCoroutine(WanderingProcess());
    }

    private IEnumerator WanderingProcess()
    {
        while (currentMovement == MovementState.moving)
        {
            yield return null;
        }
        yield return new WaitForSeconds(wanderingCooldown);
        DecideNextAction(); 
    }

    private void AttempToInteract()
    {
        UsableObjectController targetObj = GameManager.Instance.activeUsableObjects[Random.Range(0, GameManager.Instance.activeUsableObjects.Count)];

        if (!targetObj.CanBeUse())
        {
            Debug.Log(character.charNameChi + "無法使用" + targetObj.objectData.objectNameChi + "，因為" + targetObj.agentInUse.character.charNameChi + "已經在使用");
            currentUsingObj = null;
            SetMovementState(MovementState.none);
            DecideNextAction();
            return;
        }
        else
        {
            Debug.Log(character.charNameChi + "即將使用" + targetObj.objectData.objectNameChi);
            currentUsingObj = targetObj;
            ApproachingToObject();
        }
    }
}