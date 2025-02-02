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
    public bool isSpeaking;

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
        reach,
        approachingToObject, //npc
    }
    [Header("行動狀態")]
    public MovementState currentMovement = MovementState.none;

    [Header("當前目標位置")]
    public Vector3 targetPos;

    [SerializeField, Header("場景中可互動的物件")]
    protected List<InteractObjectController> interactableObjects;
    
    [Header("正在互動中的物件")]
    public InteractObjectController currentInteractingObj;

    private Coroutine moveCoroutine;

    protected virtual void Start()
    {
        EndSpeaking();
        GetInteracableObjects();
    }

    public void GetInteracableObjects()
    {
        interactableObjects.Clear();
        foreach (var obj in FindObjectsOfType<InteractObjectController>())
        {
            interactableObjects.Add(obj);
        }
    }

    public virtual void MoveTo(Vector3 position)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        targetPos = position;
        moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    protected virtual IEnumerator MoveCoroutine()
    {
        currentMovement = MovementState.moving;
        
        Vector3 direction;
        const float precision = 0.1f;
        Debug.Log("準備前往" + targetPos);

        while (Vector3.Distance(transform.position, targetPos) > precision)
        {
            direction = (targetPos - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
        currentMovement = MovementState.reach;
        Debug.Log("抵達" + targetPos);
    }

    protected IEnumerator InteractingRoutine()
    {
        //Before reached to target pos
        while (currentMovement == MovementState.moving)
        {
            yield return null;
        }

        //During interaction
        currentAction = ActionState.usingObject;
        yield return new WaitForSeconds(character.objectUsageTime);

        //Finish interaction
        currentInteractingObj.interactionState = InteractObjectController.InteractionState.idle;
        currentInteractingObj = null;
        currentAction = ActionState.idle;
    }

    public void SetActionState(ActionState newState)
    {
        if(currentAction != ActionState.chatting)//previousAction不需要儲存chatting的狀態
        {
            previousAction = currentAction; 
        }
        currentAction = newState;
    }

    public void RestorePreviousAction()
    {
        SetActionState(previousAction);
    }

    public void Speak(string speakLine)
    {
        if (!speachBubble.activeInHierarchy) speachBubble.SetActive(true);
        nameText.text = character.charNameEng; //之後改成中文
        speachText.text = speakLine;
    }
    public void EndSpeaking()
    {
        isSpeaking = false;
        nameText.text = "";
        speachText.text = "";
        speachBubble.SetActive(false);
        RestorePreviousAction();
    }
}

//while (true)
//{
//    Vector3 currentPos = transform.position;
//    if (Mathf.Abs(currentPos.x - targetPos.x) > precision) //x向移動
//    {
//        direction = targetPos.x > currentPos.x ? Vector3.right : Vector3.left;
//        currentPos += direction * moveSpeed * Time.deltaTime;

//        if ((direction == Vector3.right && currentPos.x > targetPos.x) ||
//        (direction == Vector3.left && currentPos.x < targetPos.x))
//        {
//            currentPos.x = targetPos.x;
//        }
//    }
//    else if (Mathf.Abs(transform.position.y - targetPos.y) > precision)//y向移動
//    {
//        direction = targetPos.y > currentPos.y ? Vector3.up : Vector3.down;
//        transform.position += direction * moveSpeed * Time.deltaTime;

//        if ((direction == Vector3.up && currentPos.y > targetPos.y) ||
//        (direction == Vector3.down && currentPos.y < targetPos.y))
//        {
//            currentPos.y = targetPos.y;
//        }
//    }
//    else
//    {
//        break;
//    }

//    transform.position = currentPos;
//    yield return null;
//}