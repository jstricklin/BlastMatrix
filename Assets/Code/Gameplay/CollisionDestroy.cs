using System.Collections;
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
            NetworkIdentity ni = coll.transform.GetComponent<NetworkIdentity>();
            if (ni == null || ni.GetID() != activator?.GetActivator())
            {
                networkIdentity.GetSocket().Emit("collisionDestroy", JsonUtility.ToJson(new IDData()
                {
                    id = networkIdentity.GetID(),
                }));
            }
        }
    }
}
