using System.Collections;
using System.Collections.Generic;
using Project.Controllers;
using UnityEngine;
using SA;
using Project.Networking;

public class BaseBot : PlayerController
{
    NetworkClient networkClient;
    // BaseBot baseBot;

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

    public void UpdateTarget(Transform target)
    {
        currentTarget = target;
        lookTarget = currentTarget;
    }

    public void FaceTarget()
    {
        dot = Vector3.Dot((transform.position - lookTarget.transform.position).normalized, -transform.right.normalized);

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
    }

    #endregion
}
