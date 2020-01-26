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
        public static bool TargetInSight(this Transform src, Transform target)
        {
            LayerMask targetableLayer = 1 << 0;
            RaycastHit hit;
            if (Physics.Raycast(src.position, target.position - src.position, out hit, Mathf.Infinity, targetableLayer))
            {
                // Debug.Log("hit: " + hit.transform.name);
                // Debug.DrawLine(src.position, target.position, Color.red, 0.1f);
                // return hit.transform.GetComponent<ITargetable>() != null;
                return hit.transform == target;
            } else return false;
        }
        public static void PlayNewClip(this AudioSource source, AudioClip clip)
        {
            float startVol = source.volume;
            source.volume = 0;
            source.Stop();
            source.clip = clip;
            source.Play();
            source.volume = startVol;
        }
    }
}