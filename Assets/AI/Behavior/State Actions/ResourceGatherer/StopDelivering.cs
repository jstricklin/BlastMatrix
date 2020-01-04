using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Mining/StopDelivering")]
    public class StopDelivering : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.resourceGatherer.StopDelivering();
        }
    }
}
