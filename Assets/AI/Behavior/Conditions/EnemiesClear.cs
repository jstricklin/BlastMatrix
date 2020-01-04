using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Enemies Clear")]
    public class EnemiesClear: Condition
    {
        private void OnEnable()
        {
            description = "Are there no enemies in sight?";
        }
        public override bool CheckCondition(StateManager state)
        {
            return state.enemyTargets.Count <= 0;   
        }
    }
}
