using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Controllers {
    public class CustomizeTankController : MonoBehaviour
    {
        Color primaryColor;
        Color secondaryColor;

        [SerializeField]
        ScriptableObject colorSettings;
        [SerializeField]
        GameObject bodyBase;
        [SerializeField]
        GameObject cannonBase;
        [SerializeField]
        GameObject barrelBase;

        [SerializeField]
        ScriptableObject tank;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        
    }
}
