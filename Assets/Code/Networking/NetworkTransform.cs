﻿using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utility;
// using Project.Utility.Attributes;
using UnityEngine;

namespace Project.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkTransform : MonoBehaviour
    {
        [SerializeField]
        // [GreyOut]
        private Vector3 oldPosition;
        private NetworkIdentity networkIdentity;
        public Player player;

        private float stillCounter = 0;

        public void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            oldPosition = transform.position;
            player = new Player();
            player.position = new Position();
            player.position.x = 0;
            player.position.y = 0;
            player.position.z = 0;

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        public void FixedUpdate()
        {
            if (networkIdentity.IsControlling())
            {
                if (oldPosition != transform.position)
                {
                    oldPosition = transform.position;
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

        public void SendData()
        {
            // update player information
            player.position.x = transform.position.x.TwoDecimals();
            player.position.y = transform.position.y.TwoDecimals();
            player.position.z = transform.position.z.TwoDecimals();

            // serialized player class makes it easy to convert to JSON
            networkIdentity.GetSocket().Emit("updatePosition", JsonUtility.ToJson(player));
        }
    }
}
