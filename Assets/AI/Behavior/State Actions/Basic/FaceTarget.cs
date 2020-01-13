using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/FaceTarget")]
    public class FaceTarget : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.baseBot.FaceTarget();
        }
    }
}
