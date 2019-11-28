﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities;

namespace Project.Managers
{
    public class ApplicationManager : MonoBehaviour
    {

        public void Start()
        {
            SceneManagementManager.Instance.LoadLevel(levelName: SceneList.MAIN_MENU, onLevelLoaded: (levelName) => { });
        }

    }
}