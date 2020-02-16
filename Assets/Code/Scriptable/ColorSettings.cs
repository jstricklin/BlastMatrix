using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "ColorSettings", menuName = "ScriptableObjects/ColorSettings", order = 5)]
    public class ColorSettings : ScriptableObject
    {
        public Color[] primaryColors;
        public Color[] secondaryColors;

        public struct SelectedColors {
            public int primary;
            public int secondary;
        }
    }
}