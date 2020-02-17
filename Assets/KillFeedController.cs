using System;
using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Controllers {
    public class KillFeedController : Singleton<KillFeedController>
    {
        KillFeed killFeed;
        [SerializeField]
        TMP_Text killFeedText;
        [SerializeField]
        ScrollRect feedView;
        [SerializeField]
        List<Message> killFeedMessages = new List<Message>();
        [SerializeField]
        bool testFeed = true;

        void Start()
        {
            killFeed = new KillFeed(killFeedText, 3f, feedView);
            StartCoroutine(killFeed.KillFeedSystem());
            if (testFeed)
            {
                StartCoroutine(TestFeed());
            }
        }
        public void OnPlayerKilled(string killer, string player)
        {
            
            Message feedMessage = new Message();
            feedMessage.message = $"{killer} destroyed {player}";
            killFeedMessages.Add(feedMessage);
            OnUpdateKillFeed(feedMessage);
        }
        public void OnUpdateKillFeed(Message message)
        {
            killFeed.UpdateKillFeed(message);
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
    float maxDisplayTime;
    int messagesToDisplay = 3;
    List<Message> feedMessages = new List<Message>();

    public KillFeed(TMP_Text textObj, float displayTime, ScrollRect view = null) {
        networkClient = FindObjectOfType<NetworkClient>();
        this.feedText = textObj;
        this.feedText.text = "";
        this.maxDisplayTime = displayTime;
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
        // feedText.text += message.message + "\n";
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