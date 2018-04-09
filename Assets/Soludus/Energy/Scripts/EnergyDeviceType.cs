using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Defines a type for an <see cref="EnergyDevice"/> used in energy simulation.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Energy Device Type", order = -1)]
    public class EnergyDeviceType : EnergyType
    {
        [Tooltip("Are these devices in use.")]
        [SerializeField]
        private bool m_enabled = true;

        [Tooltip("The prefab associated with this device type.")]
        public GameObject model = null;
        [Tooltip("The size of the device.")]
        public float size = 1;
        [Tooltip("Does this device require to be in a slot to be active.")]
        public bool requireSlot = true;
        [Tooltip("Maximum number of effects this device can receive.")]
        public int maxReceivedEffects = 0;

        [Tooltip("List of effects that affect Energy Values over time when devices of this type are active.")]
        public List<EnergyEffect> energyEffects = new List<EnergyEffect>();

        [Tooltip("List of effects that devices of this type can cause to other devices when active.")]
        public List<DeviceEffect> deviceEffects = new List<DeviceEffect>();

        /// <summary>
        /// All devices of this type.
        /// </summary>
        internal List<EnergyDevice> allDevices = new List<EnergyDevice>();

        /// <summary>
        /// All slots accepting devices of this type.
        /// </summary>
        internal List<EnergyDeviceSlot> allSlots = new List<EnergyDeviceSlot>();

        /// <summary>
        /// Are the devices of this type enabled.
        /// </summary>
        public bool enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        /// <summary>
        /// Total area of the active devices with a grid.
        /// </summary>
        public float totalActiveGridArea
        {
            get
            {
                float a = 0;
                for (int i = 0; i < allDevices.Count; ++i)
                {
                    if (allDevices[i].active)
                        a += allDevices[i].effectiveArea;
                }
                return a;
            }
        }

        /// <summary>
        /// Total area of the available slots.
        /// </summary>
        public float totalSlotGridArea
        {
            get
            {
                float a = 0;
                for (int i = 0; i < allSlots.Count; ++i)
                {
                    a += allSlots[i].effectiveArea;
                }
                return a;
            }
        }

        [System.Serializable]
        public class EnergyEffect
        {
            [Tooltip("The Energy Value that this effect will output to.")]
            public EnergyValue outputTo = null;
            [Tooltip("The base value of the effect.")]
            public float magnitude = 1;
            [Tooltip("The Formula that is used when calculating the output value of this effect. If left empty, a constant rate over time is used.")]
            public Formula energyFormula = null;
            [Tooltip("Multiplier for the effect output value.")]
            [Range(0, 1)]
            public float efficiency = 1;
            [Tooltip("When checked, this effect will try to output to target accumulators instead of the Energy Value directly.")]
            public bool tryUseTargetAccumulators = true;
        }

        [System.Serializable]
        public class DeviceEffect
        {
            [Tooltip("The device type that can be affected by this effect.")]
            public EnergyDeviceType targetType = null;
            [Tooltip("Output of the targeted device is multiplied by this value when this effect is active.")]
            public float efficiencyMultiplier = 1f;
        }

        /// <summary>
        /// Returns an effect if this type has a suitable effect for the targetType.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public DeviceEffect GetEffectForTarget(EnergyDeviceType targetType)
        {
            for (int i = 0; i < deviceEffects.Count; ++i)
            {
                if (deviceEffects[i].targetType == targetType)
                    return deviceEffects[i];
            }
            return null;
        }

        /// <summary>
        /// Returns all suitable effects this type has for the targetType.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public IEnumerable<DeviceEffect> GetEffectsForTarget(EnergyDeviceType targetType)
        {
            for (int i = 0; i < deviceEffects.Count; ++i)
            {
                if (deviceEffects[i].targetType == targetType)
                    yield return deviceEffects[i];
            }
        }

    }

}