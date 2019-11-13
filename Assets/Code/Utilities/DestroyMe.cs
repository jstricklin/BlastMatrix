using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities {
    public class DestroyMe : MonoBehaviour
    {
        [SerializeField]
        float lifeSpan;
        void Start()
        {
            Invoke("DestroyGO", lifeSpan);
        }

        void DestroyGO()
        {
            Destroy(gameObject);
        }

    }
}
