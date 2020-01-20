using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/Aim")]
    public class Aim : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.baseBot.AimAtTarget();
        }
    }
}