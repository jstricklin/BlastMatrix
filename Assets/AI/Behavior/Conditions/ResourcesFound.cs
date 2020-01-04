using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Mining/ResourcesAppeared")]
    public class ResourcesFound: Condition
    {
        private void OnEnable()
        {
            description = "Have resources appeared?";
        }
        public override bool CheckCondition(StateManager state)
        {
            // return state.resourceGatherer.resourceNodes.Count > 0;   
            return false;
        }
    }
}
