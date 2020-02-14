using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.ScriptableObjects;
using Project.Utilities;
using UnityEngine;

namespace Project.Managers 
{

    public class BotManager : Singleton<BotManager>
    {
        public List<GameObject> targets = new List<GameObject>();
        [SerializeField]
        GameObject tankBot;
        [SerializeField]
        public Bots bots;
        // [SerializeField]
        // int maxBots = 8;
        [SerializeField]
        GameObject spawnPoints;
        Transform spawnPoint;
        List<Transform> spawnPointsArr = new List<Transform>();
        public static List<BaseBot> SpawnedBots = new List<BaseBot>();
        public bool displayTrajectories = false;
        [SerializeField]
        bool spawnBots;

        string botToSpawn = "Basic_Bot";

        void Start()
        {
            // if (NetworkClient.ClientID == null && spawnBots)
            //     Initialize(maxBots);
        }
        public void Initialize(int botCount)
        {
            for (int i = 0; i < spawnPoints.transform.childCount; i++)
            {
                spawnPointsArr.Add(spawnPoints.transform.GetChild(i));
            }
            for (int i = 0; i < botCount; i++)
            {
                SpawnBot();
            }
            UpdateBotTargets();
        }

        public void SetBotsToSpawn(string botType)
        {
            botToSpawn = botType;
        }
        public GameObject SpawnBot(bool canSpawn = false)
        {
            if (!canSpawn)
            {
                spawnPoint = spawnPointsArr[Random.Range(0, spawnPointsArr.Count - 1)];
                foreach(BaseBot spawnedBot in SpawnedBots)
                {
                    if ((spawnedBot.transform.position - spawnPoint.position).sqrMagnitude < 100)
                    {
                        Invoke("SpawnBot", 0.25f);
                        return null;
                    }
                }
                SpawnBot(true);
            } else {
                GameObject bot = bots.GenerateBotByName(botToSpawn);
                bot.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                SpawnedBots.Add(bot.GetComponent<BaseBot>());
                return bot;
            }
            return null;
        }

        public GameObject SpawnBot(Vector3 pos, Quaternion rot, bool isHost)
        {
            GameObject bot = bots.GenerateBotByName(botToSpawn);
            bot.transform.SetPositionAndRotation(pos, rot);
            BaseBot botController = bot.GetComponent<BaseBot>();
            SpawnedBots.Add(botController);
            if (!isHost) {
                botController.enabled = false;
            }
            UpdateBotTargets();
            return bot;
        }

        public void UpdateBotHost()
        {
            Debug.Log("updating bot host");
            EnableBots();
        }

        public void RemoveBot(BaseBot toRemove)
        {
            if (SpawnedBots.Contains(toRemove))
            {
                SpawnedBots.Remove(toRemove);
                Destroy(toRemove.gameObject);
            }
            UpdateBotTargets();
        }
        public void ClearSpawnedBots()
        {
            SpawnedBots.Clear();
        }
        public void EnableBots()
        {
            foreach(BaseBot bot in SpawnedBots)
            {
                bot.enabled = true;
                Debug.Log(bot.name + " enabled: " + bot.enabled);
            }
        }
        public void DisableBots()
        {
            foreach(BaseBot bot in SpawnedBots)
            {
                bot.enabled = false;
                Debug.Log(bot.name + " enabled: " + bot.enabled);
            }

        }
        public void UpdateBotTargets()
        {
            // if (SpawnedBots.Count < 1) return;
            List<Transform> transformArr = new List<Transform>();
            if (NetworkClient.ClientID != null) 
            {
                transformArr.AddRange(NetworkClient.SpawnedPlayers);
            }
            foreach(BaseBot bot in SpawnedBots)
            {
                transformArr.Add(bot.transform);
            }
            foreach(BaseBot bot in SpawnedBots)
            {
                bot.SetBotTargets(transformArr);
            }
        }
    }
}
