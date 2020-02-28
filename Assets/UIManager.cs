using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using System;
using Project.Controllers;
using UnityEngine.InputSystem;

namespace Project.UI {
    public class UIManager : Singleton<UIManager>
    {

        [SerializeField]
        Canvas MainUI;
        [SerializeField]
        Slider[] healthBars;
        [SerializeField]
        Image healthBarCenter;
        [SerializeField]
        Color fullHealthColor, lowHealthColor;

        [SerializeField]
        public KillFeedController mainKillFeed, personalKillFeed, killedByFeed;

        [SerializeField]
        TMP_Text gameClock;
        [SerializeField]
        TMP_Text playerScore;
        [SerializeField]
        TMP_Text matchScores;
        [SerializeField]
        TMP_Text winPosition;
        
        [SerializeField]
        RawImage hitMarker;
        [SerializeField]
        GameObject pauseMenu;
        Animator myAnim;
        public TextMeshProUGUI playerLabel;
        InputController inputController;

        public override void OnEnable()
        {
            base.OnEnable();
            inputController = FindObjectOfType<InputController>();
            if (inputController != null)
            {
                inputController.pause.performed += TogglePauseMenu;
            }
        }
        void OnDisable()
        {
            inputController.pause.performed -= TogglePauseMenu;
        }
        void Start()
        {
            myAnim = GetComponent<Animator>();
        }
        public void SetHealth(float currentHealth) {
            Color newColor;
            foreach (var healthBar in healthBars)
            {
                float lerpVal;
                healthBar.value = Mathf.Ceil(currentHealth / healthBar.maxValue);
                if (currentHealth <= 0 || currentHealth == 100) {
                    lerpVal = currentHealth == 0 ? 0 : 1;
                } else {
                    lerpVal = healthBar.value / healthBar.maxValue;
                    // Debug.Log("lerp val " + lerpVal);
                }
                newColor = Color.Lerp(lowHealthColor, fullHealthColor, lerpVal);
                healthBar.fillRect.GetComponent<Image>().color = newColor;
                healthBarCenter.color = newColor;
            }
        }

        public void DisplayHitMarker(int score)
        {
            // Debug.Log("hit score: " + score);
            if (myAnim == null) return;
            myAnim.SetTrigger("hit");
        }
        public void UpdateScore(int score)
        {
            playerScore.text = score.ToString();
        }
        public void UpdateMatchScores(string scores)
        {
            matchScores.text = scores.FixLineBreaks();
        }
        public void UpdateGameClock(float gameTime)
        {
            gameClock.text = gameTime.ToString();
        }

        public void TogglePauseMenu(InputAction.CallbackContext obj)
        {
            TogglePauseMenu();
        }
        public void TogglePauseMenu()
        {
            pauseMenu?.SetActive(!pauseMenu.activeSelf);
        }

        public void ToggleBGM()
        {
            Debug.Log("toggling BGM");
        }
        public void ToggleSFX()
        {
            Debug.Log("toggling SFX");
        }
        public void TogglePostFX()
        {
            Debug.Log("toggling post FX");
        }
        public void OnPlayerKilled(string attacker, string player, bool killer = false, bool killed = false)
        {
            mainKillFeed.OnPlayerKilled(attacker, player);
            if (killer)
            {
                personalKillFeed.OnPlayerKilled("You", player);
            }  
            if (killed)
            {
                killedByFeed.OnPlayerKilled(attacker, "You");
            }
        }
    }
}