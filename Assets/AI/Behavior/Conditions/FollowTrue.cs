using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/CommandMode/FollowTrue")]
    public class FollowTrue : Condition
    {
        private void OnEnable()
        {
            description = "Is Unit set to follow?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return state.baseBot.currentCommand.command == Command.CommandType.UnitsFollow || state.baseBot.currentCommand.command == Command.CommandType.ArmyFollow;
            return false;
        }
    }
}
