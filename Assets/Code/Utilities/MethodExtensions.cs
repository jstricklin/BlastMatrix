using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Project.Utilities
{
    public static class MethodExtensions
    {
        public static bool ValidUsername(this string Value)
        {
            Regex test = new Regex(@"\W");
            return test.Matches(Value).Count == 0;
        }
        public static string FixLineBreaks(this string Value)
        {
            return Value.Replace("\\n", "\n");
        }
        public static string RemoveQuotes(this string Value)
        {
            return Value.Replace("\"", "");
        }
        public static float TwoDecimals(this float Value)
        {
            return Mathf.Round(Value * 1000.0f) / 1000.0f;
        }
        public static void FaceCamera(this Transform toFace, Camera mainCam)
        {
            toFace.transform.LookAt(toFace.position + mainCam.transform.rotation * Vector3.forward,
                mainCam.transform.rotation * Vector3.up);

        }
    }
}