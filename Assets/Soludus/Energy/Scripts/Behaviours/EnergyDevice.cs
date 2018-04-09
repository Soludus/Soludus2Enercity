using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    public struct EnergyEffect
    {
        public float size;
        public float efficiency;
        public float deviceEffectMultiplier;
        public EnergyAccumulator accumulator;
    }

    public struct DeviceEffect
    {
        public EnergyDevice source;
        public float efficiencyMultiplier;
    }

    /// <summary>
    /// Represents a device that interacts with <see cref="EnergyValue"/>s or other <see cref="EnergyDevice"/>s.
    /// </summary>
    [SelectionBase]
    public class EnergyDevice : MonoBehaviour
    {
        [Tooltip("The Energy Device Type of this device.")]
        public EnergyDeviceType type = null;
        [Tooltip("Add a GridData component and assign it here to define a grid size for this device. If null, this device has a size of 1x1. Must not be null if you want to attach the device to a slot.")]
        public GridData grid = null;
        [Tooltip("The Energy Device Slot that will receive the effects from this device when it is activated. Use the SetTarget() method to change this at runtime.")]
        public EnergyDeviceSlot targetForEffects = null;

        /// <summary>
        /// The <see cref="EnergyDeviceSlot"/> this device is attached to.
        /// </summary>
        internal EnergyDeviceSlot slot = null;
        internal bool forceDeactive = false;

        internal EnergyEffect lastEnergyEffect;
        internal List<DeviceEffect> deviceEffects = new List<DeviceEffect>();
        internal float lastEffectTime = 0;

        private EnergyDeviceSlot effectsSentTo = null;

        /// <summary>
        /// Is this device active. Inactive devices won't have any effects.
        /// </summary>
        public virtual bool active
        {
            get
            {
                if (forceDeactive)
                    return false;
                if (type.requireSlot && slot == null)
                    return false;
                return isActiveAndEnabled;
            }
        }

        public virtual float effectiveArea
        {
            get
            {
                return (grid != null ? grid.area : 1) * type.size;
            }
        }

        protected virtual void OnEnable()
        {
            if (grid == null)
                grid = GetComponentInChildren<GridData>();

            if (type != null)
                type.allDevices.Add(this);

            Debug.Assert(effectsSentTo == null);

            effectsSentTo = targetForEffects;
            if (effectsSentTo != null)
            {
                foreach (var ef in type.GetEffectsForTarget(effectsSentTo.acceptedType))
                {
                    effectsSentTo.AddDeviceEffect(this, ef);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (type != null)
                type.allDevices.Remove(this);

            if (effectsSentTo != null)
            {
                effectsSentTo.RemoveDeviceEffects(this);
                effectsSentTo = null;
            }
        }

        /// <summary>
        /// Changes the target of this device. The device effects are transferred to the new target.
        /// </summary>
        /// <param name="target">The new target.</param>
        public void SetTarget(EnergyDeviceSlot target)
        {
            if (targetForEffects != null)
                targetForEffects.targetedBy = null;

            targetForEffects = target;

            if (targetForEffects != null)
                targetForEffects.targetedBy = this;

            if (isActiveAndEnabled)
            {
                if (effectsSentTo != target)
                {
                    if (effectsSentTo != null)
                        effectsSentTo.RemoveDeviceEffects(this);

                    effectsSentTo = target;
                    if (effectsSentTo != null)
                    {
                        foreach (var ef in type.GetEffectsForTarget(effectsSentTo.acceptedType))
                        {
                            effectsSentTo.AddDeviceEffect(this, ef);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the device effect to this device.
        /// </summary>
        /// <param name="device">The device that is the source of the effect.</param>
        /// <param name="effect">The effect data.</param>
        public void AddDeviceEffect(EnergyDevice device, EnergyDeviceType.DeviceEffect effect)
        {
            deviceEffects.Add(new DeviceEffect
            {
                source = device,
                efficiencyMultiplier = effect.efficiencyMultiplier
            });
        }

        /// <summary>
        /// Removes all effects of the device from this device.
        /// </summary>
        /// <param name="device">The device whose effects will be removed.</param>
        public void RemoveDeviceEffects(EnergyDevice device)
        {
            for (int i = deviceEffects.Count - 1; i >= 0; --i)
            {
                if (deviceEffects[i].source == device)
                    deviceEffects.RemoveAt(i);
            }
            lastEffectTime = Time.time;
        }
    }

}