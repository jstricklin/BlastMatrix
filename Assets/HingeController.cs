using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Controllers {
    public class HingeController : MonoBehaviour
    {
        [SerializeField]
        Transform hingeBase;
        [SerializeField]
        Joint hinge;
        [SerializeField]
        bool enableHinge;

        Rigidbody hingeRb;
        // Start is called before the first frame update
        void Awake()
        {
            if (enableHinge)
            {
                // hinge.transform?.SetParent(null);
                hingeRb = hinge.GetComponent<Rigidbody>();
            } else {
                Destroy(hingeBase.GetComponent<Rigidbody>());
                Destroy(hinge);
                Destroy(hinge.GetComponent<Rigidbody>());
            }
        }

        public void CannonFired(Vector3 direction)
        {
            hingeRb?.AddForce(-direction * 15, ForceMode.Impulse);
        }

    }
}
