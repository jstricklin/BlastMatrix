using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

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
        AudioMixer bgmMixer, sfxMixer;
        List<AudioMixer> mixers = new List<AudioMixer>();
        

        // public void SetBGM(string track)
        // {
        //     bgm.clip 
        // }

        void Start()
        {
            mixers.Add(bgmMixer);
            mixers.Add(sfxMixer);
        }

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

        public void HandleAudio(bool enable)
        {
            AudioMixerSnapshot snapshot;
            foreach(AudioMixer mixer in mixers)
            {
                if (enable)
                {
                    snapshot = bgmMixer.FindSnapshot("Default");
                } else {
                    snapshot = bgmMixer.FindSnapshot("Disabled");
                }
                snapshot.TransitionTo(0.01f);
            }
        }

        public void StopBGM()
        {
            bgm.Stop();
        }
    }

}
