using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using TMPro;
using UnityEngine;

public class LobbyDataController : MonoBehaviour
{
    public GameObject lobbyDataGO { get; internal set; }
    TMP_Text lobbyText;
    string lobbyId;

    [SerializeField]
    LobbyInfo lobbyInfo;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public LobbyInfo SetLobbyData(string id, string text)
    {
        gameObject.SetActive(true);
        // lobbyText.text = text;
        lobbyText = GetComponentInChildren<TMP_Text>();
        lobbyText.text = text;
        lobbyInfo = new LobbyInfo(gameObject, id);
        return lobbyInfo;
    }
    public void JoinLobby()
    {
        FindObjectOfType<NetworkClient>().JoinLobby(lobbyInfo);
    }
}
public class LobbyInfo {
    public GameObject lobbyDataGO;
    public TMP_Text lobbyText;
    public string lobbyId;

    public LobbyInfo(GameObject LobbyDataGO, string id) 
    {
        this.lobbyId = id;
        this.lobbyDataGO = LobbyDataGO;
    }
}
