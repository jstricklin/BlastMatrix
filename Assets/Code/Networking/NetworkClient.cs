using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using Project.ScriptableObjects;
using Project.Gameplay;
using Project.Utility;

namespace Project.Networking 
{
    public class NetworkClient : SocketIOController
    {
        public static string ClientID { get; private set; }
        public const float SERVER_UPDATE_TIME = 10;
        [SerializeField]
        Transform networkContainer;
        [SerializeField]
        GameObject playerGO;
        [SerializeField]
        ServerObjects serverObjects;
        Dictionary<string, NetworkIdentity> networkObjects;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            SetupEvents();
            socketIO.Connect();
        }

        void Initialize() 
        {
            networkObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void SetupEvents()
        {
            socketIO.On("open", (e) => {
                Debug.Log("Connection to server established");
            });
            socketIO.On("register", (e) => {
                var id = new JSONObject(e.data)["id"].str;
                ClientID = id;
                Debug.Log("socketID: " + SocketID);
                Debug.Log($"Client Registered with Server - Client ID: {id}");
            });
            socketIO.On("spawn", (e) => {

                JSONObject data = new JSONObject(e.data);
                string id = data["id"].str;
                string name = data["username"].str;

                GameObject go = Instantiate(playerGO, networkContainer);
                go.name = string.Format("Player ({0})", name);

                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                float z = data["position"]["z"].f;

                go.transform.position = new Vector3(x, y, z);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this.socketIO);
                Debug.Log("ni socket: " + ni.GetSocket());
                Debug.Log("ni socket ID: " + ni.SocketID);
                networkObjects.Add(id, ni);
            });
            socketIO.On("serverSpawn", (e) =>
            {
                Debug.Log("server spawn!");
                JSONObject data = new JSONObject(e.data);
                string name = data["name"].str;
                string id = data["id"].ToString().RemoveQuotes();
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                float z = data["position"]["z"].f;
                if (!networkObjects.ContainsKey(id))
                {
                    // Debug.LogFormat("Server wants to spawn '{0}'", name);
                    GameObject prefab = serverObjects.GetObjectByName(name);
                    var spawnObject = Instantiate(prefab, networkContainer);
                    spawnObject.transform.position = new Vector3(x, y, z);
                    NetworkIdentity ni = spawnObject.GetComponent<NetworkIdentity>();
                    ni.SetControllerID(id);
                    ni.SetSocketReference(this.socketIO);

                    // if projectile apply direction as well
                    if (name == "Shell")
                    {
                        float directionX = data["direction"]["x"].f;
                        float directionY = data["direction"]["y"].f;
                        float directionZ = data["direction"]["z"].f;
                        string activator = data["activator"].ToString().RemoveQuotes();
                        float speed = data["speed"].f;

                        float rot = Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg;
                        Vector3 currentRotation = new Vector3(0, 0, rot + 180);
                        spawnObject.transform.rotation = Quaternion.Euler(currentRotation);

                        Projectile projectile = spawnObject.GetComponent<Projectile>();
                        projectile.SetActivator(activator);
                        projectile.Direction = new Vector3(directionX, directionY, directionZ);
                        projectile.Speed = speed;
                    }
                    networkObjects.Add(id, ni);
                }
            });
            socketIO.On("serverDespawn", (e) =>
            {
                string id = new JSONObject(e.data)["id"].str;
                NetworkIdentity ni = networkObjects[id];
                networkObjects.Remove(id);
                Destroy(ni.gameObject);
            });
            socketIO.On("disconnected", (e) => {
                var id = new JSONObject(e.data)["id"].str;
                Debug.Log($"Player has disconnected: {id}");
            });
        }
    }
    [Serializable]
    public class ProjectileData
    {
        public string id;
        public string activator;
        public Vector3 position;
        public Vector3 direction;
    }
}