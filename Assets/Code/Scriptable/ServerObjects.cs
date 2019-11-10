using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "Server_Objects", menuName = "ScriptableObjects/ServerObjects", order = 3)]
    public class ServerObjects : SerializedScriptableObject
    {
        public Dictionary<string, GameObject> serverObjects;

        public GameObject GetObjectByName(string name) 
        {
            // Debug.Log(name);
            return serverObjects[name];
        }
    }
}
