using System.Collections;
using System.Collections.Generic;
using Project.Controllers;
using UnityEngine;
using SA;
using Project.Networking;
using Project.Gameplay;

public class BaseBot : PlayerController
{
    // NetworkIdentity networkIdentity;
    public bool attackReady;

    public GameObject testProjectile;
    #region Collision Detection
    [SerializeField]
    Transform obstacle;
    int checkDist = 10;
    public Vector3 avoidVector;
    public Vector3 obstacleHitPoint;

    public bool obstacleInSight;
    bool maneuvering = false;
    [SerializeField]
    bool avoiding;
    [SerializeField]
    bool backingUp;
    #endregion

    // public float maxDist = 100;

    // Start is called before the first frame update
    public override void Awake()
    {
        isBot = true;
        // baseBot = this;
        // networkIdentity = GetComponent<NetworkIdentity>();
        base.Awake();
    }
    public override void Start()
    {
        StartCoroutine(CheckObstacle());
        base.Start();
        // commented until networking re-enabled
        // if (!NetworkClient.isHost) 
        // {
        //     // stateManager.enabled = false;
        // } else {
        //     base.Start();
        // }
    }

    IEnumerator CheckObstacle()
    {
        Ray obstacleRay;
        bool backingUp = false;
        // Debug.Log("pos: " + transform.position);
        RaycastHit obstacleHit;
        while (true)
        {
            // if (disabled)
            // {
            //     yield return new WaitUntil(() => !disabled);
            // }
            // if (attacking || repairing || battleMode == BattleMode.Idle)
            // {
            //     yield return new WaitUntil(() => !attacking && !repairing && battleMode != BattleMode.Idle);
            // }
            // //check slope below
            // if (Physics.Raycast(groundRay, out groundHit, 10f, groundLayer))
            // {
            //     slopeAngle = Vector3.Angle(groundHit.normal, transform.forward) - 90;
            //     Debug.DrawRay(transform.position, transform.forward * lookDist);
            //     Debug.DrawRay(groundHit.point, groundHit.normal * 20, Color.yellow);
            // }
            Vector3 dir;
            if (moveDir == MoveDir.BACKWARD && !backingUp)
            {
                dir = -transform.forward;
            } else {
                dir = transform.forward;
            }
            obstacleRay = new Ray(transform.position, dir);
            if (Physics.Raycast(obstacleRay, out obstacleHit, checkDist, 1 << 0))
            {
                // if (currentTarget != null && obstacleHit.transform != currentTarget.transform || currentTarget == null)
                // {
                    // Debug.Log("obstacle detected - " + obstacleHit.transform.name);
                    obstacleInSight = true;
                    obstacle = obstacleHit.transform;
                    obstacleHitPoint = obstacleHit.point;
                    StartCoroutine(AvoidObstacle());
                    Debug.DrawLine(transform.position, obstacleHit.point, Color.red);
                // }
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + transform.forward * checkDist, Color.green);
                obstacleInSight = false;
                // lookTarget = currentTarget;
                obstacle = null;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator AvoidObstacle()
    {
        while (obstacle != null)
        {
            avoiding = true;
            // if (attacking || repairing)
            // {
            //     yield break;
            // }
            if ((obstacleHitPoint - transform.position).sqrMagnitude < 100)
            {
                avoidTarget.transform.position = obstacleHitPoint;
                lookTarget = avoidTarget.transform;
                MoveForward(false);
                MoveBack(true);
                backingUp = true;
            } else {
                avoidVector = obstacleHitPoint - obstacle.transform.position;
                avoidVector = avoidVector.normalized;
                avoidVector.y = 0;
                avoidTarget.transform.position = obstacleHitPoint + avoidVector.normalized * 5f;
                lookTarget = avoidTarget.transform;
            }
            yield return new WaitForSeconds(0.1f);
        }
        avoiding = false;
        lookTarget = currentTarget;
        yield break;
    }
    public void SetBotTargets(List<Transform> targets)
    {
        List<Transform> tankTargets = new List<Transform>();
        foreach (Transform target in targets)
        {
            if (target != this.transform)
                tankTargets.Add(target);
        }
        targetingController.SetTargets(tankTargets);
    }

    #region BotActions

    public void StartPatrolling()
    {
        currentTarget = targetingController.NearestEnemy();
        lookTarget = currentTarget;
    }

    public override void UpdateTarget(Transform target)
    {
        currentTarget = target;
        lookTarget = currentTarget;
    }

    public void FaceTarget()
    {
        if (lookTarget == null || maneuvering && obstacle != null) return;
        dot = Vector3.Dot((transform.position - lookTarget.transform.position).normalized, transform.right.normalized);

        if (dot < 0f)
        {
            TurnRight(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
        else if (dot > 0f)
        {
            TurnLeft(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, -transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
    }

    public void AimAtTarget()
    {
        aimDot = Vector3.Dot((cannon.position - currentTarget.transform.position).normalized, -cannon.right.normalized);
        if (aimDot > 0.01f)
        {
            AimRight(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
        else if (aimDot < -0.01f)
        {
            AimLeft(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, -transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        } else {
            attackReady = true;
            AimLeft(false);
            AimRight(false);
            return;
        }
        attackReady = false;
    }

    public void StartAttacking()
    {
        StartCoroutine(MainAttackRoutine());
    }

    public void StopAttacking()
    {
        StopCoroutine(MainAttackRoutine());
    }
    public IEnumerator MainAttackRoutine()
    {
        if (!avoiding)
        {
            MoveForward(false);
            MoveBack(false);
            TurnLeft(false);
            TurnRight(false);
        }
        float moveTime = 100000f;
        float lastMove = Time.time;
        while (true)
        {
            switch (targetingController.aimState)
            {
                case TargetingController.AimState.IN_SIGHT : 
                    AimUp(false);
                    AimDown(false);
                    break;
                case TargetingController.AimState.TOO_FAR : 
                    AimUp(false);
                    AimDown(true);
                    break;
                case TargetingController.AimState.TOO_CLOSE :
                    AimDown(false);
                    AimUp(true);
                    break;
            }
            if (attackReady && !cannonCooldown.IsOnCooldown() && targetingController.aimState == TargetingController.AimState.IN_SIGHT)
            {
                // cannonCooldown.StartCooldown();
                if (NetworkClient.ClientID == null)
                    TestFireProjectile();
                else
                    FireCannon(networkIdentity.GetID());
                if (!maneuvering && !avoiding && Random.Range(0, 10) > 7)
                    StartCoroutine(AttackManeuvers());
            }
            // TODO work on routine below
            if (Time.time > lastMove + moveTime + (Random.Range(0, 100000)) && Random.Range(0, 100) > 99)
            {
                // Debug.Log("moving...");
                if (!maneuvering && !avoiding)
                    StartCoroutine(AttackManeuvers());
                lastMove = Time.time;
            } 
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AttackManeuvers()
    {
        // float dot;
        maneuvering = true;
        dot = Vector3.Dot((transform.position - lookTarget.transform.position).normalized, transform.forward.normalized);

        if (Random.Range(0, 10) > 5)
        {
            if (Random.Range(0, 10) > 4) MoveForward(true);
            else MoveBack(true);
        } else {
            MoveBack(false);
            MoveForward(false);
        }
        if (dot > 0f)
        {
            TurnRight(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
        else if (dot < 0f)
        {
            TurnLeft(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, -transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        } else {
            // switch (targetingController.aimState)
            // {
            //     case TargetingController.AimState.IN_SIGHT : 
            //         AimUp(false);
            //         AimDown(false);
            //         break;
            //     case TargetingController.AimState.TOO_FAR : 
            //         AimUp(false);
            //         AimDown(true);
            //         break;
            //     case TargetingController.AimState.TOO_CLOSE :
            //         AimUp(true);
            //         AimDown(false);
            //         break;
            // }
        }

        yield return new WaitForSeconds(Random.Range(2, 5));

        maneuvering = false;
        if (avoiding)
        {
            yield return new WaitUntil(() => !avoiding);
        }
        TurnRight(false);
        TurnLeft(false);
        MoveForward(false);
        MoveBack(false);
        yield break;
    }

    void TestFireProjectile()
    {
        cannonCooldown.StartCooldown();
        var spawnObject = Instantiate(testProjectile);
        Quaternion lookTo = Quaternion.LookRotation(barrel.transform.forward, spawnObject.transform.up);
        spawnObject.transform.rotation = Quaternion.Euler(lookTo.eulerAngles);
        spawnObject.transform.position = projectileSpawnPoint.position;

        Projectile projectile = spawnObject.GetComponent<Projectile>();
        // projectile.SetActivator(activator);
        // networkObjects[activator].GetComponent<PlayerController>().FireWeapon();
        projectile.FireProjectile(25, barrel.transform.forward);
    }

    #endregion
}
