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
        [SerializeField]
        public TMP_Text loggedInText;

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


            // enterUsernameButton.interactable = false;
            
        }
        public void Enqueue()
        {
            // lazy loading
            SocketReference.Emit("joinGame");
        }

        public void DisplayLoginScreen()
        {
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.ONLINE, onLevelLoaded: (levelName) =>
            {
                // StartCoroutine(ValidateConnection());
            });
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
            if (!enterUsernameButton.interactable) return;
            if (usernameField.text.ValidUsername()) 
            {
                FindObjectOfType<NetworkClient>().LoginUser(usernameField.text);
                // Enqueue();
                enterUsernameButton.interactable = false;
                StartCoroutine(ValidateConnection());
            } else {
                Color col = invalidUsername.color;
                col.a = 255;
                invalidUsername.color = col;
            }
        }
        IEnumerator ValidateConnection()
        {
            while(true) 
            {
                enterUsernameButton.interactable = NetworkClient.ClientID != null;
                if (enterUsernameButton.interactable) {
                    DisplayMainMenu();
                    yield break;
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
