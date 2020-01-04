using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/CommandMode/FollowFalse")]
    public class FollowFalse : Condition
    {
        private void OnEnable()
        {
            description = "Is Unit NOT set to follow?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // if (state.baseBot.currentCommand == null) return false;
            // return state.baseBot.currentCommand.command != Command.CommandType.UnitsFollow && state.baseBot.currentCommand.command != Command.CommandType.ArmyFollow;
            return false;
        }
    }
}
