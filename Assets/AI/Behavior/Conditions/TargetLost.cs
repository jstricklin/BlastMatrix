using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/TargetLost")]
    public class TargetLost : Condition
    {
        private void OnEnable()
        {
            description = "Is current target null?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return false;
            return !state.baseBot.targetingController.targetsInSight.Contains(state.currentTarget);
        }
    }
}
