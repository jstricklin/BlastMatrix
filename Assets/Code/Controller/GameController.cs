using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Controllers {
    public class GameController : Singleton<GameController>
    {
        public GameObject player;
        public GameObject UICanvas;
        private float timer;

        // Start is called before the first frame update
        void Start()
        {
            this.timer = 0.0f;           
        }

        // Update is called once per frame
        void Update()
        {
            this.timer = Time.deltaTime;
            while (this.timer >= Time.deltaTime)
            {
                this.timer -= Time.fixedDeltaTime;

                Physics.Simulate(Time.fixedDeltaTime);
            }
        }
    }
}
