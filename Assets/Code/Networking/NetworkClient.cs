using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;
using System;

namespace Project.Networking 
{
    public class NetworkClient : SocketIOController
    {
        // Start is called before the first frame update
        void Start()
        {
            SetupEvents();
            socketIO.Connect();
        }

        private void SetupEvents()
        {
            socketIO.On("open", (e) => {
                Debug.Log("Connection to server established");
            });
            socketIO.On("register", (e) => {
                var id = new JSONObject(e.data)["id"].str;
                Debug.Log($"Client Registered with Server - Client ID: {id}");
            });
            socketIO.On("disconnected", (e) => {
                var id = new JSONObject(e.data)["id"].str;
                Debug.Log($"Player has disconnected: {id}");
            });
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}