using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Utilities;
using Project.Gameplay;
using Project.Managers;
using Project.Ballistics;
using Project.Networking;

namespace Project.Controllers {
    public class TargetingController : MonoBehaviour
    {

        public List<Transform> allTargets = new List<Transform>();
        public List<Transform> targetsInSight = new List<Transform>();
        public Vector3[] posList;
        BaseBot botController;
        public Transform currentTarget;

        public float currentDist;
        public float currentAngle;
        public float angleToTarget;
        public float maxDist;
        [SerializeField]
        LineRenderer trajectory;
        [SerializeField]
        Projectile projectile;
        Rigidbody projectileRb;
        [SerializeField]
        Transform cannonBase, barrel, trajectoryStart;

        bool possibleHit = false;
        public enum AimState {
            IN_SIGHT,
            TOO_FAR,
            TOO_CLOSE,
        }

        Vector3 vector;

        public AimState aimState;

        void Start()
        {
            botController = GetComponentInParent<BaseBot>();
            if (botController != null) 
            {
                StartCoroutine(CheckTargetsInSight());
                StartCoroutine(CheckTrajectory());
            } else {
                StartCoroutine(DisplayProjectileTrajectory());
            }
            projectileRb = projectile.GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (currentTarget != null)
            {
                currentDist = (transform.position - currentTarget.position).sqrMagnitude;
            }
        }
        void FixedUpdate()
        {
            currentAngle = Vector3.Angle(cannonBase.forward, barrel.forward);
            vector = new Vector3(0, Mathf.Sin(currentAngle * Mathf.Deg2Rad), Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * 25;
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
        IEnumerator DisplayProjectileTrajectory()
        {
            int maxSteps = 100;
            trajectory.transform.SetParent(cannonBase);
            Color noHitColor = trajectory.material.color;
            Color hitColor = Color.green;
            hitColor.a = noHitColor.a;
            while(true)
            {
                bool possibleHit = Ballistics.Ballistics.DisplayTrajectory(trajectory, trajectoryStart, vector, maxSteps, currentAngle, allTargets);
                yield return new WaitForFixedUpdate();
            }
        }
        IEnumerator CheckTrajectory()
        {
            int maxSteps = 100;
            trajectory.transform.SetParent(cannonBase);
            Color noHitColor = trajectory.material.color;
            Color hitColor = Color.green;
            hitColor.a = noHitColor.a;
            while (true)
            {
                trajectory.enabled = BotManager.Instance.displayTrajectories;
                bool possibleHit = Ballistics.Ballistics.DisplayTrajectory(trajectory, trajectoryStart, vector, maxSteps, currentAngle, allTargets);                    
                if (currentTarget)
                {
                    angleToTarget = trajectoryStart.GetAngleToTarget(currentTarget.transform.position, vector.magnitude);
                }
                float tolerance = 0.5f;
                if (Mathf.Abs(currentAngle - angleToTarget) < tolerance && possibleHit)
                {
                    aimState = AimState.IN_SIGHT;
                    trajectory.material.color = hitColor;
                } else {
                    trajectory.material.color = noHitColor;
                    if (currentAngle < angleToTarget)
                    {
                        aimState = AimState.TOO_CLOSE;
                    } else {
                        aimState = AimState.TOO_FAR;
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
}