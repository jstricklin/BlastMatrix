using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Controllers;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRb;
    private float speed = 5;
    private float turnSpeed = 1f;
    private float aimSpeed = 0.75f;
    private float shotForce = 15;
    private float maxBarrelUp = 0.2f;
    [SerializeField]
    Transform cannon, barrel;
    [SerializeField]
    Transform projectile, projectileSpawnPoint;

    enum MoveDir {
        FORWARD,
        BACKWARD,
        IDLE,
    }
    enum TurnDir {
        LEFT,
        RIGHT,
        IDLE,
    }
    enum AimDir
    {
        LEFT,
        RIGHT,
        IDLE,
    }
    enum BarrelDir
    {
        UP,
        DOWN,
        IDLE,
    }
    MoveDir moveDir = MoveDir.IDLE;
    TurnDir turnDir = TurnDir.IDLE;
    AimDir aimDir = AimDir.IDLE;
    BarrelDir barrelDir = BarrelDir.IDLE;

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
        inputController.aimUp.performed += AimUp;
        inputController.aimUp.canceled += AimUp;
        inputController.aimDown.performed += AimDown;
        inputController.aimDown.canceled += AimDown;
        inputController.aimRight.canceled += AimRight;
        inputController.fire.performed += FireCannon;
    }

    void FixedUpdate()
    {
        Move();
        Turn();
        Aim();
    }

    private void FireCannon(InputAction.CallbackContext obj)
    {
        Transform fired = Instantiate(projectile, projectileSpawnPoint.position, barrel.rotation);
        fired.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * shotForce, ForceMode.Impulse);
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
        turnDir = obj.performed ? TurnDir.RIGHT : TurnDir.IDLE;
    }

    private void TurnLeft(InputAction.CallbackContext obj)
    {
        turnDir = obj.performed ? TurnDir.LEFT : TurnDir.IDLE;
    }

    private void AimDown(InputAction.CallbackContext obj)
    {
        barrelDir = obj.performed ? BarrelDir.DOWN : BarrelDir.IDLE;
    }

    private void AimUp(InputAction.CallbackContext obj)
    {
        barrelDir = obj.performed ? BarrelDir.UP : BarrelDir.IDLE;
    }

    private void AimLeft(InputAction.CallbackContext obj)
    {
        aimDir = obj.performed ? AimDir.LEFT : AimDir.IDLE;
    }

    private void AimRight(InputAction.CallbackContext obj)
    {
        aimDir = obj.performed ? AimDir.RIGHT : AimDir.IDLE;
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
            if (turnDir == TurnDir.RIGHT)
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
        if (aimDir != AimDir.IDLE)
        {
            Quaternion rot;
            if (aimDir == AimDir.RIGHT)
            {
                rot = Quaternion.LookRotation(cannon.right, cannon.up);
            } else {
                rot = Quaternion.LookRotation(-cannon.right, cannon.up);
            }
            cannon.rotation = Quaternion.Slerp(cannon.rotation, rot, Time.deltaTime * aimSpeed);
        }
        if (barrelDir != BarrelDir.IDLE) 
        {
            if (barrelDir == BarrelDir.UP && barrel.localRotation.x * -1 < maxBarrelUp)
            {
                Debug.Log("local x on barrel " + barrel.localRotation.x);
                Quaternion deltaRot = Quaternion.FromToRotation(barrel.forward, barrel.up) * barrel.rotation;
                barrel.rotation = Quaternion.Slerp(barrel.rotation, deltaRot, aimSpeed * Time.deltaTime);
            } else if (barrelDir == BarrelDir.DOWN && barrel.localEulerAngles.x > 0){
                Quaternion deltaRot = Quaternion.FromToRotation(barrel.forward, -barrel.up) * barrel.rotation;
                barrel.rotation = Quaternion.Slerp(barrel.rotation, deltaRot, aimSpeed * Time.deltaTime);
            }
        }
    }
}
