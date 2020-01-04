using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/PatrolModeEnabled")]
    public class PatrolModeEnabled : Condition
    {
        private void OnEnable()
        {
            description = "Is unit patrol mode enabled?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return state.baseBot.behaviorMode == AIBehaviorMode.Patrol;
            return false;
        }
    }
}
