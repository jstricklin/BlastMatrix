using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/StopFleeing")]
    public class StopFleeing : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.baseBot.StopFleeing();
        }
    }
}
