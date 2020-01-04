using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/UnitCommands/SetCommandMode")]
    public class SetCommandMode : StateActions
    {
        public override void Execute(StateManager states)
        {
            // states.baseBot.SetCommandMode();
        }

    }
        
}
