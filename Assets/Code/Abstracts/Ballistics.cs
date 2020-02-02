using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Ballistics {
    public static class Ballistics 
    {
        public static float maxAngle = 45;
        public static Vector3[] GetTrajectory(int steps, Vector3 startPos, Vector3 vel, Rigidbody projectileRb, float angle)
        {
            Vector3[] posList = new Vector3[steps];
            float fTime = 0.1f;
            // Vector3 vel = Vector3.forward * 25 / projectileRb.mass;
            float pVel = Mathf.Sqrt((vel.z * vel.z) + (vel.y * vel.y)) / projectileRb.mass;
            // angle = Mathf.Rad2Deg*(Mathf.Atan2(vel.y, vel.z));
            // float pVel = 25 / projectileRb.mass;
            // float pVel = 25;
            posList[0] = startPos;
            for (int i = 1; i < steps; i++) 
            {
                float dz = pVel * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
                float dy = pVel * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - Physics.gravity.magnitude * fTime * fTime * 0.5f;
                Vector3 pos = new Vector3(startPos.x, startPos.y + dy, startPos.z + dz);
                posList[i] = pos;
                fTime += 0.1f;
            }
            // Debug.Log("angle " + angle);
            return posList;
        }
        public static float GetAngleToTarget(this Transform weapon, Vector3 target, float vel)
        {
            // Debug.Log("target... " + target);
            float grav = Physics.gravity.y;
            float dist = Vector3.Distance(weapon.position, target);
            // float dist = Mathf.Abs(weapon.position.z - target.z);
            // Debug.Log("dist " + dist);
            // float tanAngle1 = (vel * vel) + Mathf.Sqrt(Mathf.Pow(vel, 4) - grav * (grav * (target.z * target.z) + 2 * target.y * (vel * vel))) / grav * target.z;
            // float tanAngle2 = (vel * vel) - Mathf.Sqrt(Mathf.Pow(vel, 4) - grav * (grav * (target.z * target.z) + 2 * target.y * (vel * vel))) / grav * target.z;
            // float angle = Mathf.Atan(Mathf.Min(Mathf.Abs(tanAngle1), Mathf.Abs(tanAngle2)));
            // Debug.Log("target mag " + angle);
            float angle1 = (0.5f * Mathf.Asin((grav * dist) / (vel * vel))) * Mathf.Rad2Deg;
            float angle2 = 90 - (0.5f * Mathf.Asin((grav * target.z) / (vel * vel))) * Mathf.Rad2Deg;

            float angle = Mathf.Min(Mathf.Abs(angle1), Mathf.Abs(angle2));

            Debug.Log("angles: " + Mathf.Abs(angle1) + " | " + Mathf.Abs(angle2) + " angle: " + angle);
            // Debug.Log("angle to target: " + angle);

            // float angle = 0.5f * (Mathf.Asin((-Physics.gravity.y * Vector3.Distance(weapon.position, target)) / (vel * vel)) * Mathf.Rad2Deg);
            if (float.IsNaN(angle))
            {
                angle = 0;
            } 
            // if (angle < 0) angle *= -1;
            return angle;
            // return angle;
        }
        public static bool TargetInRange(this Transform weapon, Vector3 targetPos, float vel)
        {
            return (weapon.transform.position - targetPos).magnitude <= MaxDistance(vel);
        }
        public static float MaxDistance(float vel)
        {
            float maxDist = (vel * vel) * Mathf.Sin(2 * maxAngle) / Physics.gravity.magnitude;
            Debug.Log("max dist: " + maxDist);
            return maxDist;
        }
    }
}
