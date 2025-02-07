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
    private float lastChatTime = -10f;

    public enum ActionState
    {
        idle,
        usingObject,
        chatting,
        wandering,
    }
    [Header("角色狀態")]
    public ActionState currentAction;
    [SerializeField] 
    protected ActionState previousAction;

    public enum MovementState
    {
        none,
        moving,
        reachOrAtObject,
        approachingToObject,
    }
    [Header("行動狀態")]
    public MovementState currentMovement;
    [SerializeField] 
    protected MovementState previousMovement;
    protected Coroutine moveCoroutine;
    
    [Header("當前目標位置")]
    public Vector3 targetPos;
    
    [Header("正在使用中的物件")]
    public UsableObjectController currentUsingObj;


    protected virtual void Start()
    {
        NoneChattingUi();
    }

    protected virtual void MoveTo(Vector3 position)
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

    protected virtual IEnumerator UsingCurrentObjectProcess()
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

    #region About Chat/Conversation
    public bool CanStartNewChat()
    {
        bool isFirstChat = lastChatTime < 0f;
        return (character.chatIntent >= ChatManager.Instance.minChatIntent && 
                currentAction != ActionState.chatting &&
                currentAction != ActionState.usingObject &&
                (isFirstChat || Time.time - lastChatTime >= character.chatCoolDown));
    }
    
    public virtual void Chatting()
    {
        lastChatTime = Time.time;
        SetActionState(ActionState.chatting);
        SetMovementState(MovementState.none);
    }

    public virtual void SpeakTo(string listenerName, string line)
    {
        //把listenerName丟給ai
        if (!speachBubble.activeInHierarchy) speachBubble.SetActive(true);
        nameText.text = character.charNameEng; //charNameChi
        speachText.text = line;
    }

    public virtual void SummarizeConversation(string convRecord)
    {
        character.AgentMemory += "[conv]" + convRecord;
        NoneChattingUi();
        RestorePreviousAction();
        RestorePreviousMovement();
    }

    public void NoneChattingUi()
    {
        nameText.text = "";
        speachText.text = "";
        speachBubble.SetActive(false);
    }
    #endregion

    #region About Action and Movement
    protected virtual void SetActionState(ActionState newState)
    {
        //chatting or chattingCoolDown action no need to store in previousAction
        previousAction = (currentAction != ActionState.chatting) ? currentAction : previousAction;
        currentAction = newState;
    }

    protected virtual void SetMovementState(MovementState newMove)
    {
        previousMovement = currentMovement;
        currentMovement = newMove;
    }
    protected virtual void RestorePreviousAction()
    {
        SetActionState(previousAction);
    }

    protected virtual void RestorePreviousMovement()
    {
        SetMovementState(previousMovement);
    }
    #endregion
}
