using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/AttackerLost")]
    public class AttackerLost : Condition
    {
        private void OnEnable()
        {
            description = "Has unit lost attacker?";
        }

        public override bool CheckCondition(StateManager state)
        {
            return (state.mTransform.position - state.attacker.position).sqrMagnitude > 8000f || state.attacker == null;
        }
    }
}
