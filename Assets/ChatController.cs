using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Project.Utilities;
using System;
using Project.Networking;

namespace Project.Controllers {
    public class ChatController : Singleton<ChatController>
    {
        [SerializeField]
        TMP_InputField chatInput;
        [SerializeField]
        TMP_Text chatText;
        [SerializeField]
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
            networkClient = FindObjectOfType<NetworkClient>();
            inputController = FindObjectOfType<InputController>();
            inputController.chat.performed += DisplayChatOrSendMessage;
            chat = new Chat(chatText, maxDisplayTime, chatInput);
            StartCoroutine(chat.ChatSystem());
        }

        void OnDisable()
        {
            inputController.chat.performed -= DisplayChatOrSendMessage;
        }

        private void DisplayChatOrSendMessage(InputAction.CallbackContext input)
        {
            chat.chatActive = !chat.chatActive;
            if (chat.chatInput.isActiveAndEnabled && chat.chatInput.text.Length > 1)
            {
                networkClient.SendChatMessage(chat.chatInput.text);
                chat.chatInput.text = "";
            }
            inputController.TogglePlayerControls(!chat.chatActive);
            // chatText.enabled = chatActive;
            chat.chatInput.gameObject.SetActive(chat.chatActive);
            chat.chatInput.ActivateInputField();
        }

        public void OnMessageReceived(Message message) 
        {
            // chatText.text += "\n"+message;
            messages.Add(message);
            string text = chat.chatText.text;
            text += "\n" + message.username +  ": " + message.message;
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
    // public string chat = "";
    public float lastMessageTime;
    float maxDisplayTime;

    public Chat(TMP_Text textObj, float displayTime, TMP_InputField input = null) {
        this.chatText = textObj;
        this.maxDisplayTime = displayTime;
        this.chatInput = input;
        chatInput?.gameObject.SetActive(false);
        // StartCoroutine(this.ChatSystem());
    }
    public void UpdateChat(string message) 
    {
        chatText.text = message;
        lastMessageTime = Time.time;
    }
    public IEnumerator ChatSystem()
    {
        Debug.Log(chatText + " chat text");
        Color col = chatText.color;
        Color colClear = col;
        colClear.a = 0;
        while (true) 
        {
            if (Time.time - lastMessageTime > maxDisplayTime && !chatActive) 
            {
                // chatText.enabled = false;
                chatText.color = Color.Lerp(chatText.color, colClear, Time.deltaTime * 2);
            } else {
                // chatText.enabled = true;
                chatText.color = col;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}