using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AgentController
{
    private Vector3 clickTargetPos;

    void Update()
    {
        MoveOnKeyInput();
        //MoveOnMouseInput();
    }

    private void MoveOnKeyInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direction += Vector3.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) direction += Vector3.right;

        if (direction != Vector3.zero)
        {
            MoveTo(transform.position + direction);
        }
    }

    private void MoveOnMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector3 newClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickTargetPos = new Vector3(newClickPos.x, newClickPos.y, 0);
            MoveTo(clickTargetPos); 
        }
    }
}
