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
    private float shotForce = 15;
    [SerializeField]
    Transform gunBarrel;
    [SerializeField]
    Transform projectile, projectileSpawnPoint;

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
    TurnDir aimDir = TurnDir.IDLE;

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
        inputController.aimLeft.performed += AimLeft;
        inputController.aimLeft.canceled += AimLeft;
        inputController.aimRight.performed += AimRight;
        inputController.aimRight.canceled += AimRight;
        inputController.fire.performed += FireCannon;
    }

    private void FireCannon(InputAction.CallbackContext obj)
    {
        Transform fired = Instantiate(projectile, projectileSpawnPoint.position, gunBarrel.rotation);
        fired.GetComponent<Rigidbody>().AddForce(gunBarrel.transform.forward * shotForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        Move();
        Turn();
        Aim();
    }
    
    private void MoveForward(InputAction.CallbackContext obj)
    {
        moveDir = obj.performed ? MoveDir.FORWARD : MoveDir.IDLE;
    }

    private void MoveBackwards(InputAction.CallbackContext obj)
    {
        moveDir = obj.performed ? MoveDir.BACKWARD : MoveDir.IDLE;
    }

    private void TurnRight(InputAction.CallbackContext obj)
    {
        turnDir = obj.performed ? TurnDir.Right : TurnDir.IDLE;
    }

    private void TurnLeft(InputAction.CallbackContext obj)
    {
        turnDir = obj.performed ? TurnDir.Left : TurnDir.IDLE;
    }

    private void AimLeft(InputAction.CallbackContext obj)
    {
        aimDir = obj.performed ? TurnDir.Left : TurnDir.IDLE;
    }

    private void AimRight(InputAction.CallbackContext obj)
    {
            Debug.Log("turning");
        aimDir = obj.performed ? TurnDir.Right : TurnDir.IDLE;
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
    private void Aim()
    {
        if (aimDir != TurnDir.IDLE)
        {
            Quaternion rot;
            if (aimDir == TurnDir.Right)
            {
                rot = Quaternion.LookRotation(gunBarrel.right, gunBarrel.up);
            } else {
                rot = Quaternion.LookRotation(-gunBarrel.right, gunBarrel.up);
            }
            gunBarrel.rotation = Quaternion.Slerp(gunBarrel.rotation, rot, Time.deltaTime * turnSpeed);
        }
    }
}
