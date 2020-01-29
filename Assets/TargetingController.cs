using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Utilities;
using Project.Gameplay;
using Project.Managers;

namespace Project.Controllers {
    public class TargetingController : MonoBehaviour
    {

        public List<Transform> allTargets = new List<Transform>();
        public List<Transform> targetsInSight = new List<Transform>();
        public List<Vector3> posList = new List<Vector3>();
        BaseBot botController;
        public Transform currentTarget;

        public float currentDist;
        public float maxDist = 10000f;
        [SerializeField]
        LineRenderer trajectory;
        [SerializeField]
        Projectile projectile;
        Rigidbody projectileRb;
        [SerializeField]
        Transform barrel, trajectoryStart;

        bool possibleHit = false;
        public enum AimState {
            IN_SIGHT,
            TOO_FAR,
            TOO_CLOSE,
        }

        public AimState aimState;

        void Start()
        {
            botController = GetComponentInParent<BaseBot>();
            projectileRb = projectile.GetComponent<Rigidbody>();
            StartCoroutine(CheckTargetsInSight());
            StartCoroutine(CheckTrajectory());
        }

        void Update()
        {
            if (currentTarget != null)
            {
                currentDist = (transform.position - currentTarget.position).sqrMagnitude;
            }
        }

        public void SetProjectile(Projectile newProjectile)
        {
            projectile = newProjectile;
            projectileRb = projectile.GetComponent<Rigidbody>();
        }

        public void TargetEnemy()
        {
            if (currentTarget != null) return;
            Transform trans;
            if (targetsInSight.Count > 0)
            {
                trans = NearestEnemyInSight();
            } else {
                trans = NearestEnemy();
            }
            botController.UpdateTarget(trans);
            currentTarget = trans;
        }

        public void SetTargets(List<Transform> targets)
        {
            allTargets = targets;
        }
        public void RemoveTarget(Transform toRemove)
        {
            if (allTargets.Contains(toRemove))
                allTargets.Remove(toRemove);
        }
        public void AddTarget(Transform toAdd)
        {
            if (!allTargets.Contains(toAdd))
                allTargets.Add(toAdd);
        }
        public void TargetInSight(Transform target)
        {
            if (!targetsInSight.Contains(target))
                targetsInSight.Add(target);
        }
        public Transform NearestEnemy()
        {
            if (allTargets.Count < 1) return null;
            if (allTargets.Count < 2) return allTargets[0];
            else return allTargets.OrderBy(a => Vector3.Distance(a.position, transform.position))
           .ToList()[0];
        }
        public Transform NearestEnemyInSight()
        {
            if (targetsInSight.Count < 1) return null;
            if (targetsInSight.Count < 2) return targetsInSight[0];
            else return targetsInSight.OrderBy(a => Vector3.Distance(a.position, transform.position))
           .ToList()[0];
        }

        public bool TargetInSight()
        {
            return transform.TargetInSight(currentTarget);
        }

        public bool TargetInRange()
        {
            return false;    
        }

        IEnumerator CheckTargetsInSight()
        {
            while(true)
            {
                targetsInSight.Clear();
                foreach(Transform tank in allTargets)
                {
                    if (transform.TargetInSight(tank))
                    {
                        TargetInSight(tank);
                    }
                }
                TargetEnemy();
                yield return new WaitForSeconds(0.25f);
            }
        }
        IEnumerator CheckTrajectory()
        {
            Vector3 startPos; 
            int step = 1;
            int maxSteps = 30;
            trajectory.positionCount = 1;
            float fTime = 0.1f;
            Color noHitColor = trajectory.material.color;
            Color hitColor = Color.green;
            hitColor.a = noHitColor.a;
            int hitCount = 0;
            while (true)
            {
                startPos = trajectory.GetPosition(0);
                posList.Add(startPos);
                if (trajectory.positionCount <= step) trajectory.positionCount++;
                Vector3 vel = (barrel.transform.position - barrel.forward).normalized * 25 / projectileRb.mass;
                // Debug.Log("vel " + vel + "barrel fwd" + (barrel.forward) * 25);
                float pVel = Mathf.Sqrt((vel.z * vel.z) + (vel.y * vel.y));
                float angle = Mathf.Rad2Deg*(Mathf.Atan2(vel.y, vel.z));
                float dz = pVel * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
                if (dz < 0) dz *= -1;
                float dy = pVel * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - Physics.gravity.magnitude * fTime * fTime * 0.5f;
                Vector3 pos = new Vector3(startPos.x, startPos.y + dy, startPos.z + dz);
                fTime += 0.1f;
                possibleHit = allTargets.Count > 0 ? (barrel.transform.TransformPoint(pos) - barrel.transform.TransformPoint(pos).NearestTarget(allTargets).position).sqrMagnitude < 10 : false;
                if (possibleHit)
                {
                    hitCount++;
                    trajectory.material.color = hitColor;
                }
                if (step >= maxSteps - 1 || barrel.transform.TransformPoint(pos).y < 0 /* || hitCount > 0 */) 
                {
                    float aimDot = currentTarget != null ? Vector3.Dot((barrel.transform.TransformPoint(pos) - currentTarget.transform.position).normalized, barrel.forward) : 0;
                    if (hitCount > 0)
                    {
                        aimState = AimState.IN_SIGHT;
                    } else {
                        trajectory.material.color = noHitColor;
                        if (aimDot < 0)
                        {
                            aimState = AimState.TOO_CLOSE;
                        } else {
                            aimState = AimState.TOO_FAR;
                        }
                        // Debug.Log("dot " + aimDot + "| aimState: " + aimState);
                    }
                    posList.Clear();
                    trajectory.positionCount = step > 1 ? step - 1 : 1;
                    hitCount = 0;
                    step = 1;
                    fTime = 0.1f;
                } else {
                    posList.Add(pos);
                    trajectory.SetPosition(step, pos);
                    step++;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        // public void DisplayTrajectory(LineRenderer line, List<Vector3> positions)
        // {
        //     for (int i = 0; i < line.positionCount; i++)
        //     {
        //         Debug.Log("positions count: " + positions.Count + "line count : " + line.positionCount);
        //         line.SetPosition(i, positions[i]);
        //     }
        // }
    }
}