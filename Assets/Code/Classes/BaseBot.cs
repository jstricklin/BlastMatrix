using System.Collections;
using System.Collections.Generic;
using Project.Controllers;
using UnityEngine;
using SA;
using Project.Networking;
using Project.Gameplay;

public class BaseBot : PlayerController
{
    NetworkClient networkClient;
    public bool attackReady;

    public GameObject testProjectile;

    // public float maxDist = 100;

    // Start is called before the first frame update
    public override void Awake()
    {
        isBot = true;
        // baseBot = this;
        networkClient = FindObjectOfType<NetworkClient>();
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        // commented until networking re-enabled
        // if (!NetworkClient.isHost) 
        // {
        //     // stateManager.enabled = false;
        // } else {
        //     base.Start();
        // }
    }

    public void SetBotTargets(List<Transform> targets)
    {
        List<Transform> tankTargets = new List<Transform>();
        foreach (Transform bot in targets)
        {
            if (bot != this.transform)
                tankTargets.Add(bot);
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
        dot = Vector3.Dot((transform.position - lookTarget.transform.position).normalized, -transform.forward.normalized);

        if (dot < 0.9f)
        {
            TurnRight(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
        else if (dot > -0.9f)
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
        MoveForward(false);
        MoveBack(false);
        TurnLeft(false);
        TurnRight(false);
        float moveTime = 5f;
        float lastMove = Time.time;
        while (true)
        {
            if (attackReady && !cannonCooldown.IsOnCooldown())
            {
                cannonCooldown.StartCooldown();
                FireWeapon();
            }
            if (Time.time > lastMove + moveTime && Random.Range(0, 10) > 9)
            {
                Debug.Log("moving...");
                AttackManeuvers();
                lastMove = Time.time;
            } else {
                MoveForward(false);
                TurnLeft(false);
                TurnRight(false);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void AttackManeuvers()
    {
        // float dot;
        dot = Vector3.Dot((transform.position - lookTarget.transform.position).normalized, transform.forward.normalized);

        if (dot > 0.01f)
        {
            TurnRight(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
        else if (dot < -0.01f)
        {
            TurnLeft(true);
            // Quaternion deltaRot = Quaternion.FromToRotation(transform.forward, -transform.right) * transform.rotation;
            // transform.rotation = Quaternion.Slerp(transform.rotation, deltaRot, turnSpeed * Time.deltaTime);
        }
        MoveForward(true);
    }

    void TestFireProjectile()
    {
        var spawnObject = Instantiate(testProjectile);
        Quaternion lookTo = Quaternion.LookRotation(barrel.transform.forward, spawnObject.transform.up);

        spawnObject.transform.rotation = Quaternion.Euler(lookTo.eulerAngles);

        Projectile projectile = spawnObject.GetComponent<Projectile>();
        // projectile.SetActivator(activator);
        // networkObjects[activator].GetComponent<PlayerController>().FireWeapon();
        projectile.FireProjectile(10, barrel.transform.forward);
    }

    #endregion
}
