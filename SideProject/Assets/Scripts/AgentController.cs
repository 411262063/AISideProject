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
    private ActionState previousAction;

    public enum MovementState
    {
        none,
        moving,
        reachOrAtObject,
        approachingToObject,
    }
    [Header("行動狀態")]
    public MovementState currentMovement;
    private MovementState previousMovement;

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
        MovementState newMovement = (currentUsingObj == null) ? MovementState.moving : MovementState.approachingToObject;
        SetMovementState(newMovement);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        targetPos = position;
        moveCoroutine = StartCoroutine(MovingProcess());
    }

    protected virtual IEnumerator MovingProcess()
    {
        Vector3 direction;
        const float precision = 0.1f;

        while (Vector3.Distance(transform.position, targetPos) > precision)
        {
            direction = (targetPos - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if(currentMovement == MovementState.approachingToObject 
                && Vector3.Distance(transform.position, targetPos) <= 1f)
            {
                if (!currentUsingObj.CanBeUse())
                {
                    SetMovementState(MovementState.none);
                    SetActionState(ActionState.idle);

                    if (currentUsingObj.agentInUse)
                        Debug.Log(character.charNameChi + " 接近 " + currentUsingObj.objectData.objectNameChi + " 時發現 " + currentUsingObj.agentInUse.character.charNameChi + " 正在使用，因此中離");
                    else
                        Debug.Log(character.charNameChi + " 接近 " + currentUsingObj.objectData.objectNameChi + " 時發現 還在冷卻中，因此中離");

                    currentUsingObj = null;
                    yield break;
                }
            }
            yield return null;
        }

        transform.position = targetPos;
        SetMovementState(MovementState.reachOrAtObject);
    }

    public virtual IEnumerator UsingCurrentObjectProcess()
    {
        while(currentMovement != MovementState.reachOrAtObject)
        {
            yield return null;
        }
        if(currentUsingObj == null)
        {
            Debug.Log(character.charNameChi + " 在抵達後找不到 currentUsingObj，取消使用");
            EndUsingCurrentObject();
            yield break;
        }
        SetActionState(ActionState.usingObject);
        currentUsingObj.UsingByAgent(this);
    }


    public void EndUsingCurrentObject() //invoke by currentUsingObj
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
