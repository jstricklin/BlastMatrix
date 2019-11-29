using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

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

        public TextMeshProUGUI playerLabel;
        public void SetHealth(float currentHealth) {
            Color newColor;
            Debug.Log("current health: " + currentHealth);
            foreach (var healthBar in healthBars)
            {
                float lerpVal;
                healthBar.value = Mathf.Ceil(currentHealth / healthBar.maxValue);
                if (currentHealth == 0) {
                    lerpVal = 0;
                } else {
                    lerpVal = healthBar.value / healthBar.maxValue;
                    Debug.Log("lerp val " + lerpVal);
                }
                newColor = Color.Lerp(lowHealthColor, fullHealthColor, lerpVal);
                healthBar.fillRect.GetComponent<Image>().color = newColor;
                healthBarCenter.color = newColor;
            }
        }
    }
}