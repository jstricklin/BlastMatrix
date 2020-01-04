using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/MoveToTarget")]
    public class MoveToTarget : StateActions
    {
        public override void Execute(StateManager states)
        {
            // Debug.Log("moving... to target.");
            states.baseBot.MoveForward(true);
        }
    }
}
