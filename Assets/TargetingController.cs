using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Utilities;
using Project.Gameplay;

namespace Project.Controllers {
    public class TargetingController : MonoBehaviour
    {

        public List<Transform> allTargets = new List<Transform>();
        public List<Transform> targetsInSight = new List<Transform>();
        PlayerController playerController;
        public Transform currentTarget;

        public float currentDist;
        public float maxDist = 8000f;
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
            playerController = GetComponentInParent<PlayerController>();
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
            playerController.UpdateTarget(trans);
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

        public bool CheckObstacle()
        {
            return false;
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
            Vector3 startPos = trajectory.GetPosition(0);
            int step = 0;
            int maxSteps = 10;
            trajectory.positionCount = maxSteps;
            float fTime = 0;
            while (true)
            {
                step++;
                possibleHit = false;
                Vector3 vel = (trajectoryStart.position - trajectoryStart.forward) / projectileRb.mass * 0.25f;
                float pVel = Mathf.Sqrt((vel.z * vel.z) + (vel.y * vel.y));
                float angle = Mathf.Rad2Deg*(Mathf.Atan2(vel.y, vel.z));
                float dz = pVel * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
                if (dz < 0) dz *= -1;
                float dy = pVel * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
                Vector3 pos = new Vector3(startPos.x, startPos.y + dy, startPos.z + dz);
                fTime += 0.1f;
                // yPos = (step * Mathf.Tan(0) - (Physics.gravity.y * (step * step))) / 2 * 3 * 3 * (Mathf.Cos(0) * Mathf.Cos(0));
                // Debug.Log("steps: " + trajectory.positionCount + " and step: " + step);
                trajectory.SetPosition(step, pos);
                if (!possibleHit)
                {
                    possibleHit = (pos - currentTarget.position).sqrMagnitude < 500f;
                }
                if (step >= maxSteps - 1) 
                {
                    if (!possibleHit)
                    {
                        if ((pos - transform.position).sqrMagnitude < currentDist)
                        {
                            aimState = AimState.TOO_CLOSE;
                        } else {
                            aimState = AimState.TOO_FAR;
                        }
                    } else {
                        aimState = AimState.IN_SIGHT;
                    }
                    step = 0;
                    fTime = 0.1f;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}