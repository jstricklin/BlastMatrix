using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Surface/Patrol")]
    public class StartPatrol : StateActions
    {

        public override void Execute(StateManager states)
        {
            states.baseBot.StartPatrolling();
        }
    }
}
