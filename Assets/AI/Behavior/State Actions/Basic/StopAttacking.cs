using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Combat/StopAttacking")]
    public class StopAttacking : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.baseBot.StopAttacking();
        }
    }
}
