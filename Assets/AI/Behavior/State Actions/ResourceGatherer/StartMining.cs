using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Mining/StartMining")]
    public class StartMining : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.resourceGatherer.StartMining();
        }
    }
}
