using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "Tank", menuName = "ScriptableObjects/Tank", order = 4)]
    public class Tank : SerializedScriptableObject
    {
        public Dictionary<string, GameObject> barrels;
        public Dictionary<string, GameObject> cannonBases;
        public Dictionary<string, GameObject> bodies;
        public Dictionary<string, GameObject> treads;
        public Dictionary<string, GameObject> GenerateTankParts(Transform tankBase,  string barrel = "Barrel_01", string cannonBase = "CannonBase_01", string body = "Body_01", string tread = "Tread_01") 
        {
            // Debug.Log(name);
            // GameObject bot = Instantiate(bots[name]);
            // GameObject[] tankParts = GameObject[4];
            Dictionary<string, GameObject> tank = new Dictionary<string, GameObject>();
            tank["barrel"] = Instantiate(barrels[barrel]);
            tank["cannon"] = Instantiate(cannonBases[cannonBase]);
            tank["body"] = Instantiate(bodies[body]);
            tank["tread"] = Instantiate(treads[tread]);

            tank["barrel"].transform.SetParent(tankBase.Find("[ Barrel ]"));
            tank["cannon"].transform.SetParent(tankBase.Find("[ Weapon ]"));
            tank["body"].transform.SetParent(tankBase.Find("[ Body ]"));
            tank["tread"].transform.SetParent(tankBase.Find("[ Tread ]"));
            // Color newPrimary = Random.ColorHSV();
            // Color newGlow = Random.ColorHSV();
            // foreach(MeshRenderer mesh in bot.GetComponentsInChildren<MeshRenderer>())
            // {
            //     mesh.material.SetColor("_primaryColor", newPrimary);
                // mesh.material.SetColor("_glowColor", newGlow);
            // }
            return tank;
        }
    }
}