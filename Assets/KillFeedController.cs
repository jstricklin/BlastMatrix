using System;
using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

namespace Project.Controllers {
    public class KillFeedController : MonoBehaviour
    {

        KillFeed killFeed;
        [SerializeField]
        TMP_Text killFeedText;
        [SerializeField]
        ScrollRect feedView;
        [SerializeField]
        List<Message> killFeedMessages = new List<Message>();
        [SerializeField]
        bool testFeed = false;

        [SerializeField]
        float messageDisplayTime = 3f;
        [SerializeField]
        int messagesToDisplay = 3;

        void Start()
        {
            killFeed = new KillFeed(killFeedText, messageDisplayTime, messagesToDisplay, feedView);
            StartCoroutine(killFeed.KillFeedSystem());
            if (testFeed)
            {
                StartCoroutine(TestFeed());
            }
        }
        public void OnPlayerKilled(string killer, string player)
        {
            
            Message feedMessage = killFeed.GenerateKillMessage(killer, player);
            killFeedMessages.Add(feedMessage);
            OnUpdateKillFeed(feedMessage);
        }
        public void OnUpdateKillFeed(Message message)
        {
            killFeed.UpdateKillFeed(message);
        }
        public void SetMessageDisplayTime()
        {

        }
        IEnumerator TestFeed()
        {
            int i = 0;
            while (true)
            {
                string a = $"player{i+1}";
                string b = $"player{i+2}";
                OnPlayerKilled(a, b);
                i+=2;
                yield return new WaitForSeconds(2f);
            }
        }
    }
    [Serializable]
    public class KillFeed : MonoBehaviour {
    public bool killFeedActive = false;
    public TMP_Text feedText;
    NetworkClient networkClient;
    ScrollRect feedView;
    Image feedBG;
    Color feedBGColor;
    public float lastMessageTime;
    [SerializeField]
    float maxDisplayTime;
    [SerializeField]
    int messagesToDisplay = 3;
    List<Message> feedMessages = new List<Message>();

    string[] killActions = {
        "destroyed",
        "blasted",
        "wrecked",
        "obliterated",
        "disintegrated",
        "disincorporated",
        "atomized",
        "dusted",
        "explosively distributed",
        "popped",
        "finished",
        "got one over",
        "exploded",
        "nullified",
        "discomposed",
        "extirpated",
        "ruined",
        "outplayed",
        "exterminated",
        "demolished",
        "eradicated",
        "smashed",
        "annihilated",
        "floored"
    };

    public Message GenerateKillMessage(string killer, string player)
    {
        Message feedMessage = new Message();
        feedMessage.date = Time.time;
        feedMessage.message = $"{killer} {killActions[Random.Range(0, killActions.Length)]} {player}";

        return feedMessage;
    }

    public KillFeed(TMP_Text textObj, float displayTime, int messagesToDisplay, ScrollRect view = null) {
        networkClient = FindObjectOfType<NetworkClient>();
        this.feedText = textObj;
        this.feedText.text = "";
        this.maxDisplayTime = displayTime;
        this.messagesToDisplay = messagesToDisplay;
        if (view != null)
        {
            this.feedView = view;
            this.feedBG = this.feedView.GetComponent<Image>();
            this.feedBGColor = this.feedBG.color;
        }
    }
    public void UpdateKillFeed(Message message) 
    {
        feedMessages.Add(message);
        lastMessageTime = Time.time;
        if (feedView != null)
        {
            // feedView.verticalNormalizedPosition = 0f;
            // chatView.vertical;
        }
    }
        public IEnumerator KillFeedSystem()
        {
            // Debug.Log(chatText + " chat text");
            Color col = feedText.color;
            Color colClear = col;
            Color bgClear = feedBGColor;
            bgClear.a = 0;
            colClear.a = 0;
            if (feedBG != null) feedBG.color = bgClear;
            while (true) 
            {
                if (Time.time - lastMessageTime > maxDisplayTime && !killFeedActive) 
                {
                    feedText.color = Color.Lerp(feedText.color, colClear, Time.deltaTime * 2);
                    if (feedBG != null) 
                    {
                        feedBG.color = Color.Lerp(feedBG.color, bgClear, Time.deltaTime * 2);
                    }
                    
                } else {
                    feedText.color = col;
                    if (feedBG != null) 
                    {
                        feedBG.color = feedBGColor;
                    }
                }
                feedText.text = "";
                for (int i = 0; i < feedMessages.Count; i++)
                {
                    if (i >= messagesToDisplay) break;
                    Message message = feedMessages[(feedMessages.Count - 1) - i];
                    if (Time.time - message.date <= maxDisplayTime)
                    {
                        feedText.text = message.message + "\n" + feedText.text;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}