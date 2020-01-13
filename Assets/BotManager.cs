using System.Collections;
using System.Collections.Generic;
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
        int maxBots = 8;
        [SerializeField]
        GameObject spawnPoints;
        List<Transform> spawnPointsArr = new List<Transform>();
        List<BaseBot> spawnedBots = new List<BaseBot>();

        void Start()
        {
            Initialize(8);
        }

        public void Initialize(int botCount)
        {
            for (int i = 0; i < spawnPoints.transform.childCount; i++)
            {
                spawnPointsArr.Add(spawnPoints.transform.GetChild(i));
            }
            maxBots = botCount;
            for (int i = 0; i < maxBots; i++)
            {
                SpawnBot();
            }
            UpdateBotTargets();
        }

        public void SpawnBot()
        {
            Transform spawnPoint = spawnPointsArr[Random.Range(0, spawnPointsArr.Count - 1)];
            GameObject bot = Instantiate(tankBot);
            bot.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            spawnedBots.Add(bot.GetComponent<BaseBot>());
        }

        public void UpdateBotTargets()
        {
            List<Transform> transformArr = new List<Transform>();
            foreach(BaseBot bot in spawnedBots)
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
