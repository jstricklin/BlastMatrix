using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/EnemiesInSight")]
    public class EnemiesInSight : Condition
    {
        private void OnEnable()
        {
            description = "Are there enemies in sight?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // if (state.baseBot.behaviorMode == AIBehaviorMode.Patrol || 
            //     state.baseBot.behaviorMode == AIBehaviorMode.Command && state.baseBot.currentCommand != null && 
            //         !state.baseBot.currentCommand.passive)
            // {
            //     return state.enemyTargets.Count > 0;
            // } else return false;
            return false;
        }
    }
}
