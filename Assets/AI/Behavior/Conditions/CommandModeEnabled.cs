using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/CommandModeEnabled")]
    public class CommandModeEnabled : Condition
    {
        private void OnEnable()
        {
            description = "Is unit command mode enabled?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return state.baseBot.behaviorMode == AIBehaviorMode.Command;
            return false;
        }
    }
}
