using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using UnityEngine;

public class EndGameButton : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        FindObjectOfType<NetworkClient>().ExitToMainMenu();
    }

}
