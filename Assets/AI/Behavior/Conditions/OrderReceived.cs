using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/OrderReceived")]
    public class OrderReceived : Condition
    {
        private void OnEnable()
        {
            description = "Has unit received an order?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return state.baseBot.currentCommand != null && state.baseBot.currentCommand != state.baseBot.prevCommand;
            return false;
        }
    }
}
