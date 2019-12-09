using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utilities;
using UnitySocketIO;
using UnitySocketIO.SocketIO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Controllers;

// should be in controllers

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
            if (FindObjectOfType<NetworkClient>()) {

                startMenu.SetActive(false);
                DisplayMainMenu();
                return;
            }
            LoginButtonEnabled(false);
        }
        public void Enqueue()
        {
            // lazy loading
            SocketReference.Emit("quickPlay");
        }

        public void DisplayLoginScreen()
        {
            startMenu.SetActive(false);
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.ONLINE, onLevelLoaded: (levelName) =>
            {
                StartCoroutine(ValidateConnection());
            });
            loginMenu.SetActive(true);
            // Invoke("LoginButtonEnabled", 2.5f);
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
                FindObjectOfType<NetworkClient>().LoginUser(this, usernameField.text);
                // Enqueue();
                LoginButtonEnabled(false);
                StartCoroutine(ValidateConnection());
            } else {
                Color col = invalidUsername.color;
                col.a = 255;
                invalidUsername.color = col;
            }
        }
        public void BrowseLobbies()
        {
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.LOBBY_BROWSER, onLevelLoaded: (levelName) => {
                FindObjectOfType<LobbyBrowserController>().SetBrowserMode(LobbyBrowserController.BrowserMode.BROWSE);
                SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
            });
        }
        public void CreateLobby()
        {
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.LOBBY_BROWSER, onLevelLoaded: (levelName) => {
                FindObjectOfType<LobbyBrowserController>().SetBrowserMode(LobbyBrowserController.BrowserMode.CREATE);
                SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
            });
        }
        IEnumerator ValidateConnection()
        {
            yield return new WaitForSeconds(0.5f);
            while(true) 
            {
                if (LoginButtonEnabled(NetworkClient.responseCode == 200)) {
                    // DisplayMainMenu();
                    yield break;
                }
                yield return new WaitForSeconds(0.25f);
            }
        }
        bool LoginButtonEnabled(bool enabled)
        {
            enterUsernameButton.interactable = enabled;
            return enabled;
        }
    }
}
