using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/UnderAttack")]
    public class UnderAttack : Condition
    {
        private void OnEnable()
        {
            description = "Is unit under attack?";
        }

        public override bool CheckCondition(StateManager state)
        {
            return state.attacker != null;
        }
    }
}
