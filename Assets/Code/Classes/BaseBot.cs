using System.Collections;
using System.Collections.Generic;
using Project.Controllers;
using UnityEngine;
using SA;
using Project.Networking;

public class BaseBot : PlayerController
{
    NetworkClient networkClient;

    // Start is called before the first frame update
    public override void Awake()
    {
        isBot = true;
        networkClient = FindObjectOfType<NetworkClient>();
        base.Awake();
    }
    public override void Start()
    {
        if (!NetworkClient.isHost) 
        {
            // stateManager.enabled = false;
        }
    }
}
