using System.Collections;
using System.Collections.Generic;
using Project.Controllers;
using Project.Utilities;
// using Project.Utility.Attributes;
using UnityEngine;

namespace Project.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkRotation : MonoBehaviour
    {
        [SerializeField]
        // [GreyOut]
        private WeaponRotation oldWeaponRotation;
        [SerializeField]
        // [GreyOut]
        private Quaternion oldRotation;
        private NetworkIdentity networkIdentity;
        [Header("Class References")]
        [SerializeField]
        private PlayerController playerController;
        private Player player;

        private float stillCounter = 0;

        public void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            // playerManager = GetComponent<PlayerManager>();
            oldWeaponRotation = playerController.GetLastWeaponRotation();
            player = new Player();
            player.rotation = new Rotation();
            // player.rotation.weaponRotation = 0;

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        public void Update()
        {
            if (networkIdentity.IsControlling())
            {
                if (oldWeaponRotation.lastWeaponRotation != playerController.GetLastWeaponRotation().lastWeaponRotation 
                || oldWeaponRotation.lastBarrelRotation != playerController.GetLastWeaponRotation().lastBarrelRotation
                || oldRotation != playerController.GetLastRotation())
                {
                    oldWeaponRotation = playerController.GetLastWeaponRotation();
                    oldRotation = playerController.GetLastRotation();
                    stillCounter = 0;
                    SendData();
                }
                else
                {
                    stillCounter += Time.deltaTime;
                    if (stillCounter >= 1)
                    {
                        stillCounter = 0;
                        SendData();
                    }
                }
            }

        }

        private void SendData()
        {
            // update player information
            player.rotation.weaponRotation.x = playerController.GetLastWeaponRotation().lastWeaponRotation.x.TwoDecimals();
            player.rotation.weaponRotation.y = playerController.GetLastWeaponRotation().lastWeaponRotation.y.TwoDecimals();
            player.rotation.weaponRotation.z = playerController.GetLastWeaponRotation().lastWeaponRotation.z.TwoDecimals();
            player.rotation.weaponRotation.w = playerController.GetLastWeaponRotation().lastWeaponRotation.w.TwoDecimals();
            player.rotation.barrelRotation.x = playerController.GetLastWeaponRotation().lastBarrelRotation.x.TwoDecimals();
            player.rotation.barrelRotation.y = playerController.GetLastWeaponRotation().lastBarrelRotation.y.TwoDecimals();
            player.rotation.barrelRotation.z = playerController.GetLastWeaponRotation().lastBarrelRotation.z.TwoDecimals();
            player.rotation.barrelRotation.w = playerController.GetLastWeaponRotation().lastBarrelRotation.w.TwoDecimals();
            player.rotation.rotation = playerController.GetLastRotation();

            // serialized player class makes it easy to convert to JSON
            networkIdentity.GetSocket().Emit("updateRotation", JsonUtility.ToJson(player));
        }
    }
}

