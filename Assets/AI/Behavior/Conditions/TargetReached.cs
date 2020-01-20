using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/TargetReached")]
    public class TargetReached : Condition
    {
        private void OnEnable()
        {
            description = "Has unit reached minimum distance to target?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // if (state.baseBot.currentCommand?.command == Command.CommandType.ArmyFollow || state.baseBot.currentCommand?.command == Command.CommandType.UnitsFollow)
            // return false;
            return state.currentTarget != null && state.baseBot.targetingController.currentDist <= state.baseBot.targetingController.maxDist && state.lookTarget != state.avoidTarget;
        }
    }
}
