using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Project.Utilities;
using UnityEngine.Audio;

namespace Project.Controllers {
    public class PlayerAudioController : MonoBehaviour
    {
        PlayerController tank;
        [TabGroup("Tank Audio Sources")]
        [SerializeField]
        AudioSource engine, cannon, weaponBase;
        float startPitch, pitchDiff = 1.5f;
        
        bool isMoving = false;

        void Start()
        {
            tank = GetComponentInParent<PlayerController>();
            startPitch = engine.pitch;
            StartCoroutine(EngineMoveAudio());
            
        }
        void Update()
        {
            CheckTankEngine();
        }

        public void FireCannon()
        {
            cannon.Play();
        }

        void CheckTankEngine()
        {
            if ((tank.moveDir != PlayerController.MoveDir.IDLE || tank.turnDir != PlayerController.TurnDir.IDLE) && !isMoving)
            {
                isMoving = true;
                // StopAllCoroutines();
            } else if (tank.moveDir == PlayerController.MoveDir.IDLE && tank.turnDir == PlayerController.TurnDir.IDLE && isMoving) {
                isMoving = false;
            }
        }

        public IEnumerator EngineMoveAudio()
        {
            while(true)
            {
                if (engine.pitch < startPitch * pitchDiff && isMoving)
                {
                    engine.pitch = Mathf.Lerp(engine.pitch, startPitch + pitchDiff, ((startPitch * pitchDiff) / engine.pitch) * Time.deltaTime * 3);
                } else if (!isMoving && engine.pitch > startPitch) {
                    engine.pitch = Mathf.Lerp(engine.pitch, startPitch,  ((startPitch) / engine.pitch) * Time.deltaTime * 3);
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

}
