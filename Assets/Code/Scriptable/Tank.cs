using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

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

            if (barrel == "DEFAULT") barrel = barrels.Keys.ElementAt(0);
            if (body == "DEFAULT") body = bodies.Keys.ElementAt(0);
            if (cannonBase == "DEFAULT") cannonBase = cannonBases.Keys.ElementAt(0);

            tank["barrel"] = Instantiate(barrels[barrel]);
            tank["cannon"] = Instantiate(cannonBases[cannonBase]);
            tank["body"] = Instantiate(bodies[body]);
            tank["tread"] = Instantiate(treads[tread]);

            tank["barrel"].name = "barrel";
            tank["cannon"].name = "cannon";
            tank["body"].name = "body";
            tank["tread"].name = "tread";

            tank["barrel"].layer = 10;
            tank["cannon"].layer = 10;
            tank["body"].layer = 10;
            tank["tread"].layer = 10;

            tank["barrel"].GetComponent<MeshRenderer>().sharedMaterial = mat;
            tank["cannon"].GetComponent<MeshRenderer>().sharedMaterial = mat;
            tank["body"].GetComponent<MeshRenderer>().sharedMaterial = mat;
            tank["tread"].GetComponent<MeshRenderer>().sharedMaterial = mat;

            tank["barrel"].AddComponent<MeshCollider>().convex = true;
            tank["cannon"].AddComponent<MeshCollider>().convex = true;
            tank["body"].AddComponent<MeshCollider>().convex = true;
            tank["tread"].AddComponent<MeshCollider>().convex = true;

            tank["barrel"].transform.SetParent(tankBase.Find("[ Model ]/[ Body ]/[ Cannon ]/[ Barrel ]"), false);
            tank["cannon"].transform.SetParent(tankBase.Find("[ Model ]/[ Body ]/[ Cannon ]"), false);
            tank["body"].transform.SetParent(tankBase.Find("[ Model ]/[ Body ]"), false);
            tank["tread"].transform.SetParent(tankBase.Find("[ Model ]/[ Tread ]"), false);

            tank["barrel"].transform.localEulerAngles = Vector3.zero;
            tank["cannon"].transform.localEulerAngles = Vector3.zero;
            tank["body"].transform.localEulerAngles = Vector3.zero;
            tank["tread"].transform.localEulerAngles = Vector3.zero;

            return tank;
        }
        public void SetTankColor(Dictionary<string, GameObject> tankParts, Color newColor)
        {
            if (newColor.a == -1) return;
            foreach(GameObject part in tankParts.Values) {
                // Debug.Log(part.name);
                part.GetComponent<MeshRenderer>().material.SetColor("_primaryColor", newColor);
            };
        }
    }
}