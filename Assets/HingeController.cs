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
        // Start is called before the first frame update
        void Awake()
        {
            if (enableHinge)
            {
                // hinge.transform?.SetParent(null);
            } else {
                Destroy(hingeBase.GetComponent<Rigidbody>());
                Destroy(hinge);
                Destroy(hinge.GetComponent<Rigidbody>());
            }
        }
    }
}
