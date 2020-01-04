using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Combat/StartAttacking")]
    public class StartAttacking : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.baseBot.StartAttacking();
        }
    }
}
