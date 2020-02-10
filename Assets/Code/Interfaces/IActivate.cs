using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Interfaces {
    public interface IActivate 
    {
        string activator { get; set; }
        void SetActivator(string ID, bool fromBot);
        string GetActivator();
    }
}
