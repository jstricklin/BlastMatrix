using System.Collections;
using System.Collections.Generic;
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
        Bots bots;
        [SerializeField]
        int maxBots = 8;
        [SerializeField]
        GameObject spawnPoints;
        Transform spawnPoint;
        List<Transform> spawnPointsArr = new List<Transform>();
        public static List<BaseBot> SpawnedBots = new List<BaseBot>();
        public bool displayTrajectories = false;

        void Start()
        {
            Initialize(maxBots);
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

        public void SpawnBot(bool canSpawn = false)
        {
            // Transform spawnPoint = spawnPointsArr[0];
            // Transform spawnPoint;
            if (!canSpawn)
            {
                spawnPoint = spawnPointsArr[Random.Range(0, spawnPointsArr.Count - 1)];
                foreach(BaseBot spawnedBot in SpawnedBots)
                {
                    if ((spawnedBot.transform.position - spawnPoint.position).sqrMagnitude < 100)
                    {
                        SpawnBot();
                        return;
                    }
                }
                SpawnBot(true);
            } else {
                // GameObject bot = Instantiate(tankBot);
                GameObject bot = bots.GenerateBotByName("Basic_Bot");
                bot.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                SpawnedBots.Add(bot.GetComponent<BaseBot>());
            }
        }

        public void UpdateBotTargets()
        {
            List<Transform> transformArr = new List<Transform>();
            foreach(BaseBot bot in SpawnedBots)
            {
                transformArr.Add(bot.transform);
            }
            foreach(Transform bot in transformArr)
            {
                bot.GetComponent<BaseBot>().SetBotTargets(transformArr);
            }
        }
    }
}
