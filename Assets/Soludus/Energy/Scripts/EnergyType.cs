using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Base type for objects in the Soludus framework.
    /// </summary>
    public class EnergyType : ScriptableObject
    {
        [Multiline]
        public string description;
    }

}