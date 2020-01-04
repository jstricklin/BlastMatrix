using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/OutOfRange")]
    public class OutOfRange : Condition
    {
        private void OnEnable()
        {
            description = "Is Target Out of Range?";
        }
        public override bool CheckCondition(StateManager state)
        {
            return state.currentDist > state.maxDist;
            // return state.currentTarget != null && state.currentDist > state.maxDist && state.lookTarget != state.avoidTarget;
        }
    }
}
