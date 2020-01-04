using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/StartFleeing")]
    public class StartFleeing : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.baseBot.StartFleeing(states.attacker);
        }
    }
}
