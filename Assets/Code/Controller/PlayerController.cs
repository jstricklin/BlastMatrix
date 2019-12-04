using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Controllers;
using UnityEngine.InputSystem;
using System;
using Project.Utilities;
using Project.Networking;
using Cinemachine;
using Project.UI;

namespace Project.Controllers {
    public class PlayerController : MonoBehaviour
    {
        Rigidbody myRb;
        private float speed = 5;
        private float turnSpeed = 0.75f;
        private float aimSpeed = 0.5f;
        // private float shotForce = 15;
        private float maxBarrelUp = 0.2f;
        [SerializeField]
        Canvas tankUI;
        [SerializeField]
        private float shotCooldown = 3;
        [SerializeField]
        Transform cannon, barrel;
        [SerializeField]
        Transform projectile, projectileSpawnPoint;
        [SerializeField]
        ParticleSystem muzzleFlash;
        [SerializeField]
        NetworkIdentity networkIdentity;
        ProjectileData projectileData;
        private Quaternion lastRotation;
        WeaponRotation weaponRotation;

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

        Cooldown cannonCooldown;

        Animator myAnim;

        void Awake()
        {
            myRb = GetComponent<Rigidbody>();
            myAnim = GetComponent<Animator>();
            cannonCooldown = new Cooldown(1.5f);
            projectileData = new ProjectileData();
            weaponRotation = new WeaponRotation();
            EnableInputs();
        }

        void Update()
        {
            cannonCooldown.CooldownUpdate();
        }
        void FixedUpdate()
        {
            Move();
            Turn();
            Aim();
        }

        void Start()
        {
            if (!networkIdentity.IsControlling()) DisableInputs();
            else {
                CinemachineStateDrivenCamera cam = Camera.main.transform.parent.GetComponent<CinemachineStateDrivenCamera>();
                cam.Follow = cannon;
                cam.LookAt = cannon;
            }
        }

        void OnDisable()
        {
            DisableInputs();
        }
        void EnableInputs()
        {
            InputController inputController = FindObjectOfType<InputController>();
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

        void DisableInputs()
        {
            InputController inputController = FindObjectOfType<InputController>();
            inputController.moveBackwards.performed -= MoveBackwards;
            inputController.moveBackwards.canceled -= MoveBackwards;
            inputController.moveForward.performed -= MoveForward;
            inputController.moveForward.canceled -= MoveForward;
            inputController.turnLeft.performed -= TurnLeft;
            inputController.turnLeft.canceled -= TurnLeft;
            inputController.turnRight.performed -= TurnRight;
            inputController.turnRight.canceled -= TurnRight;
            inputController.aimLeft.performed -= AimLeft;
            inputController.aimLeft.canceled -= AimLeft;
            inputController.aimRight.performed -= AimRight;
            inputController.aimUp.performed -= AimUp;
            inputController.aimUp.canceled -= AimUp;
            inputController.aimDown.performed -= AimDown;
            inputController.aimDown.canceled -= AimDown;
            inputController.aimRight.canceled -= AimRight;
            inputController.fire.performed -= FireCannon;
        }

        private void FireCannon(InputAction.CallbackContext obj)
        {
            if (cannonCooldown.IsOnCooldown()) return;
            cannonCooldown.StartCooldown();
            projectileData.activator = NetworkClient.ClientID;
            // Debug.Log("activator: " + projectileData.activator);
            projectileData.position.x = projectileSpawnPoint.position.x.TwoDecimals();
            projectileData.position.y = projectileSpawnPoint.position.y.TwoDecimals();
            projectileData.position.z = projectileSpawnPoint.position.z.TwoDecimals();
            projectileData.direction = barrel.transform.forward;
            // Debug.DrawRay(projectileSpawnPoint.position, barrel.transform.forward * 5, Color.blue, 2.5f);
            networkIdentity.GetSocket().Emit("fireProjectile", JsonUtility.ToJson(projectileData));
        }

        public void MuzzleFlash()
        {
            muzzleFlash.Play();
        }

        public void TankHit()
        {
            myAnim.SetTrigger("isHit");
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
            turnDir = obj.performed ? TurnDir.RIGHT 
                : turnDir != TurnDir.LEFT ? TurnDir.IDLE
                : TurnDir.LEFT;
        }

        private void TurnLeft(InputAction.CallbackContext obj)
        {
            turnDir = obj.performed ? TurnDir.LEFT 
                : turnDir != TurnDir.RIGHT ? TurnDir.IDLE
                : TurnDir.RIGHT;
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
            aimDir = obj.performed ? AimDir.LEFT 
                : aimDir != AimDir.RIGHT ? AimDir.IDLE
                : aimDir;
        }

        private void AimRight(InputAction.CallbackContext obj)
        {
            aimDir = obj.performed ? AimDir.RIGHT
                : aimDir != AimDir.LEFT ? AimDir.IDLE
                : aimDir;
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
                    Quaternion deltaRot = Quaternion.FromToRotation(barrel.forward, barrel.up) * barrel.rotation;
                    barrel.rotation = Quaternion.Slerp(barrel.rotation, deltaRot, aimSpeed * Time.deltaTime);
                } else if (barrelDir == BarrelDir.DOWN && barrel.localEulerAngles.x > 0){
                    Quaternion deltaRot = Quaternion.FromToRotation(barrel.forward, -barrel.up) * barrel.rotation;
                    barrel.rotation = Quaternion.Slerp(barrel.rotation, deltaRot, aimSpeed * Time.deltaTime);
                }
            }
        }
        #region Networking
        public Quaternion GetLastRotation()
        {
            lastRotation = transform.rotation;
            return lastRotation;
        }

        public WeaponRotation GetLastWeaponRotation()
        {
            weaponRotation.lastBarrelRotation = barrel.rotation;
            weaponRotation.lastWeaponRotation = cannon.rotation;
            return weaponRotation;
        }

        public void SetTankRotations(Quaternion tankRotation, WeaponRotation weaponRotation)
        {
            transform.rotation = tankRotation;
            cannon.transform.rotation = weaponRotation.lastWeaponRotation;
            barrel.transform.rotation = weaponRotation.lastBarrelRotation;
        }

        #endregion
    }
    public struct WeaponRotation 
    {
        public Quaternion lastWeaponRotation;
        public Quaternion lastBarrelRotation;
        public WeaponRotation(Quaternion weaponRot, Quaternion barrelRot) {
            this.lastWeaponRotation = weaponRot;
            this.lastBarrelRotation = barrelRot;
        }
    }
}