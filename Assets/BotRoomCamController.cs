using System.Collections;
using System.Collections.Generic;
using Project.Managers;
using UnityEngine;
using Cinemachine;

public class BotRoomCamController : MonoBehaviour
{
    [SerializeField]
    CinemachineFreeLook cinemachine;
    Transform currentTarget;
    void Start()
    {
        Invoke("Initialize", 1f);
    }
    void Initialize()
    {
        currentTarget = BotManager.SpawnedBots[Random.Range(0, BotManager.SpawnedBots.Count - 1)].transform;
        SetViewTarget(currentTarget);
    }
    public void NextViewTarget()
    {
        int index = BotManager.SpawnedBots.IndexOf(currentTarget.GetComponent<BaseBot>())+1;
        if (index == BotManager.SpawnedBots.Count) index = 0;
        currentTarget = BotManager.SpawnedBots[index].transform;
        SetViewTarget(currentTarget);
    }
    public void PreviousViewTarget()
    {
        int index = BotManager.SpawnedBots.IndexOf(currentTarget.GetComponent<BaseBot>())-1;
        if (index == -1) index = BotManager.SpawnedBots.Count - 1;
        currentTarget = BotManager.SpawnedBots[index].transform;
        SetViewTarget(currentTarget);
    }
    void SetViewTarget(Transform target)
    {
        cinemachine.Follow = target;
        cinemachine.LookAt = target;
    }
}
