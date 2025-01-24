using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : CharacterController
{
    private Vector3 clickTargetPos;

    void Update()
    {
        MoveOnKeyInput();
        MoveOnMouseInput();
    }

    private void MoveOnKeyInput() //使用WASD行走時
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.down;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

        if (direction != Vector3.zero)
        {
            MoveTo(transform.position + direction);
        }
    }

    private void MoveOnMouseInput() //點擊目標位置並尋找最佳路徑
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector3 newClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickTargetPos = new Vector3(newClickPos.x, newClickPos.y, 0);
            MoveTo(clickTargetPos); 
        }
    }
}
