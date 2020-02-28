using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Controllers;
using UnityEngine.InputSystem;
using System;
using Project.Utilities;
using Project.Networking;
using Cinemachine;
using Sirenix.OdinInspector;
using Project.UI;
using SA;

namespace Project.Controllers {
    public class PlayerController : StateManager, ITargetable, IDamageable
    {
        #region client side (offline) damage logic 
        // public int maxHealth { get; set; }
        // public int currentHealth { get; set; }

        public void DealDamage(int amount)
        {
            currentHealth -= amount;
        }

        public void AddHealth (int amount)
        {
            currentHealth += amount;
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
        }


        #endregion

        // override Rigidbody myRb;
        public float speed = 0f;
        public float maxSpeed = 6.5f;
        public float acceleration = 3f;
        public float turnSpeed = 0.75f;
        public float aimSpeed = 0.5f;
        public float barrelSpeed = 0.25f;
        // private float shotForce = 15;
        private float maxBarrelUp = 45;
        [SerializeField]
        Canvas tankUI;
        [SerializeField]
        private float shotCooldown = 3;
        public Transform cannon, barrel;
        List<GameObject> cloneParts = new List<GameObject>();
        // death vars
        [BoxGroup("Parts to clone for destroy FX")]
        [SerializeField]
        Transform dCannon, dBarrel, dBody;
        GameObject cannonClone, barrelClone, bodyClone;
        bool isDestroyed = false;

        public Transform projectile, projectileSpawnPoint;
        [SerializeField]
        ParticleSystem muzzleFlash;
        [SerializeField]
        public NetworkIdentity networkIdentity;
        ProjectileData projectileData;
        private Quaternion lastRotation;
        WeaponRotation weaponRotation;

        [SerializeField]
        PlayerAudioController audioController;
        public TargetingController targetingController;
        public HingeController hingeController;
        [SerializeField]
        float startHeight;

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

        [HideInInspector]
        public Cooldown cannonCooldown;

        Animator myAnim;

        public virtual void Awake()
        {
            myRb = GetComponent<Rigidbody>();
            myAnim = GetComponent<Animator>();
            cannonCooldown = new Cooldown(shotCooldown);
            projectileData = new ProjectileData();
            weaponRotation = new WeaponRotation();
            // if (isBot) return;
            // EnableInputs();
        }

        void OnEnable() 
        {
            StartCoroutine(HandleSpeed());
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
            if (transform.position.y < startHeight)
            {
                transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
            }
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
            startHeight = transform.position.y;
            if (NetworkClient.ClientID == null)
                GenerateDestroyedModel();
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

        void GenerateDestroyedModel()
        {
            Dictionary<string, GameObject> offlineParts = new Dictionary<string, GameObject>();
            offlineParts["barrel"] = dBarrel.gameObject;
            offlineParts["cannon"] = dCannon.gameObject;
            offlineParts["body"] = dBody.gameObject;
            GenerateDestroyedModel(offlineParts);
        }

        public void GenerateDestroyedModel(Dictionary<string, GameObject> tankParts)
        {
            dBarrel = tankParts["barrel"].transform;
            dCannon = tankParts["cannon"].transform;
            dBody = tankParts["body"].transform;

            barrelClone = Instantiate(dBarrel, dBarrel.position, dBarrel.rotation, dBarrel).gameObject;
            cannonClone = Instantiate(dCannon, dCannon.position, dCannon.rotation, dCannon).gameObject;
            bodyClone = Instantiate(dBody, dBody.position, dBody.rotation, dBody).gameObject;

            cloneParts.Add(barrelClone.gameObject);
            cloneParts.Add(cannonClone.gameObject);
            cloneParts.Add(bodyClone.gameObject);

            if (hingeController.enableHinge) 
            {
                Destroy(bodyClone.GetComponent<Joint>());
            }
            foreach(GameObject clonePart in cloneParts)
            {
                clonePart.layer = 10;
                Rigidbody rb;
                rb = clonePart.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = clonePart.AddComponent<Rigidbody>();
                rb.mass = 5;
                rb.useGravity = true;
                clonePart.SetActive(false);
            }
            // reset anim to account for generated tank parts
            myAnim.Rebind();
        }

        public void DestroyTank()
        {
            foreach(GameObject clonePart in cloneParts)
            {
                clonePart.SetActive(true);
                clonePart.transform.SetParent(null);
                Rigidbody rb = clonePart.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.up * 50, ForceMode.Impulse);
                int x = UnityEngine.Random.Range(1,10);
                int y = UnityEngine.Random.Range(1,10);
                int z = UnityEngine.Random.Range(1,10);
                Vector3 torque = new Vector3(x, y, z);
                rb.AddTorque(torque, ForceMode.Impulse);
            }
            dBarrel.gameObject.SetActive(false);
            dBody.gameObject.SetActive(false);
            dCannon.gameObject.SetActive(false);
        }

        void ResetDestroyedParts()
        {
            barrelClone.transform.position = dBarrel.transform.position;
            barrelClone.transform.rotation = dBarrel.transform.rotation;
            barrelClone.transform.SetParent(dBarrel);
            cannonClone.transform.position = dCannon.transform.position;
            cannonClone.transform.rotation = dCannon.transform.rotation;
            cannonClone.transform.SetParent(dCannon);
            bodyClone.transform.position = dBody.transform.position;
            bodyClone.transform.rotation = dBody.transform.rotation;
            bodyClone.transform.SetParent(dBody);

            foreach(GameObject clonePart in cloneParts)
            {
                clonePart.SetActive(false);
            }
            dBarrel.gameObject.SetActive(true);
            dBody.gameObject.SetActive(true);
            dCannon.gameObject.SetActive(true);
        }

        private void FireCannon(InputAction.CallbackContext obj)
        {
            FireCannon(NetworkClient.ClientID);
        }

        public void FireCannon(string activator) 
        {
            if (cannonCooldown.IsOnCooldown()) return;
            cannonCooldown.StartCooldown();
            projectileData.activator = activator;
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
            hingeController.CannonFired(barrel.transform.forward);
        }

        public virtual void TankHit(Transform attacker)
        {
            myAnim.SetTrigger("isHit");
            Debug.Log("my anim... " + myAnim.IsInTransition(0));
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
        }

        private void AimUp(InputAction.CallbackContext obj)
        {
            AimUp(obj.performed);
        }

        private void AimLeft(InputAction.CallbackContext obj)
        {
            AimLeft(obj.performed);
        }

        private void AimRight(InputAction.CallbackContext obj)
        {
            AimRight(obj.performed);
        }
        private void Move()
        {
            if (isDestroyed) return;
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
            if (isDestroyed) return;
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
            if (isDestroyed) return;
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
                float angle = Vector3.Angle(cannon.forward, barrel.forward);
                if (barrelDir == BarrelDir.UP && angle <= maxBarrelUp)
                {
                    Quaternion deltaRot = Quaternion.FromToRotation(barrel.forward, barrel.up) * barrel.rotation;
                    barrel.rotation = Quaternion.Slerp(barrel.rotation, deltaRot, barrelSpeed * Time.deltaTime);
                } else if (barrelDir == BarrelDir.DOWN && angle > 0){
                    Quaternion deltaRot = Quaternion.FromToRotation(barrel.forward, -barrel.up) * barrel.rotation;
                    barrel.rotation = Quaternion.Slerp(barrel.rotation, deltaRot, barrelSpeed * Time.deltaTime);
                } else {
                    barrelDir = BarrelDir.IDLE;
                }
            }
        }

        #region Actions

        // [SerializeField]
        // StateManager stateManager;
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
            audioController.CannonTurn(aim);
            aimDir = aim ? AimDir.LEFT 
                : aimDir != AimDir.RIGHT ? AimDir.IDLE
                : aimDir;
        }
        public void AimRight(bool aim)
        {
            audioController.CannonTurn(aim);
            aimDir = aim ? AimDir.RIGHT 
                : aimDir != AimDir.LEFT ? AimDir.IDLE
                : aimDir;
        }
        public void AimUp(bool aim)
        {
            audioController.CannonAim(aim);
            barrelDir = aim ? BarrelDir.UP : BarrelDir.IDLE;
        }
        public void AimDown(bool aim)
        {
            audioController.CannonAim(aim);
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

        public virtual void UpdateTarget(Transform target)
        {
            currentTarget = target;
            // lookTarget = currentTarget;
        }

        public IEnumerator HandleSpeed()
        {
            MoveDir lastMove = moveDir;
            while(true)
            {
                if (moveDir != MoveDir.IDLE && speed < maxSpeed)
                {
                    if (lastMove != moveDir)
                    {
                        lastMove = moveDir;
                        speed = 0f;
                    }
                    speed += 0.1f * acceleration;
                } else if (moveDir == MoveDir.IDLE && speed > 0) {
                    speed -= 0.3f;
                    Vector3 dir = lastMove == MoveDir.FORWARD ? transform.forward : -transform.forward;
                    myRb.MovePosition(transform.position + ((dir * speed) * Time.fixedDeltaTime));
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void IsDestroyed()
        {
            isDestroyed = true;
            hingeController.DisableHinge();
            // gameObject.SetActive(false);
            DestroyTank();
        }

        public void SpawnPlayer()
        {
            gameObject.SetActive(true);
            if (isDestroyed)
            {
                isDestroyed = false;
                ResetDestroyedParts();
                hingeController.ResetHinge();
            }
        }

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