using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/UnitCommands/ClearCommand")]
    public class ClearCommand : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.baseBot.ClearCommand();
        }

    }
        
}
