using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Utilities;

namespace Project.Controllers {
    public class InputController : Singleton<InputController>
    {
        public InputAction moveForward;
        public InputAction moveBackwards;
        public InputAction turnLeft;
        public InputAction turnRight;
        public InputAction fire;
        public override void OnEnable()
        {
            EnableInputs();            
        }

        private void EnableInputs()
        {
            moveForward.Enable();
            moveBackwards.Enable();
            turnLeft.Enable();
            turnRight.Enable();
            fire.Enable();
        }

        void OnDisable()
        {
            DisableInputs();
        }

        private void DisableInputs()
        {
            moveForward.Disable();
            moveBackwards.Enable();
            turnLeft.Enable();
            turnRight.Enable();
            fire.Enable();
        }
    }

}