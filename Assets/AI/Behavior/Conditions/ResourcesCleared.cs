using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Mining/ResourcesCleared")]
    public class ResourcesCleared: Condition
    {
        private void OnEnable()
        {
            description = "Are all local resources depleted?";
        }
        public override bool CheckCondition(StateManager state)
        {
            // return state.currentTarget == null && state.resourceGatherer.resourceNodes.Count <= 0;   
            return false;
        }
    }
}
