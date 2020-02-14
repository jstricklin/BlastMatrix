using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Basic/GetEnemyTarget")]
    public class StartChase : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.baseBot.StartChasing();
        }
    }
}
