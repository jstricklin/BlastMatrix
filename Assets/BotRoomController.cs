using System.Collections;
using System.Collections.Generic;
using Project.Managers;
using Project.Networking;
using UnityEngine;

public class BotRoomController : MonoBehaviour
{

    [SerializeField]
    bool spawnOfflineBots;
    [SerializeField]
    int maxBots;

        
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkClient.ClientID == null && spawnOfflineBots)
        {
            BotManager.Instance.SetBotsToSpawn("Basic_Offline");
            BotManager.Instance.Initialize(maxBots);
        }
    }
}
