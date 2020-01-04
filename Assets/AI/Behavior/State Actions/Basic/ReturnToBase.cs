using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/ReturnToBase")]
    public class ReturnToBase : StateActions
    {

        public override void Execute(StateManager states)
        {
            // states.baseBot.ReturnToBase();
        }
    }
}
