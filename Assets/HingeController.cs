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
        [SerializeField]
        bool unparentHinge = false;

        Rigidbody hingeRb;

        //TODO work on hinge parent logic to smooth and fix bugs
        // Start is called before the first frame update
        void Awake()
        {
            if (enableHinge)
            {
                hingeRb = hinge.GetComponent<Rigidbody>();
                if (unparentHinge)
                    hinge.transform?.SetParent(null);
            } else {
                Destroy(hingeBase.GetComponent<Rigidbody>());
                Destroy(hinge);
                Destroy(hinge.GetComponent<Rigidbody>());
            }
        }
        public void DisableHinge()
        {
            hinge.gameObject.SetActive(false);
        }
        public void ResetHinge()
        {
            hinge.gameObject.SetActive(true);
            // hinge.transform.position = hingeBase.position;
            // hinge.transform.rotation = hingeBase.rotation;
        }

        public void CannonFired(Vector3 direction)
        {
            hingeRb?.AddForce(-direction * 15, ForceMode.Impulse);
        }

    }
}
