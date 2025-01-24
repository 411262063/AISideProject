using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    [Header("角色資料")]
    public CharacterData character;
    public float moveSpeed = 3f;

    [Header("當前目標位置")]
    public Vector3 targetPos;

    [Header("是否抵達")]
    public bool isReached = false;

    [Header("是否抵達物件")]
    public bool isApproachingObject = false;

    [SerializeField, Header("場景中可互動的物件")]
    protected List<InteractObjectController> interactableObjects;
    
    [Header("正在互動中的物件")]
    public InteractObjectController currentInteractingObj;

    [Header("是否在互動中")]
    public bool isInteracting = false;

    private Coroutine moveCoroutine;

    protected virtual void Start()
    {
        FindInteracableObjects();
    }

    public void FindInteracableObjects()
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

    private IEnumerator MoveCoroutine()
    {
        isReached = false;
        Vector3 direction;
        const float precision = 0.1f;
        Debug.Log("準備前往" + targetPos);

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

        while (Vector3.Distance(transform.position, targetPos) > precision)
        {
            direction = (targetPos - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
        isReached = true;
        Debug.Log("抵達" + targetPos);
    }
}
