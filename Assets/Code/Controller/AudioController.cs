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
        [SerializeField]
        AudioMixer masterMixer;
        public bool bgmOn = true, sfxOn = true;
        

        void Start()
        {
            HandleBGM(bgmOn);
            HandleSFX(sfxOn);
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

        public void HandleBGM(bool enable)
        {
            bgmOn = enable;
            AudioMixerSnapshot snapshot;
            if (enable && sfxOn)
            {
                snapshot = masterMixer.FindSnapshot("Default");
            } else if (enable && !sfxOn){
                snapshot = masterMixer.FindSnapshot("Disable SFX");
            } else if (!enable && sfxOn){
                snapshot = masterMixer.FindSnapshot("Disable BGM");
            } else {
                snapshot = masterMixer.FindSnapshot("Disable Audio");
            }
            snapshot.TransitionTo(0);
        }
        public void HandleSFX(bool enable)
        {
            sfxOn = enable;
            AudioMixerSnapshot snapshot;
            if (enable && bgmOn)
            {
                snapshot = masterMixer.FindSnapshot("Default");
            } else if (enable && !bgmOn){
                snapshot = masterMixer.FindSnapshot("Disable BGM");
            } else if (!enable && bgmOn){
                snapshot = masterMixer.FindSnapshot("Disable SFX");
            } else {
                snapshot = masterMixer.FindSnapshot("Disable Audio");
            }
            snapshot.TransitionTo(0);
        }

        public void StopBGM()
        {
            bgm.Stop();
        }
    }

}
