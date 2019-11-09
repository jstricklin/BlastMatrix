using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Gameplay {
    public interface IProjectile 
    {
        string activator { get; set; }
        void SetActivator(string ID);
        string GetActivator();
    }
}
