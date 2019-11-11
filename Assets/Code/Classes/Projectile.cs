using System.Collections;
using System.Collections.Generic;
using Project.Interfaces;
using Project.Networking;
using Project.Utility;
using UnityEngine;

namespace Project.Gameplay
{
    public class Projectile : MonoBehaviour, IActivate
    {
        Vector3 direction;
        float speed;
        ProjectileData projectile;
        NetworkIdentity networkIdentity;

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
            // Vector2 pos = direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;
            // transform.position += new Vector3(pos.x, pos.y, 0);
        }

        void FixedUpdate()
        {
            UpdateProjectile();   
        }

        public void UpdateProjectile()
        {
            if (NetworkClient.ClientID != activator)
            {
                return;
            }
            projectile.position.x = transform.position.x.TwoDecimals();
            projectile.position.y = transform.position.y.TwoDecimals();
            projectile.position.z = transform.position.z.TwoDecimals();

            // serialized player class makes it easy to convert to JSON
            networkIdentity.GetSocket().Emit("updateProjectile", JsonUtility.ToJson(projectile));
        }
        public void FireProjectile(float Speed, Vector3 Direction)
        {
            // Transform fired = Instantiate(projectile, projectileSpawnPoint.position, barrel.rotation);
            direction = Direction;
            speed = Speed;
            projectile = new ProjectileData();
            networkIdentity = GetComponent<NetworkIdentity>();
            projectile.id = networkIdentity.GetID();
            GetComponent<Rigidbody>().AddForce(Direction * Speed, ForceMode.Impulse);
        }
    }

}
