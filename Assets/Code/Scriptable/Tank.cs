using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "Tank", menuName = "ScriptableObjects/Tank", order = 4)]
    public class Tank : SerializedScriptableObject
    {
        [SerializeField]
        Material tankMat;
        public Dictionary<string, GameObject> barrels;
        public Dictionary<string, GameObject> cannonBases;
        public Dictionary<string, GameObject> bodies;
        public Dictionary<string, GameObject> treads;
        public Dictionary<string, GameObject> GenerateTankParts(Transform tankBase,  string barrel = "Barrel_01", string cannonBase = "Cannon_01", string body = "Body_01", string tread = "Tread_01") 
        {
            Material mat = Instantiate(tankMat);
            // Debug.Log(name);
            Dictionary<string, GameObject> tank = new Dictionary<string, GameObject>();
            tank["barrel"] = Instantiate(barrels[barrel]);
            tank["cannon"] = Instantiate(cannonBases[cannonBase]);
            tank["body"] = Instantiate(bodies[body]);
            tank["tread"] = Instantiate(treads[tread]);

            tank["barrel"].GetComponent<MeshRenderer>().sharedMaterial = mat;
            tank["cannon"].GetComponent<MeshRenderer>().sharedMaterial = mat;
            tank["body"].GetComponent<MeshRenderer>().sharedMaterial = mat;
            tank["tread"].GetComponent<MeshRenderer>().sharedMaterial = mat;

            tank["barrel"].transform.SetParent(tankBase.Find("[ Barrel ]"), false);
            tank["cannon"].transform.SetParent(tankBase.Find("[ Cannon ]"), false);
            tank["body"].transform.SetParent(tankBase.Find("[ Body ]"), false);
            tank["tread"].transform.SetParent(tankBase.Find("[ Tread ]"), false);

            return tank;
        }
        public void SetTankColor(Dictionary<string, GameObject> tankParts, Color newColor)
        {
            foreach(GameObject part in tankParts.Values) {
                // Debug.Log(part.name);
                part.GetComponent<MeshRenderer>().material.SetColor("_primaryColor", newColor);
            };
        }
    }
}