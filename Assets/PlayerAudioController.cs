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
        AudioSource engine, cannon, weaponBase, barrelBase;
        float startPitch, pitch, turnPitch = 1.15f, movePitch = 1.5f;
        
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
            } else if (tank.moveDir == PlayerController.MoveDir.IDLE && tank.turnDir == PlayerController.TurnDir.IDLE && isMoving) {
                isMoving = false;
            }
        }

        public void CannonTurn(bool play)
        {
            if (play)
            {
                weaponBase.Play();
            } else {
                weaponBase.Stop();
            }
        }

        public void CannonAim(bool play)
        {
            if (play)
            {
                barrelBase.Play();
            } else {
                barrelBase.Stop();
            }
        }

        public IEnumerator EngineMoveAudio()
        {
            while(true)
            {
                if (tank.moveDir == PlayerController.MoveDir.IDLE) 
                {
                    pitch = startPitch + turnPitch;
                } else {
                    pitch = startPitch + movePitch;
                }
                if (engine.pitch != pitch && isMoving)
                {
                    engine.pitch = Mathf.Lerp(engine.pitch, pitch, (pitch / engine.pitch) * Time.deltaTime * 3);
                } else if (!isMoving && engine.pitch > startPitch) {
                    engine.pitch = Mathf.Lerp(engine.pitch, startPitch,  (startPitch / engine.pitch) * Time.deltaTime * 3);
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
