﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.ScriptableObjects {
    [CreateAssetMenu(fileName = "ColorSettings", menuName = "ScriptableObjects/ColorSettings", order = 5)]
    public class ColorSettings : ScriptableObject
    {
        public Color[] primaryColors;
        public Color[] secondaryColors;

        [SerializeField]
        GameObject colorThumb;

        public struct SelectedColors {
            public int primary;
            public int secondary;
        }

        public List<GameObject> GenerateColorThumbs(Transform colorContainer, Vector3 startPos, int width)
        {
            float containerWidth = colorContainer.GetComponent<RectTransform>().rect.width;


            int xOffset = 50;
            int yOffset = -25;
            List<GameObject> colorThumbs = new List<GameObject>();
            int i = 0;
            int xStep = 0;
            Vector3 pos = startPos;
            //TODO work on dynamic size color
            int maxX = (int)Mathf.Floor(containerWidth / xOffset + colorThumb.GetComponent<Image>().rectTransform.rect.width); 
            foreach(Color col in primaryColors)
            {
                GameObject _colorThumb = Instantiate(colorThumb);
                _colorThumb.transform.SetParent(colorContainer);
                _colorThumb.GetComponent<Image>().color = col;
                if (i > 0 && xStep < width) {
                    pos.x += xOffset;
                } else if (i > 0 && xStep >= width) {
                    pos.x = startPos.x;
                    pos.y += yOffset; 
                    xStep = 0;
                }
                _colorThumb.transform.position = pos;
                _colorThumb.transform.localScale = new Vector3(1, 1, 1);
                colorThumbs.Add(_colorThumb);
                xStep++;
                i++;
            }
            return colorThumbs;
        }
    }
}