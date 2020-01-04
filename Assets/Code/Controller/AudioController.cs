using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Project.Controllers 
{
    public class AudioController : Singleton<AudioController>
    {
        [SerializeField]
        AudioClip[] bgmClips;
        [SerializeField]
        AudioSource bgm;
        [SerializeField]
        AudioClip mainMenu;

        // public void SetBGM(string track)
        // {
        //     bgm.clip 
        // }
        public void StartBGM(string scene)
        {
            if (bgm.isPlaying) 
            {
                // return;
                StopBGM();
            }

            switch(scene)
            {
                case SceneList.LEVEL : bgm.clip = bgmClips[1];
                    break;
                default : bgm.clip = bgmClips[0];
                    break;
            }

            bgm.Play();
        }
        public void StopBGM()
        {
            bgm.Stop();
        }
    }

}
