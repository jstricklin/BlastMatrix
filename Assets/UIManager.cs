using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using System;

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
        TMP_Text gameClock;
        [SerializeField]
        TMP_Text playerScore;
        [SerializeField]
        TMP_Text winPosition;
        
        [SerializeField]
        RawImage hitMarker;
        Animator myAnim;

        public TextMeshProUGUI playerLabel;

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
                    Debug.Log("lerp val " + lerpVal);
                }
                newColor = Color.Lerp(lowHealthColor, fullHealthColor, lerpVal);
                healthBar.fillRect.GetComponent<Image>().color = newColor;
                healthBarCenter.color = newColor;
            }
        }

        public void DisplayHitMarker(int score)
        {
            Debug.Log("hit score: " + score);
            myAnim.SetTrigger("hit");
        }
        public void SetScore(int score)
        {
            playerScore.text = score.ToString();
        }
        public void UpdateGameClock(float gameTime)
        {
            gameClock.text = gameTime.ToString();
        }
    }
}