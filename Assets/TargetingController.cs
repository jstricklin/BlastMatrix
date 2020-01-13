using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Project.Controllers {
    public class TargetingController : MonoBehaviour
    {

        public List<Transform> allTargets = new List<Transform>();

        public void SetTargets(List<Transform> targets)
        {
            allTargets = targets;
        }
        // public void RemoveTarget(GameObject newTarget)
        // {
        //     allTargets.Remove(newTarget);
        // }
        public Transform NearestEnemy()
        {
            if (allTargets.Count < 1) return null;
            if (allTargets.Count < 2) return allTargets[0];
            else return allTargets.OrderBy(a => Vector3.Distance(a.position, transform.position))
           .ToList()[0];
        }

        public bool CheckObstacle()
        {
            return false;
        }

        public bool TargetInSight()
        {
            return false;
        }

        public bool TargetInRange()
        {
            return false;    
        }

    }
}