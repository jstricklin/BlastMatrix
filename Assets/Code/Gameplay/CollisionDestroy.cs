﻿using System.Collections;
using System.Collections.Generic;
using Project.Interfaces;
using Project.Networking;
using UnityEngine;

namespace Project.Gameplay {
    public class CollisionDestroy : MonoBehaviour
    {
        NetworkIdentity networkIdentity => GetComponent<NetworkIdentity>();
        IActivate activator => GetComponent<IActivate>();
        void OnCollisionEnter(Collision coll)
        {
            // GetComponent<Projectile>()?.UpdateProjectile();
            NetworkIdentity ni = coll.transform.GetComponent<NetworkIdentity>();
            if (ni == null || ni.GetID() != activator?.GetActivator())
            {
                // Debug.Log("projectile network identity" + networkIdentity.GetID() + " and " + GetComponent<NetworkTransform>().player.id);
                // gameObject.SetActive(false);
                networkIdentity.GetSocket().Emit("collisionDestroy", JsonUtility.ToJson(new IDData()
                {
                    id = networkIdentity.GetID(),
                }));
            }
        }
    }
}
