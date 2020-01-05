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
using SA;

namespace Project.Controllers {
    public class PlayerController : StateManager
    {
        // override Rigidbody myRb;
        private float speed = 6.5f;
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

        [SerializeField]
        PlayerAudioController audioController;

        public enum MoveDir {
            FORWARD,
            BACKWARD,
            IDLE,
        }
        public enum TurnDir {
            LEFT,
            RIGHT,
            IDLE,
        }
        public enum AimDir
        {
            LEFT,
            RIGHT,
            IDLE,
        }
        public enum BarrelDir
        {
            UP,
            DOWN,
            IDLE,
        }
        public MoveDir moveDir = MoveDir.IDLE;
        public TurnDir turnDir = TurnDir.IDLE;
        public AimDir aimDir = AimDir.IDLE;
        public BarrelDir barrelDir = BarrelDir.IDLE;

        Cooldown cannonCooldown;

        Animator myAnim;

        public virtual void Awake()
        {
            myRb = GetComponent<Rigidbody>();
            myAnim = GetComponent<Animator>();
            cannonCooldown = new Cooldown(1.5f);
            projectileData = new ProjectileData();
            weaponRotation = new WeaponRotation();
            if (isBot) return;
            EnableInputs();
        }

        public new virtual void Update()
        {
            if (isBot) 
                base.Update();
            cannonCooldown.CooldownUpdate();
        }
        public new virtual void FixedUpdate()
        {
            if (isBot) 
                base.FixedUpdate();
            Move();
            Turn();
            Aim();
        }

        public new virtual void Start()
        {
            if (isBot) 
                base.Start();
            else if (!networkIdentity.IsControlling()) DisableInputs();
            else {
                CinemachineStateDrivenCamera cam = Camera.main.transform.parent.GetComponent<CinemachineStateDrivenCamera>();
                cam.Follow = cannon;
                cam.LookAt = cannon;
            }
        }

        void OnDisable()
        {
            if (!isBot)
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

        public void FireWeapon()
        {
            muzzleFlash.Play();
            audioController.FireCannon();
        }

        public void TankHit()
        {
            myAnim.SetTrigger("isHit");
        }
        
        private void MoveForward(InputAction.CallbackContext obj)
        {
            MoveForward(obj.performed);
        }

        private void MoveBackwards(InputAction.CallbackContext obj)
        {
            MoveBack(obj.performed);
        }

        private void TurnRight(InputAction.CallbackContext obj)
        {
            TurnRight(obj.performed);
        }

        private void TurnLeft(InputAction.CallbackContext obj)
        {
            TurnLeft(obj.performed);
        }

        private void AimDown(InputAction.CallbackContext obj)
        {
            AimDown(obj.performed);
            audioController.CannonAim(obj.performed);
        }

        private void AimUp(InputAction.CallbackContext obj)
        {
            AimUp(obj.performed);
            audioController.CannonAim(obj.performed);
        }

        private void AimLeft(InputAction.CallbackContext obj)
        {
            AimLeft(obj.performed);
            audioController.CannonTurn(obj.performed);
        }

        private void AimRight(InputAction.CallbackContext obj)
        {
            AimRight(obj.performed);
            audioController.CannonTurn(obj.performed);
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

        #region Actions

        [SerializeField]
        StateManager stateManager;
        [HideInInspector]
        public bool isBot = false;
        public void MoveForward(bool move)
        {
            moveDir = move ? MoveDir.FORWARD : MoveDir.IDLE;
        }
        public void MoveBack(bool move)
        {
            moveDir = move ? MoveDir.BACKWARD : MoveDir.IDLE;
        }
        public void TurnRight(bool turn)
        {
            turnDir = turn ? TurnDir.RIGHT 
                : turnDir != TurnDir.LEFT ? TurnDir.IDLE
                : TurnDir.LEFT;
        }
        public void TurnLeft(bool turn)
        {
            turnDir = turn ? TurnDir.LEFT 
                : turnDir != TurnDir.RIGHT ? TurnDir.IDLE
                : TurnDir.RIGHT;

        }
        public void AimLeft(bool aim)
        {
            aimDir = aim ? AimDir.LEFT 
                : aimDir != AimDir.RIGHT ? AimDir.IDLE
                : aimDir;
        }
        public void AimRight(bool aim)
        {
            aimDir = aim ? AimDir.RIGHT 
                : aimDir != AimDir.LEFT ? AimDir.IDLE
                : aimDir;
        }
        public void AimUp(bool aim)
        {
            barrelDir = aim ? BarrelDir.UP : BarrelDir.IDLE;
        }
        public void AimDown(bool aim)
        {

            barrelDir = aim ? BarrelDir.DOWN : BarrelDir.IDLE;
        }
        #endregion
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