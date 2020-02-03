using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;

namespace Project.Ballistics {
    public static class Ballistics 
    {
        public static float maxAngle = 45;
        public static Vector3[] GetTrajectory(int steps, Vector3 vel, float angle)
        {
            Vector3[] posList = new Vector3[steps];
            float fTime = 0.1f;
            // Vector3 vel = Vector3.forward * 25 / projectileRb.mass;
            float pVel = Mathf.Sqrt((vel.z * vel.z) + (vel.y * vel.y));
            // angle = Mathf.Rad2Deg*(Mathf.Atan2(vel.y, vel.z));
            // float pVel = 25 / projectileRb.mass;
            // float pVel = 25;
            Vector3 startPos = Vector3.zero;
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
            float angle1 = Mathf.Abs(0.5f * Mathf.Asin((grav * dist) / (vel * vel)) * Mathf.Rad2Deg);
            float angle2 = Mathf.Abs(90 - 0.5f * Mathf.Asin((grav * target.z) / (vel * vel)) * Mathf.Rad2Deg);

            float angle = Mathf.Min(angle1, angle2);

            // Debug.Log("angles: " + Mathf.Abs(angle1) + " | " + Mathf.Abs(angle2) + " angle: " + angle);
            // Debug.Log("angle to target: " + angle);

            // float angle = 0.5f * (Mathf.Asin((-Physics.gravity.y * Vector3.Distance(weapon.position, target)) / (vel * vel)) * Mathf.Rad2Deg);
            if (float.IsNaN(angle))
            {
                angle = 0;
            } if (angle > maxAngle) {
                angle = maxAngle;
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
            float maxDist = (vel * vel) * Mathf.Sin(2 * maxAngle) / Physics.gravity.y;
            Debug.Log("max dist: " + maxDist);
            return maxDist;
        }

        public static bool DisplayTrajectory(LineRenderer trajectory, Transform trajectoryStart, Vector3 vector, int maxSteps, float currentAngle, List<Transform> targetList = null)
        {
            int hitCount = 0;
            int i = 0;
            trajectory.positionCount = maxSteps;
            Vector3[] posList;
            posList = GetTrajectory(maxSteps, vector, currentAngle);
            foreach(Vector3 pos in posList)
            {
                Vector3 newPos = pos + trajectory.transform.InverseTransformPoint(trajectoryStart.transform.position);
                trajectory.SetPosition(i, newPos);
                if (targetList != null && CheckPointForTargetHit(trajectory, newPos, targetList)) hitCount++;
                if (trajectory.transform.TransformPoint(pos).y <= 0)
                {
                    trajectory.positionCount = i + 1;
                    // Debug.Log("new pos + " + trajectory.transform.TransformPoint(newPos));
                    // rangeColl.radius = pos.z; 
                    break;
                }
                i++;
            }
            return hitCount > 0;
        }
        public static bool CheckPointForTargetHit(LineRenderer trajectory, Vector3 pos, List<Transform> targetList)
        {
            return targetList.Count > 0 ? (trajectory.transform.TransformPoint(pos) - trajectory.transform.TransformPoint(pos).NearestTarget(targetList).position).sqrMagnitude < 25 : false;
        }
    }
}
