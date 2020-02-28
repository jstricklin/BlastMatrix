using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Controllers {
    public class ToggleButtonController : MonoBehaviour
    {
        TextMeshProUGUI onText;
        TextMeshProUGUI offText;
        
        Color onColor, offColor;
        bool isOn = true;

        // Start is called before the first frame update
        void Start()
        {
            onText = transform.Find("On Text").GetComponent<TextMeshProUGUI>();
            offText = transform.Find("Off Text").GetComponent<TextMeshProUGUI>();
            onColor = onText.color;
            offColor = offText.color;
        }
        public void ToggleText()
        {
            isOn = !isOn;
            onText.color = isOn ? onColor : offColor;
            offText.color = !isOn ? onColor : offColor;
        }
    }
}
