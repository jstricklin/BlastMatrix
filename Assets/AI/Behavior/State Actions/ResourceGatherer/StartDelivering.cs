using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Mining/StartDelivering")]
    public class StartDelivering : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.resourceGatherer.StartDelivering();
        }
    }
}
