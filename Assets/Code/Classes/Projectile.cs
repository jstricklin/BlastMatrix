using System.Collections;
using System.Collections.Generic;
using Project.Interfaces;
using Project.Networking;
using UnityEngine;

namespace Project.Gameplay
{
    public class Projectile : MonoBehaviour, IActivate
    {
        Vector2 direction;
        float speed;

        public string activator { get; set; }

        public Vector2 Direction
        {
            set
            {
                direction = value;
            }
        }

        public float Speed
        {
            set
            {
                speed = value;
            }
        }


        public string GetActivator()
        {
            return activator;
        }

        public void SetActivator(string ID)
        {
            activator = ID;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 pos = direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;
            transform.position += new Vector3(pos.x, pos.y, 0);
        }
        public void FireProjectile(float speed, Vector3 direction)
        {
            // Transform fired = Instantiate(projectile, projectileSpawnPoint.position, barrel.rotation);
            GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.Impulse);
        }
    }

}
