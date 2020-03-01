using System.Collections;
using System.Collections.Generic;
using Project.Managers;
using UnityEngine;

namespace Project.Controllers {
    public class PostProcessController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            PostProcessManager.Instance.HandlePostProcessController(gameObject);
        }

    }
}
