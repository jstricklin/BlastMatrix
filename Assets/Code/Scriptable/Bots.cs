using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "Bots", menuName = "ScriptableObjects/Bots", order = 4)]
    public class Bots : SerializedScriptableObject
    {
        public Dictionary<string, GameObject> bots;

        public GameObject GenerateBotByName(string name) 
        {
            // Debug.Log(name);
            GameObject bot = Instantiate(bots[name]);
            Color newPrimary = Random.ColorHSV();
            // Color newGlow = Random.ColorHSV();
            foreach(MeshRenderer mesh in bot.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material.SetColor("_primaryColor", newPrimary);
                // mesh.material.SetColor("_glowColor", newGlow);
            }
            return bot;
        }
    }
}