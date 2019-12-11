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
using Project.Managers;
using UnityEngine.Networking;
using TMPro;

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
        string playerName;
        [SerializeField]
        TMP_Text serverMessage;
        Chat chat;
        float serverMessageDisplayTime = 10f;
        float lastMessageTime;
        public static string username = "";
        [SerializeField]
        ServerObjects serverObjects;
        [OdinSerialize]
        Dictionary<string, NetworkIdentity> networkObjects;
        [SerializeField]
        bool useLocalHost = false;

        public delegate void UserLogin(string username);

        public static event UserLogin UserLoginHandler;
        public static long responseCode;
        public static bool isHost = false;

        public string GetClientID()
        {
            return ClientID;
        }
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
            // Debug.Break();
            StartCoroutine(CheckServer("http" + (settings.sslEnabled ? "s" : "") + @"://" + settings.url + (!settings.sslEnabled && settings.port != 0 ? ":" + settings.port.ToString() : "")));
            // base.Awake();
        }

        // Start is called before the first frame update
        void Start()
        {
            // StartCoroutine(CheckServerStatus(Time.time));
            // Initialize();
            // SetupEvents();
            // chat = new Chat(serverMessage, serverMessageDisplayTime);
            // StartCoroutine(chat.ChatSystem());
            // socketIO.Connect();
        }
        public void LoginUser(MenuManager menuManager, string name)
        {
            socketIO.Connect();
            socketIO.On("open", (e) => {
                Debug.Log("Connection to server established");
            });
            socketIO.On("register", (e) => {
                var id = new JSONObject(e.data)["id"].str;
                ClientID = id;
                Debug.Log("socketID: " + SocketID);

                socketIO.Emit("registerUsername", JsonUtility.ToJson(new IDData {
                    username = name,
                }));
                Debug.Log($"Client Registered with Server - Client ID: {id}");
            });
            socketIO.On("usernameRegistered", (e) => {
                username = new JSONObject(e.data)["username"].str;
                menuManager.loggedInText.text = $"Logged in as: {username}"; 
                Debug.Log("registered username: " + username);
                FindObjectOfType<MenuManager>().loggedInText.text = $"Logged in as: {username}";
                InitServerCommunication();
            });
            socketIO.On("setHost", (e) => {
                Debug.Log("host event...");
                isHost = true;
            });
        }
        void Initialize() 
        {
            networkObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void SetupEvents()
        {
            socketIO.On("updatePosition", (e) =>
            {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                // Debug.LogFormat("updating position: {0}", id);
                float x = data["position"]["x"].f;
                float y = data["position"]["y"].f;
                float z = data["position"]["z"].f;
                if (!networkObjects.ContainsKey(id)) return;
                NetworkIdentity ni = networkObjects[id];
                ni.transform.position = new Vector3(x, y, z);
                // if (data["isProjectile"]?.b == true)
                // {
                //     Debug.LogFormat("updating projectile: {0} {1}, {2}, {3}", id, x, y, z);
                //     // Debug.Break();
                // }
            });
            socketIO.On("messageReceived", (e) => {
                JSONObject data = new JSONObject(e.data);
                Debug.Log("message received: " + data);
                string messageString = data["message"].str;
                string username = data["player"]["username"].str;
                string msgDate = data["date"].str;
                if (data["serverMessage"].b)
                {
                    chat.UpdateChat(messageString);
                } else {
                    Message message = new Message();
                    message.username = username;
                    message.message = messageString;
                    message.date = msgDate;
                    ChatController.Instance.OnMessageReceived(message);
                }
            });
            socketIO.On("updateRotation", (e) =>
            {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                if (!networkObjects.ContainsKey(id)) return;
                Quaternion weaponRot = new Quaternion(data["weaponRotation"]["x"].f, data["weaponRotation"]["y"].f, data["weaponRotation"]["z"].f, data["weaponRotation"]["w"].f);
                Quaternion barrelRot = new Quaternion(data["barrelRotation"]["x"].f, data["barrelRotation"]["y"].f, data["barrelRotation"]["z"].f, data["barrelRotation"]["w"].f);
                Quaternion tankRot = new Quaternion(data["rotation"]["x"].f, data["rotation"]["y"].f, data["rotation"]["z"].f, data["rotation"]["w"].f);
                NetworkIdentity ni = networkObjects[id];
                ni.GetComponent<PlayerController>().SetTankRotations(tankRot, new WeaponRotation(weaponRot, barrelRot));
            });
            socketIO.On("spawn", (e) => {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].ToString().RemoveQuotes();
                string name = data["username"].ToString().RemoveQuotes();

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
                    playerName = name;
                    canvasController.playerLabel.enabled = false;
                    // UIManager.Instance.playerLabel.text = name;
                } else {
                    Debug.Log("name " + name);
                    canvasController.playerLabel.text = name;
                }
                networkObjects.Add(id, ni);
            });
             socketIO.On("playerHit", (e) => {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].str;
                string attackerId = data["attackerId"].str;
                if (this.GetClientID() == attackerId)
                {
                    int score = (int)data["hitScore"].f;
                    int playerScore = (int)data["playerScore"].f;
                    UIManager.Instance.DisplayHitMarker(score);
                    UIManager.Instance.UpdateScore(playerScore);
                }
                NetworkIdentity ni = networkObjects[id];
                ni.GetComponent<PlayerController>().TankHit();
                if (ni.IsControlling())
                {
                    float currentHealth = data["currentHealth"].f;
                    UIManager.Instance.SetHealth(currentHealth);
                }
            });
             socketIO.On("playerDied", (e) => {
                JSONObject data = new JSONObject(e.data);
                string id = data["id"].str;
                string attackerId = data["attackerId"].str;
                NetworkIdentity ni = networkObjects[id];

                if (this.GetClientID() == attackerId)
                {
                    int score = (int)data["hitScore"].f;
                    int playerScore = (int)data["playerScore"].f;
                    UIManager.Instance.DisplayHitMarker(score);
                    UIManager.Instance.UpdateScore(playerScore);
                }

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
                Debug.Log("server despawn... ");
                string id = new JSONObject(e.data)["id"].str;
                NetworkIdentity ni = networkObjects[id];
                networkObjects.Remove(id);
                Destroy(ni.gameObject);
            });
            socketIO.On("loadGame", (e) =>
            {
                Debug.Log("switching to game");
                LoadGame();
            });

            socketIO.On("exitGame", (e) =>
            {
                Debug.Log("exiting to Main Menu");
                ReturnToMainMenu();
            });

            socketIO.On("updateMatchScores", (e) => {
                JSONObject data = new JSONObject(e.data);
                UIManager.Instance?.UpdateMatchScores(data["matchScores"].str);
            });

            socketIO.On("updateGameClock", (e) => {
                JSONObject data = new JSONObject(e.data);
                UIManager.Instance?.UpdateGameClock(data["timeRemaining"].f);
            });

            socketIO.On("endGame", (e) => {
                JSONObject data = new JSONObject(e.data);
                Debug.Log("end game");
                SceneManagementManager.Instance.LoadLevel(levelName: SceneList.ENDGAME, onLevelLoaded: (levelName) =>
                {
                    SceneManagementManager.Instance.UnLoadLevel(SceneList.UI);
                    SceneManagementManager.Instance.UnLoadLevel(SceneList.LEVEL);
                    EndGameUIController.Instance.SetMatchResults(data["matchResults"].str, (int)data["countdownTime"].f);
                });
            });

            socketIO.On("disconnected", (e) => {
                isHost = false;
                string id = new JSONObject(e.data)["id"].str;
                Debug.Log("player disconnected " + networkObjects[id].SocketID);
                GameObject go = networkObjects[id].gameObject;
                Destroy(go); // remove GO from game
                networkObjects.Remove(id);
            });
        }

        private void InitServerCommunication()
        {
            socketIO.On("lobbyQuery", (e) => {
                JSONObject data = new JSONObject(e.data);
                LobbyBrowserController.Instance.SetLobbyResponse(data);
                // Debug.Log(data);
            });
        }

        public void QueryLobbies()
        {
            socketIO.Emit("queryLobbies");
        }
        public void JoinLobby(LobbyInfo lobbyId)
        {
            socketIO.Emit("joinGame", JsonUtility.ToJson(lobbyId));
        }
        public void CreateLobby(LobbySettings settings)
        {
            Debug.Log("creating lobby!");
            socketIO.Emit("createLobby", JsonUtility.ToJson(settings));
        }
        public void ReturnToMainMenu()
        {
            isHost = false;
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.MAIN_MENU, onLevelLoaded: (levelName) =>
            {
                if (IsSceneLoaded(SceneList.LEVEL)) SceneManagementManager.Instance.UnLoadLevel(SceneList.LEVEL);
                if (IsSceneLoaded(SceneList.UI)) SceneManagementManager.Instance.UnLoadLevel(SceneList.UI);
                if (IsSceneLoaded(SceneList.CHAT)) SceneManagementManager.Instance.UnLoadLevel(SceneList.CHAT);
                if (IsSceneLoaded(SceneList.LOBBY_BROWSER)) SceneManagementManager.Instance.UnLoadLevel(SceneList.LOBBY_BROWSER);
                if (IsSceneLoaded(SceneList.ENDGAME)) SceneManagementManager.Instance.UnLoadLevel(SceneList.ENDGAME);
                FindObjectOfType<MenuManager>().loggedInText.text = $"Logged in as: {username}";
                ClearNetworkObjects();
            });
        }
        public void ClearNetworkObjects()
        {
            networkObjects.Clear();
            for (int i = 0; i < networkContainer.childCount; i++)
            {
                Destroy(networkContainer.GetChild(i).gameObject);
            }
        }
        public void ReturnToMainMenu(string message)
        {
            isHost = false;
            if (SceneManagementManager.Instance.IsSceneLoaded(SceneList.MAIN_MENU))
            {
                FindObjectOfType<MenuManager>().loggedInText.text = $"{message}";
            } else {
                SceneManagementManager.Instance.LoadLevel(levelName: SceneList.MAIN_MENU, onLevelLoaded: (levelName) =>
                {
                    if (IsSceneLoaded(SceneList.LEVEL)) SceneManagementManager.Instance.UnLoadLevel(SceneList.LEVEL);
                    if (IsSceneLoaded(SceneList.UI)) SceneManagementManager.Instance.UnLoadLevel(SceneList.UI);
                    if (IsSceneLoaded(SceneList.CHAT)) SceneManagementManager.Instance.UnLoadLevel(SceneList.CHAT);
                    if (IsSceneLoaded(SceneList.LOBBY_BROWSER)) SceneManagementManager.Instance.UnLoadLevel(SceneList.LOBBY_BROWSER);
                    if (IsSceneLoaded(SceneList.ENDGAME)) SceneManagementManager.Instance.UnLoadLevel(SceneList.ENDGAME);
                    networkObjects.Clear();
                    for (int i = 0; i < networkContainer.childCount; i++)
                    {
                        Debug.Log("destroying network objects...");
                        Destroy(networkContainer.GetChild(i).gameObject);
                    }
                    FindObjectOfType<MenuManager>().loggedInText.text = $"{message}";
                });
            }
        }
        public void SendChatMessage(string text)
        {
            socketIO.Emit("sendMessage", JsonUtility.ToJson(new Message { message = text }));
        }

        void LoadGame()
        {
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.LEVEL, onLevelLoaded: (levelName) =>
            {
                if (IsSceneLoaded(SceneList.ENDGAME)) SceneManagementManager.Instance.UnLoadLevel(SceneList.ENDGAME);
                if (IsSceneLoaded(SceneList.LOBBY_BROWSER)) SceneManagementManager.Instance.UnLoadLevel(SceneList.LOBBY_BROWSER);
                if (IsSceneLoaded(SceneList.MAIN_MENU)) SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
                if (isHost)
                {
                    Debug.Log("is host!");
                    SpawnPoint[] points = FindObjectsOfType<SpawnPoint>();
                    SpawnPoints spawnPoints = new SpawnPoints();
                    // spawnPoints.spawnPoints = points;
                    spawnPoints.spawnPoints = new Position[points.Length];
                    spawnPoints.spawnRotations = new Rotation[points.Length];
                    for (int i = 0; i < points.Length; i++)
                    {
                        spawnPoints.spawnPoints[i] = points[i].position;
                        spawnPoints.spawnRotations[i] = points[i].rotation;
                    }
                    socketIO.Emit("updateSpawnPoints", JsonUtility.ToJson(spawnPoints)); 
                }
                socketIO.Emit("levelLoaded");
            });
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.CHAT, onLevelLoaded: (levelName) => {
                // UIManager.Instance.playerLabel.text = playerName;
            });
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.UI, onLevelLoaded: (levelName) => {
                UIManager.Instance.playerLabel.text = playerName;
            });
        }
        bool IsSceneLoaded(string scene)
        {
            return SceneManagementManager.Instance.IsSceneLoaded(scene);
        }
        public void ExitToMainMenu()
        {
            isHost = false;
            socketIO.Emit("exitGame");
            ReturnToMainMenu();
        }

        public IEnumerator CheckServer(string uri) 
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log("web req err: " + webRequest.error);
                    responseCode = webRequest.responseCode;
                    FindObjectOfType<MenuManager>().loggedInText.text = "Server is down.";
                }
                else
                {
                    Debug.Log("web req: " + webRequest.responseCode);
                    base.Awake();
                    yield return new WaitForSeconds(0.15f);
                    Initialize();
                    SetupEvents();
                    chat = new Chat(serverMessage, serverMessageDisplayTime);
                    StartCoroutine(chat.ChatSystem());
                    responseCode = webRequest.responseCode;
                    FindObjectOfType<MenuManager>().loggedInText.text = "";
                }
            }
            yield break;
        }       
    }

    [Serializable]
    public class LobbySettings 
    {
       public string name;
       public int maxPlayers;
       public string gameType;
       public bool locked;
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
    public class Message 
    {
        public string username;
        public string date;
        public string message;
    }
    [Serializable]
    public class IDData
    {
        public string id;
        public string username;
    }
    [Serializable]
    public class SpawnPoints
    {
        public Position[] spawnPoints;
        public Rotation[] spawnRotations;
    }
}