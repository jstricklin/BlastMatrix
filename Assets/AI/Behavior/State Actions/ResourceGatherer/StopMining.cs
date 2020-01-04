using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Mining/StopMining")]
    public class StopMining : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.resourceGatherer.StopMining();
        }
    }
}
