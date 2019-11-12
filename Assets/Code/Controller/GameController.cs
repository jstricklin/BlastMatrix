using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Controllers {
    public class GameController : MonoBehaviour
    {
        public GameObject player;
        private float timer;
        void OnEnable()
        {
        }

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
