using System.Collections;
using System.Collections.Generic;
using Project.Controllers;
using Project.Utilities;
using UnityEngine;

namespace Project.Managers {
    public class PostProcessManager : Singleton<PostProcessManager>
    {
        public bool postProcessEnabled = true;
        GameObject postProcessController;

        public void TogglePostProcesses()
        {
            postProcessEnabled = !postProcessEnabled;
            if  (postProcessController == null)
            {
                return;
            }
            postProcessController.SetActive(postProcessEnabled);
            // Debug.Log(FindObjectOfType<PostProcessController>());
        }

        public void HandlePostProcessController(GameObject controller)
        {
            postProcessController = controller;
            postProcessController.SetActive(postProcessEnabled);
        }

    }
}
