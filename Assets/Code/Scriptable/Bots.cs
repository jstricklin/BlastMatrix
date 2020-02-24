using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Project.Controllers;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "Bots", menuName = "ScriptableObjects/Bots", order = 4)]
    public class Bots : SerializedScriptableObject
    {
        public Dictionary<string, GameObject> bots;
        public Tank tank;
        public ColorSettings colorSettings;

        public GameObject GenerateBotByName(string name) 
        {
            GameObject bot = Instantiate(bots[name]);
            // Color newPrimary = Random.ColorHSV();
            // Color newGlow = Random.ColorHSV();
            Dictionary<string, GameObject> tankParts = tank.GenerateTankParts(bot.transform);
            // Color newPrimary = Random.ColorHSV();
            Color newPrimary = colorSettings.primaryColors[Random.Range(0, colorSettings.primaryColors.Length - 1)];

            tank.SetTankColor(tankParts, newPrimary);

            PlayerController botController = bot.GetComponent<PlayerController>();
            botController.GenerateDestroyedModel(tankParts);
            return bot;
        }
    }
}