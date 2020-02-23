using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Controllers {
    public class CustomizeTankController : MonoBehaviour
    {
        Color primaryColor;
        Color secondaryColor;

        [SerializeField]
        ColorSettings colorSettings;

        [SerializeField]
        Transform tankSpawnPoint;

        [SerializeField]
        Tank tank;
        [SerializeField]
        Transform tankDummy;
        [SerializeField]
        Transform colorContainer;
        [SerializeField]
        Transform colorObj;
        [SerializeField]
        List<GameObject> colorThumbs = new List<GameObject>();
        Color selectedColor;
        Dictionary<string, GameObject> tankParts;

        PlayerTank playerTank;

        NetworkClient networkClient;

        // Start is called before the first frame update
        void Start()
        {
            networkClient = FindObjectOfType<NetworkClient>();
            playerTank = NetworkClient.ClientID != null ? networkClient.playerTank : new PlayerTank();
            tankParts = tank.GenerateTankParts(tankDummy);
            tank.SetTankColor(tankParts, playerTank.primaryColor);
            colorThumbs = colorSettings.GenerateColorThumbs(colorContainer, colorObj.position, 5);
        }

        // Update is called once per frame
        void Update()
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                foreach(GameObject colorThumb in colorThumbs)
                { 
                    if (colorThumb.GetComponent<BoxCollider2D>().bounds.Contains(Input.mousePosition))
                    {
                        selectedColor = colorThumb.GetComponent<Image>().color;
                        tank.SetTankColor(tankParts, selectedColor);
                        playerTank.primaryColor = selectedColor;
                    }
                }
            }
        }

        public void SaveAndReturnToMainMenu()
        {
            networkClient?.SaveTankConfig(playerTank);
            ReturnToMainMenu();
        }

        public void ReturnToMainMenu()
        {
            networkClient?.ReturnToMainMenu();
        }
    }
}
