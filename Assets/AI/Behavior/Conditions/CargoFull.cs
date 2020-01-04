using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Mining/CargoFull")]
    public class CargoFull : Condition
    {
        private void OnEnable()
        {
            description = "Is cargo bay full?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return state.resourceGatherer.currentResources >= state.resourceGatherer.maxResources;
            return false;
        }
    }
}
