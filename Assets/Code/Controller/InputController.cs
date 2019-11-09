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
        public InputAction aimLeft;
        public InputAction aimRight;
        public InputAction aimUp;
        public InputAction aimDown;
        public InputAction fire;
        public override void OnEnable()
        {
            EnableInputs();            
        }

        void OnDisable()
        {
            DisableInputs();
        }

        private void EnableInputs()
        {
            moveForward.Enable();
            moveBackwards.Enable();
            turnLeft.Enable();
            turnRight.Enable();
            aimRight.Enable();
            aimLeft.Enable();
            aimUp.Enable();
            aimDown.Enable();
            fire.Enable();
        }

        private void DisableInputs()
        {
            moveForward.Disable();
            moveBackwards.Disable();
            turnLeft.Disable();
            turnRight.Disable();
            aimRight.Disable();
            aimLeft.Disable();
            aimUp.Disable();
            aimDown.Disable();
            fire.Disable();
        }
    }

}