using System;
using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utilities;
using TMPro;
using UnityEngine;

namespace Project.Controllers {
    public class LobbyBrowserController : Singleton<LobbyBrowserController>
    {
        NetworkClient networkClient;
        public enum BrowserMode {
            BROWSE,
            CREATE,
        }
        [SerializeField]
        GameObject browseCanvas, createCanvas;
        BrowserMode browserMode;
        [SerializeField]
        GameObject lobbyDataDisplay;
        [SerializeField]
        Transform lobbyWindow;
        JSONObject lobbyData;
        [SerializeField]
        List<LobbyInfo> lobbyCollection = new List<LobbyInfo>();
        void Start() 
        {
            networkClient = FindObjectOfType<NetworkClient>();
            StartCoroutine(QueryLobbies());
        }  
        public void JoinLobby()
        {
            Debug.Log("joining lobby... ID: ");
            // networkClient.JoinLobby();
        } 
        public void GenerateLobby()
        {
            networkClient.CreateLobby();
        }
        public void CreateLobbies()
        {
            SetBrowserMode(BrowserMode.CREATE);
        } 

        public void BrowseLobbies()
        {
            SetBrowserMode(BrowserMode.BROWSE);
        } 
        public void ReturnToMainMenu()
        {
            networkClient.ReturnToMainMenu();
        }

        IEnumerator QueryLobbies()
        {
            while(true)
            {
                networkClient.QueryLobbies();
                yield break;
                // yield return new WaitForSeconds(1f);
            }
        }
        public void SetLobbyResponse(JSONObject lobbyData)
        {
            lobbyCollection.ForEach(lobby => {
                Destroy(lobby.lobbyDataGO);
            });
            lobbyCollection.Clear();
            Vector3 pos = lobbyDataDisplay.transform.position;
            lobbyData.keys.ForEach((k) => {
                Debug.Log("lobby response " + lobbyData[k]);
                JSONObject data = lobbyData[k];
                string gameMode = data["settings"]["gameMode"].str;
                string name = data["settings"]["name"].str;
                int playerCount = (int)data["playerCount"].n;
                int maxPlayers = (int)data["settings"]["maxPlayers"].n;
                string parsedString = gameMode + " " + name + "     [" + playerCount + " / " + maxPlayers + "]";
                LobbyInfo lobby = new LobbyInfo(Instantiate(lobbyDataDisplay, pos, Quaternion.identity, lobbyWindow), k, parsedString);
                Debug.Log("pos : " + pos);
                // lobby.lobbyDataGO.transform.position = pos;
                lobbyCollection.Add(lobby);
                pos.y -= 55;
            });
        }

        public void SetBrowserMode(BrowserMode mode)
        {
            browserMode = mode;
            browseCanvas.SetActive(browserMode == BrowserMode.BROWSE);
            createCanvas.SetActive(browserMode == BrowserMode.CREATE);
        }
    }
}

class LobbyInfo {
    public GameObject lobbyDataGO;
    TMP_Text lobbyText;
    string lobbyId;

    public LobbyInfo(GameObject LobbyDataGO, string id, string text) 
    {
        this.lobbyId = id;
        this.lobbyDataGO = LobbyDataGO;
        this.lobbyDataGO.SetActive(true);
        this.lobbyText = LobbyDataGO.GetComponentInChildren<TMP_Text>();
        this.lobbyText.text = text;
    }
}