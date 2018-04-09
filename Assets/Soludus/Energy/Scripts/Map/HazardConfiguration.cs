using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Configuration data used by a <see cref="DeviceHazardManager"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Hazard Configuration")]
    public class HazardConfiguration : ScriptableObject
    {
        [Tooltip("Is hazard spawning enabled.")]
        public bool hazardsEnabled = true;
        [Tooltip("Maximum number of hazards that can be present at a time.")]
        public int maxHazardCount = 10;
        [Tooltip("Hazards will not target a slot until this many seconds have elapsed since the slot was previously targeted.")]
        public int targetCooldown = 5;
        [Tooltip("Scale the hazard spawning speed.")]
        public float spawnSpeedScale = 1;
        [Tooltip("The interval hazards spawn at will vary randomly between min and max.")]
        [MinMaxSlider(0, 100)]
        public Vector2 spawnIntervalRange = new Vector2(1, 5);
        [Tooltip("List of hazard types to spawn.")]
        public List<DeviceHazard> spawnedHazards = new List<DeviceHazard>();

        [System.Serializable]
        public class DeviceHazard
        {
            [Tooltip("The Energy Device Type of the hazard.")]
            public EnergyDeviceType type = null;
            [Tooltip("Portion of the total spawned devices will be this type. Relative to the total sum of chances.")]
            public float chance = 1;
            [Tooltip("How many hazards of this type can be present at a time.")]
            public int maxCount = -1;
        }

        public int currentHazardCount
        {
            get
            {
                int c = 0;
                for (int i = 0; i < spawnedHazards.Count; ++i)
                {
                    c += spawnedHazards[i].type.allDevices.Count;
                }
                return c;
            }
        }
    }

}