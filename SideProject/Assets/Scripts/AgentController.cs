using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class AgentController : MonoBehaviour
{
    [Header("角色資料")]
    public CharacterData character;
    public float moveSpeed = 3f;

    [Header("對話框")]
    public GameObject speachBubble;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speachText;

    public enum ActionState
    {
        idle,
        usingObject,
        chatting,
        wandering,
    }
    [Header("角色狀態")]
    public ActionState currentAction;
    public ActionState previousAction;

    public enum MovementState
    {
        none,
        moving,
        approachingToObject, //npc
    }
    [Header("行動狀態")]
    public MovementState currentMovement;
    public MovementState previousMovement;

    [Header("當前目標位置")]
    public Vector3 targetPos;
    
    [Header("正在使用中的物件")]
    public UsableObjectController currentUsingObj;

    private Coroutine moveCoroutine;

    protected virtual void Start()
    {
        EndSpeaking();
    }

    public virtual void MoveTo(Vector3 position)
    {
        SetMovementState(MovementState.moving);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        targetPos = position;
        moveCoroutine = StartCoroutine(MovingProcess());
    }

    public virtual void ApproachingToObject() 
    {
        SetMovementState(MovementState.approachingToObject);
        MoveTo(currentUsingObj.transform.position);
        //前往物件與使用物件邏輯分開 
        //之後玩家透過點擊物件觸發 (解決螢幕位置和ui打架的問題)
    }

    protected virtual IEnumerator MovingProcess()
    {
        Vector3 direction;
        const float precision = 0.1f;

        while (Vector3.Distance(transform.position, targetPos) > precision)
        {
            direction = (targetPos - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if( currentMovement == MovementState.approachingToObject)
            {
                if (!currentUsingObj.CanBeUse()) 
                {
                    SetMovementState(MovementState.none);
                    currentUsingObj = null;
                    yield break; 
                }
            }
            yield return null;
        }

        transform.position = targetPos;
        OnReached();
    }

    private void OnReached()
    {
        switch (currentMovement)
        {
            case MovementState.moving:
                SetMovementState(MovementState.none);
                break;

            case MovementState.approachingToObject:
                SetMovementState(MovementState.none);
                StartUsingObject();
                break;
        }
    }

    private void StartUsingObject()
    {
        SetActionState(ActionState.usingObject);
        currentUsingObj.StartUsingByAgent(this);
    }

    public void EndUsingByCurrentObj() //invoke by currentUsingObj
    {
        SetActionState(ActionState.idle);
        SetMovementState(MovementState.none);
        currentUsingObj = null;
    }

    public void SetActionState(ActionState newState)
    {
        previousAction = (currentAction != ActionState.chatting) ? currentAction : previousAction; //當前若為聊天狀態則不須存在previousAction
        currentAction = newState;
    }

    public void SetMovementState(MovementState newMove)
    {
        previousMovement = currentMovement;
        currentMovement = newMove;
    }

    public void RestorePreviousAction()
    {
        SetActionState(previousAction);
    }

    public void RestorePreviousMovement()
    {
        SetMovementState(previousMovement);
    }

    public void Speak(string speakLine)
    {
        if (!speachBubble.activeInHierarchy) speachBubble.SetActive(true);
        nameText.text = character.charNameEng; //之後改成中文
        speachText.text = speakLine;
    }

    public void Respond(string speakLine)
    {
        if (!speachBubble.activeInHierarchy) speachBubble.SetActive(true);
        nameText.text = character.charNameEng; //之後改成中文
        speachText.text = speakLine;
    }

    public void EndSpeaking()
    {
        nameText.text = "";
        speachText.text = "";
        speachBubble.SetActive(false);
        RestorePreviousAction();
        RestorePreviousMovement();
    }
}
