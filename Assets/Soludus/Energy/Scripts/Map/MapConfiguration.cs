using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Configuration data for a map, also know as a game level.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Map Configuration")]
    public class MapConfiguration : ScriptableObject
    {
        [Tooltip("Geographic location.")]
        public LocationData location = null;
        [Tooltip("Local date and time when the map is loaded.")]
        public UDateTime beginDateTime = System.DateTime.Now;
        [Tooltip("Set the speed time passes. E.g. Time Scale = 2.0 means that a minute will elapse in 30 seconds.")]
        public float timeScale = 1;
        [Tooltip("Create a Hazard Configuration and drag it here to provide data for spawning hazardous devices.")]
        public HazardConfiguration hazards = null;
        [Tooltip("The Energy Values used in this Map. You can define target values to set a target for the player.")]
        public List<EnergyTarget> energyTargets = new List<EnergyTarget>();

        [System.Serializable]
        public class EnergyTarget
        {
            public EnergyValue energyValue;
            [Tooltip("What value should be reached for this Energy Value to consider the map complete.")]
            public float targetValue;
        }
    }

}