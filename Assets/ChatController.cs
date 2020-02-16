using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Project.Utilities;
using System;
using Project.Networking;
using UnityEngine.UI;
using Project.Controllers;

namespace Project.Controllers {
    public class ChatController : Singleton<ChatController>
    {
        [SerializeField]
        TMP_InputField chatInput;
        [SerializeField]
        TMP_Text chatText;
        [SerializeField]
        ScrollRect scrollView;
        Chat chat;
        float maxDisplayTime = 5f;
        float lastMessageTime = 0f;
        NetworkClient networkClient;
        bool chatActive = false;
        List<Message> messages = new List<Message>();
        InputController inputController;
        
        void Start()
        {
            // chatText.enabled = false;
            // chatBG = scrollView.GetComponent<Image>();
            // chatBGColor = chatBG.color;
            networkClient = FindObjectOfType<NetworkClient>();
            inputController = FindObjectOfType<InputController>();
            inputController.chat.performed += ToggleChat;
            chat = new Chat(chatText, maxDisplayTime, chatInput, chatView: scrollView);
            StartCoroutine(chat.ChatSystem());
        }

        void OnDisable()
        {
            inputController.chat.performed -= ToggleChat;
        }

        private void ToggleChat(InputAction.CallbackContext input)
        {
            chat.DisplayChatOrSendMessage();
        }

        public void OnMessageReceived(Message message) 
        {
            // chatText.text += "\n"+message;
            // Debug.Log("message update " + message.message);
            messages.Add(message);
            string text = chat.chatText.text;
            text += "\n" + message.username +  ": " + message.message;
            chat.UpdateChat(text);
        }

        public void SetWelcomeMessage(Message message) 
        {
            // chatText.text += "\n"+message;
            // messages.Add(message);
            // string text = chat.chatText.text;
            // text += "\n" + message.username +  ": " + message.message;
            // text += "\n" + message.username +  ": " + message.message;
            string text = message.message + "\n";
            chat.UpdateChat(text);
        }

        public void ClearChat()
        {
            messages.Clear();
            chat.chatText.text = "";
        }

    }
}

[Serializable]
public class Chat : MonoBehaviour {
    public bool chatActive = false;
    public TMP_InputField chatInput;
    public TMP_Text chatText;
    NetworkClient networkClient;
    // public string chat = "";
    ScrollRect chatView;
    Image chatBG;
    Color chatBGColor;
    public float lastMessageTime;
    float maxDisplayTime;

    public Chat(TMP_Text textObj, float displayTime, TMP_InputField input = null, ScrollRect chatView = null) {
        networkClient = FindObjectOfType<NetworkClient>();
        this.chatText = textObj;
        this.maxDisplayTime = displayTime;
        this.chatInput = input;
        this.chatInput?.gameObject.SetActive(false);
        if (chatView != null)
        {
            this.chatView = chatView;
            this.chatBG = this.chatView.GetComponent<Image>();
            this.chatBGColor = this.chatBG.color;
        }
    }
    public void UpdateChat(string message) 
    {
        chatText.text = message;
        lastMessageTime = Time.time;
        if (chatView != null)
        {
            chatView.verticalNormalizedPosition = 0.5f;
        }
    }
    public void DisplayChatOrSendMessage()
    {
        chatActive = !chatActive;
        if (chatInput.isActiveAndEnabled && chatInput.text.Length > 1)
        {
            networkClient.SendChatMessage(chatInput.text);
            chatInput.text = "";
        }
        InputController.Instance.TogglePlayerControls(!chatActive);
        // chatText.enabled = chatActive;
        chatInput.gameObject.SetActive(chatActive);
        chatInput.ActivateInputField();
    }
   public IEnumerator ChatSystem()
    {
        // Debug.Log(chatText + " chat text");
        Color col = chatText.color;
        Color colClear = col;
        Color bgClear = chatBGColor;
        bgClear.a = 0;
        colClear.a = 0;
        if (chatBG != null) chatBG.color = bgClear;
        while (true) 
        {
            if (Time.time - lastMessageTime > maxDisplayTime && !chatActive) 
            {
                chatText.color = Color.Lerp(chatText.color, colClear, Time.deltaTime * 2);
                if (chatBG != null) 
                {
                    chatBG.color = Color.Lerp(chatBG.color, bgClear, Time.deltaTime * 2);
                }
                
            } else {
                chatText.color = col;
                if (chatBG != null) 
                {
                    chatBG.color = chatBGColor;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}