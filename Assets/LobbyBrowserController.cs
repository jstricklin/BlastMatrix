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
        [SerializeField]
        TMP_InputField lobbyName;
        LobbySettings lobbySettings;

        void Start() 
        {
            networkClient = FindObjectOfType<NetworkClient>();
            lobbyName.ActivateInputField();
            StartCoroutine(QueryLobbies());
        }  
        public void JoinLobby()
        {
            Debug.Log("joining lobby... ID: ");
            // networkClient.JoinLobby();
        } 
        public void GenerateLobby()
        {
            // add server side lobby name validation
            Debug.Log("name: " + lobbyName.text + " and length is " + lobbyName.text.Length);
            if (lobbyName.text.Length < 1) 
            {
                Debug.Log("please enter a server name");
                return;
            }
            lobbySettings = new LobbySettings();
            lobbySettings.name = lobbyName.text;
            networkClient.CreateLobby(lobbySettings);
        }
        public void CreateLobbies()
        {
            SetBrowserMode(BrowserMode.CREATE);
            lobbyName.ActivateInputField();
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
                yield return new WaitForSeconds(1f);
            }
        }
        public void SetLobbyResponse(JSONObject lobbyData)
        {
            lobbyCollection.ForEach(lob => {
                Destroy(lob.lobbyDataGO);
            });
            lobbyCollection.Clear();
            Vector3 pos = lobbyDataDisplay.transform.position;
            // pos.y -= 50;
            lobbyData.keys.ForEach((k) => {
                // Debug.Log("lobby response " + lobbyData[k]);
                JSONObject data = lobbyData[k];
                string gameMode = data["settings"]["gameMode"].str;
                string name = data["settings"]["name"].str;
                int playerCount = (int)data["playerCount"].n;
                int maxPlayers = (int)data["settings"]["maxPlayers"].n;
                string parsedString = gameMode + " " + name + "     [" + playerCount + " / " + maxPlayers + "]";
                LobbyInfo lobby = Instantiate(lobbyDataDisplay).GetComponent<LobbyDataController>().SetLobbyData(k, parsedString);
                lobby.lobbyDataGO.transform.position = pos;
                lobby.lobbyDataGO.transform.SetParent(lobbyWindow, false);
                lobbyCollection.Add(lobby);
                pos.y -= 55;
            });
        }

        public void SetBrowserMode(BrowserMode mode)
        {
            browserMode = mode;
            browseCanvas.SetActive(browserMode == BrowserMode.BROWSE);
            createCanvas.SetActive(browserMode == BrowserMode.CREATE);
            lobbyName.ActivateInputField();
        }
    }
}
