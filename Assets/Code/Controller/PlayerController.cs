using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Controllers;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRb;
    private float speed = 10;
    private float turnSpeed = 2.5f;

    enum MoveDir {
        FORWARD,
        BACKWARD,
        IDLE,
    }
    enum TurnDir {
        Left,
        Right,
        IDLE,
    }
    MoveDir moveDir = MoveDir.IDLE;
    TurnDir turnDir = TurnDir.IDLE;

    void Awake()
    {
        InputController inputController = FindObjectOfType<InputController>();
        myRb = GetComponent<Rigidbody>();
        inputController.moveBackwards.performed += MoveBackwards;
        inputController.moveBackwards.canceled += MoveBackwards;
        inputController.moveForward.performed += MoveForward;
        inputController.moveForward.canceled += MoveForward;
        inputController.turnLeft.performed += TurnLeft;
        inputController.turnLeft.canceled += TurnLeft;
        inputController.turnRight.performed += TurnRight;
        inputController.turnRight.canceled += TurnRight;
    }

    private void TurnRight(InputAction.CallbackContext obj)
    {
        turnDir = obj.performed ? TurnDir.Right : TurnDir.IDLE;
    }

    private void TurnLeft(InputAction.CallbackContext obj)
    {
        turnDir = obj.performed ? TurnDir.Left : TurnDir.IDLE;
    }

    void FixedUpdate()
    {
        Move();
        Turn();
    }
    
    private void MoveForward(InputAction.CallbackContext obj)
    {
        moveDir = obj.performed ? MoveDir.FORWARD : MoveDir.IDLE;
    }

    private void MoveBackwards(InputAction.CallbackContext obj)
    {
        moveDir = obj.performed ? MoveDir.BACKWARD : MoveDir.IDLE;
    }

    private void Move()
    {
        if (moveDir != MoveDir.IDLE)
        {
            if (moveDir == MoveDir.FORWARD)
            {
                myRb.MovePosition(transform.position + ((transform.forward * speed) * Time.fixedDeltaTime));
            } else {
                myRb.MovePosition(transform.position + ((-transform.forward * speed) * Time.fixedDeltaTime));
            }
        }
    }
    private void Turn()
    {
        if (turnDir != TurnDir.IDLE)
        {
            Quaternion rot;
            if (turnDir == TurnDir.Right)
            {
                rot = Quaternion.LookRotation(transform.right, transform.up);
            } else {
                rot = Quaternion.LookRotation(-transform.right, transform.up);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
        }
    }
}
