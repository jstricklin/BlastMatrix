using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utilities;
using UnitySocketIO;
using UnitySocketIO.SocketIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Project.Managers
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Button queueButton;
        [SerializeField]
        private Button enterUsernameButton;
        [SerializeField]
        private Button customizeTankButton;
        [SerializeField]
        private Button browseLobbiesButton;
        [SerializeField]
        private Button createLobbyButton;
        [SerializeField]
        GameObject startMenu;
        [SerializeField]
        GameObject mainMenu;
        [SerializeField]
        GameObject loginMenu;
        [SerializeField]
        TMP_InputField usernameField;
        [SerializeField]
        TMP_Text invalidUsername;

        private BaseSocketIO socketReference;
        private BaseSocketIO SocketReference
        {
            get
            {
                return socketReference = (socketReference == null) ? FindObjectOfType<SocketIOController>().socketIO : socketReference;
            }
        }

        // Start is called before the first frame update
        void Start()
        {


            queueButton.interactable = false;
            
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.ONLINE, onLevelLoaded: (levelName) =>
            {
                queueButton.interactable = true;
            });
        }
        public void Enqueue()
        {
            // lazy loading
            SocketReference.Emit("joinGame");
        }

        public void DisplayLoginScreen()
        {
            startMenu.SetActive(false);
            loginMenu.SetActive(true);
            usernameField.ActivateInputField();
        }
        public void DisplayMainMenu()
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        public void EnterUsername()
        {

            if (usernameField.text.ValidUsername()) 
            {
                FindObjectOfType<NetworkClient>().RegisterUsername(usernameField.text);
                // Enqueue();
                DisplayMainMenu();
            } else {
                Color col = invalidUsername.color;
                col.a = 255;
                invalidUsername.color = col;
            }
        }
    }
}
