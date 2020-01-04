using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Mining/CargoEmpty")]
    public class CargoEmpty : Condition
    {
        private void OnEnable()
        {
            description = "Is cargo bay empty?";
        }

        public override bool CheckCondition(StateManager state)
        {
            // return state.resourceGatherer.currentResources <= 0;
            return false;
        }
    }
}
