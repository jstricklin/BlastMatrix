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
using Project.Utilities;
using Project.Controllers;
using Project.UI;

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
        [SerializeField]
        bool useLocalHost = false;

        public override void Awake()
        {
            // #if UNITY_EDITOR
            if (useLocalHost) {
                Debug.Log("Connecting to local host server");
                settings.url = "localhost";
                settings.port = 5280;
                settings.sslEnabled = false;
            } else {
                Debug.Log("Connecting to online server");
            }
            // #endif
            base.Awake();
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            SetupEvents();
            socketIO.Connect();
        }
        public string GetClientID()
        {
            return ClientID;
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
            socketIO.On("updatePosition", (e) =>
            {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                // Debug.LogFormat("updating position: {0}", id);
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                float z = data["position"]["z"].f;
                NetworkIdentity ni = networkObjects[id];
                ni.transform.position = new Vector3(x, y, z);
                // if (data["isProjectile"]?.b == true)
                // {
                //     Debug.LogFormat("updating projectile: {0} {1}, {2}, {3}", id, x, y, z);
                //     // Debug.Break();
                // }
            });
            socketIO.On("updateRotation", (e) =>
            {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                Quaternion weaponRot = new Quaternion(data["weaponRotation"]["x"].f, data["weaponRotation"]["y"].f, data["weaponRotation"]["z"].f, data["weaponRotation"]["w"].f);
                Quaternion barrelRot = new Quaternion(data["barrelRotation"]["x"].f, data["barrelRotation"]["y"].f, data["barrelRotation"]["z"].f, data["barrelRotation"]["w"].f);
                Quaternion tankRot = new Quaternion(data["rotation"]["x"].f, data["rotation"]["y"].f, data["rotation"]["z"].f, data["rotation"]["w"].f);
                NetworkIdentity ni = networkObjects[id];
                ni.GetComponent<PlayerController>().SetTankRotations(tankRot, new WeaponRotation(weaponRot, barrelRot));
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
                TankCanvasController canvasController = ni.GetComponentInChildren<TankCanvasController>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this.socketIO);
                if (ni.IsControlling())
                {
                    canvasController.playerLabel.enabled = false;
                    // UIManager.Instance.playerLabel.text = name;
                } else {
                    canvasController.playerLabel.text = name;
                }
                networkObjects.Add(id, ni);
            });
             socketIO.On("playerHit", (e) => {
                 JSONObject data = new JSONObject(e.data);
                string id = data["id"].str;
                NetworkIdentity ni = networkObjects[id];
                if (ni.IsControlling())
                {
                    float currentHealth = data["currentHealth"].f;
                    UIManager.Instance.SetHealth(currentHealth);
                }
            });
             socketIO.On("playerDied", (e) => {
                string id = new JSONObject(e.data)["id"].str;
                NetworkIdentity ni = networkObjects[id];
                if (ni.IsControlling())
                {
                    UIManager.Instance.SetHealth(0);
                }
                ni.gameObject.SetActive(false);
            });
            socketIO.On("playerRespawn", (e) => {
                // Debug.Log("respawning player");
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                float z = data["position"]["z"].f;
                NetworkIdentity ni = networkObjects[id];
                ni.transform.position = new Vector3(x, y, z);
                if (ni.IsControlling())
                {
                    UIManager.Instance.SetHealth(100);
                }
                ni.gameObject.SetActive(true);
            });
            socketIO.On("serverSpawn", (e) =>
            {
                // Debug.Log("server spawn!");
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

                        Vector3 direction = new Vector3(directionX, directionY, directionZ);
                        string activator = data["activator"].ToString().RemoveQuotes();
                        float speed = data["speed"].f;

                        Quaternion lookTo = Quaternion.LookRotation(direction, spawnObject.transform.up);

                        spawnObject.transform.rotation = Quaternion.Euler(lookTo.eulerAngles);

                        Projectile projectile = spawnObject.GetComponent<Projectile>();
                        projectile.SetActivator(activator);
                        networkObjects[activator].GetComponent<PlayerController>().MuzzleFlash();
                        projectile.FireProjectile(speed, direction);
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
            socketIO.On("loadGame", (e) =>
            {
                Debug.Log("switching to game");
                SceneManagementManager.Instance.LoadLevel(levelName: SceneList.LEVEL, onLevelLoaded: (levelName) =>
                {
                    SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
                });
            });
            socketIO.On("disconnected", (e) => {
                string id = new JSONObject(e.data)["id"].str;
                Debug.Log("player disconnected " + networkObjects[id].SocketID);
                GameObject go = networkObjects[id].gameObject;
                Destroy(go); // remove GO from game
                networkObjects.Remove(id);
            });
        }
        // public void AttemptToJoinLobby()
        // {
        //     Debug.Log("attempting join lobby");
        //     socketIO.Emit("joinGame");
        // }
    }
    [Serializable]
    public class Player
    {
        public string id;
        public Position position;
        public Rotation rotation;
    }
    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public class Rotation
    {
        public Quaternion rotation;
        public Quaternion weaponRotation;
        public Quaternion barrelRotation;
    }
    [Serializable]
    public class ProjectileData
    {
        public string id;
        public string activator;
        public Vector3 position;
        public Vector3 direction;
    }
    [Serializable]
    public class IDData
    {
        public string id;
    }
}