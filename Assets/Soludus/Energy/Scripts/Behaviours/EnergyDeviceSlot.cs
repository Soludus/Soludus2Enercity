using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    public struct EnergyDeviceSlotEventArgs
    {
        public EnergyDevice device;
        public int x, y;
    }

    public delegate void EnergyDeviceSlotEventHandler(object sender, EnergyDeviceSlotEventArgs e);

    /// <summary>
    /// Represents a slot for <see cref="EnergyDevice"/>s.
    /// </summary>
    [SelectionBase]
    public class EnergyDeviceSlot : MonoBehaviour
    {
        [Tooltip("Only Energy Devices of this type can attach to this slot.")]
        public EnergyDeviceType acceptedType = null;
        [Tooltip("Add a GridData component and assign it here to define a grid size for this slot. Must not be null.")]
        public GridData grid = null;
        [Tooltip("(Optional) Energy Accumulators that will store the Energy value produced by devices in this slot.")]
        public List<EnergyAccumulator> accumulators = new List<EnergyAccumulator>();

        internal List<EnergyDevice> attachedDevices = new List<EnergyDevice>();
        internal EnergyDevice targetedBy = null;
        internal EnergyEffect lastEnergyEffect;
        internal List<DeviceEffect> deviceEffects = new List<DeviceEffect>();
        internal float lastEffectTime = 0;

        public event EnergyDeviceSlotEventHandler DeviceAttached;
        public event EnergyDeviceSlotEventHandler DeviceRemoved;

        [System.Serializable]
        public class SlotData
        {
            /// <summary>
            /// Reference to the device attached to this slot.
            /// </summary>
            public EnergyDevice attachedDevice = null;
        }

        /// <summary>
        /// The width * height matrix of currently attached devices.
        /// </summary>
        private SlotData[] slotMatrix = null;

        public virtual float effectiveArea
        {
            get
            {
                return (grid != null ? grid.area : 1) * acceptedType.size;
            }
        }

        protected virtual void OnEnable()
        {
            if (grid == null)
                grid = GetComponentInChildren<GridData>();

            if (acceptedType != null)
                acceptedType.allSlots.Add(this);

            attachedDevices.Clear();
            GetComponentsInChildren(attachedDevices);
            for (int i = 0; i < attachedDevices.Count; ++i)
            {
                attachedDevices[i].slot = this;
            }
        }

        protected virtual void OnDisable()
        {
            if (acceptedType != null)
                acceptedType.allSlots.Remove(this);
        }

        public virtual bool HasActiveDevices()
        {
            for (int i = 0; i < attachedDevices.Count; ++i)
            {
                if (attachedDevices[i].active)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the device effect to this slot and all attached devices.
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

            for (int i = 0; i < attachedDevices.Count; ++i)
            {
                attachedDevices[i].AddDeviceEffect(device, effect);
            }
        }

        /// <summary>
        /// Removes all effects of the device from this slot and all attached devices.
        /// </summary>
        /// <param name="device">The device whose effects will be removed.</param>
        public void RemoveDeviceEffects(EnergyDevice device)
        {
            for (int i = 0; i < attachedDevices.Count; ++i)
            {
                attachedDevices[i].RemoveDeviceEffects(device);
            }

            for (int i = deviceEffects.Count - 1; i >= 0; --i)
            {
                if (deviceEffects[i].source == device)
                    deviceEffects.RemoveAt(i);
            }
            lastEffectTime = Time.time;
        }



        /* Grid */

        protected SlotData GetSlot(int x, int y)
        {
            // create or resize slot matrix if necessary
            UpdateMatrix();
            return slotMatrix[x + grid.width * y];
        }

        protected void SetSlot(int x, int y, SlotData value)
        {
            UpdateMatrix();
            slotMatrix[x + grid.width * y] = value;
        }

        private void UpdateMatrix()
        {
            if (slotMatrix == null || slotMatrix.Length != grid.width * grid.height)
            {
                var newMatrix = new SlotData[grid.width * grid.height];
                if (slotMatrix != null)
                    System.Array.Copy(slotMatrix, newMatrix, Mathf.Min(slotMatrix.Length, newMatrix.Length));
                slotMatrix = newMatrix;

                for (int i = 0; i < slotMatrix.Length; ++i)
                {
                    if (slotMatrix[i] == null)
                        slotMatrix[i] = new SlotData();
                }
            }
        }

        protected void SetToMatrix(EnergyDevice device, int x, int y)
        {
            for (int ix = 0; ix < device.grid.width; ++ix)
            {
                for (int iy = 0; iy < device.grid.height; ++iy)
                {
                    var slot = GetSlot(x + ix, y + iy);
                    slot.attachedDevice = device;
                }
            }
        }

        protected void RemoveFromMatrix(EnergyDevice device)
        {
            for (int x = 0; x < grid.width; ++x)
            {
                for (int y = 0; y < grid.height; ++y)
                {
                    var slot = GetSlot(x, y);
                    if (slot.attachedDevice == device)
                        slot.attachedDevice = null;
                }
            }
        }

        protected void RemoveFromMatrix(int x, int y)
        {
            var device = GetSlot(x, y).attachedDevice;
            if (device == null)
                return;
            for (int ix = 0; ix < device.grid.width; ++ix)
            {
                for (int iy = 0; iy < device.grid.height; ++iy)
                {
                    var slot = GetSlot(x + ix, y + iy);
                    if (slot.attachedDevice == device)
                        slot.attachedDevice = null;
                }
            }
        }

        /// <summary>
        /// Gets the device from the position in this slot.
        /// </summary>
        /// <param name="x">X coordinate of the position.</param>
        /// <param name="y">Y coordinate of the position.</param>
        /// <returns>The device.</returns>
        public EnergyDevice GetDeviceInPosition(int x, int y)
        {
            return GetSlot(x, y).attachedDevice;
        }

        /// <summary>
        /// Checks if the position is available for the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="x">X coordinate of the position.</param>
        /// <param name="y">Y coordinate of the position.</param>
        /// <returns>True if the position is available.</returns>
        public bool IsValidPosition(EnergyDevice device, int x, int y)
        {
            // check if in bounds
            if (x < 0 || y < 0) return false;
            if (x + device.grid.width > grid.width || y + device.grid.height > grid.height) return false;

            // check if area occupied
            for (int ix = 0; ix < device.grid.width; ++ix)
            {
                for (int iy = 0; iy < device.grid.height; ++iy)
                {
                    if (GetSlot(x + ix, y + iy).attachedDevice != null)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds an available position for the device in this slot.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="x">X coordinate of the found position.</param>
        /// <param name="y">Y coordinate of the found position.</param>
        /// <returns>True if a free position was found.</returns>
        public bool FindFirstAvailablePosition(EnergyDevice device, out int x, out int y)
        {
            // naive, inefficient
            for (int ix = 0; ix < grid.width; ++ix)
            {
                for (int iy = 0; iy < grid.height; ++iy)
                {
                    if (IsValidPosition(device, ix, iy))
                    {
                        x = ix; y = iy;
                        return true;
                    }
                }
            }
            x = y = -1;
            return false;
        }

        /// <summary>
        /// Tries to add the device to the position in this slot.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="x">X coordinate of the position.</param>
        /// <param name="y">Y coordinate of the position.</param>
        /// <returns>True if the device was added.</returns>
        public bool TryAddToPosition(EnergyDevice device, int x, int y)
        {
            if (!IsValidPosition(device, x, y))
                return false;

            AddToPosition(device, x, y);
            return true;
        }

        /// <summary>
        /// Tries to find an available position for the device in this slot and add it to that position.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="x">X coordinate of the position of the added device.</param>
        /// <param name="y">Y coordinate of the position of the added device.</param>
        /// <returns>True if the device was added.</returns>
        public bool TryAddToFirstAvailablePosition(EnergyDevice device, out int x, out int y)
        {
            if (FindFirstAvailablePosition(device, out x, out y))
            {
                AddToPosition(device, x, y);
                return true;
            }
            return false;
        }

        /* protected because this does not check if the position is valid */
        protected void AddToPosition(EnergyDevice device, int x, int y)
        {
            SetToMatrix(device, x, y);

            if (attachedDevices.Count == 0)
                lastEffectTime = Time.time;

            attachedDevices.Add(device);
            for (int i = 0; i < deviceEffects.Count; ++i)
            {
                device.deviceEffects.Add(deviceEffects[0]);
            }
            device.slot = this;

            if (DeviceAttached != null)
                DeviceAttached(this, new EnergyDeviceSlotEventArgs { device = device, x = x, y = y });
        }

        public bool Remove(EnergyDevice device)
        {
            if (!attachedDevices.Remove(device))
                return false;
            device.slot = null;
            int x, y;
            if (FindPositionOfDevice(device, out x, out y))
            {
                RemoveFromMatrix(x, y);
            }
            for (int i = 0; i < deviceEffects.Count; ++i)
            {
                device.RemoveDeviceEffects(deviceEffects[i].source);
            }
            if (DeviceRemoved != null)
                DeviceRemoved(this, new EnergyDeviceSlotEventArgs { device = device, x = x, y = y });
            return true;
        }

        public bool FindPositionOfDevice(EnergyDevice device, out int x, out int y)
        {
            for (int ix = 0; ix < grid.width; ++ix)
            {
                for (int iy = 0; iy < grid.height; ++iy)
                {
                    if (GetSlot(ix, iy).attachedDevice == device)
                    {
                        x = ix; y = iy;
                        return true;
                    }
                }
            }
            x = y = -1;
            return false;
        }

        public void SetElementsActive(EnergyDevice device, bool active, int childIndex = -1)
        {
            for (int x = 0; x < grid.width; ++x)
            {
                for (int y = 0; y < grid.height; ++y)
                {
                    var slot = GetSlot(x, y);
                    if (slot.attachedDevice == device)
                    {
                        var go = grid.GetElementObject(x, y);
                        if (go != null)
                        {
                            if (childIndex >= 0)
                            {
                                var child = go.transform.GetChild(childIndex);
                                if (child != null)
                                    child.gameObject.SetActive(active);
                            }
                            else
                            {
                                go.SetActive(active);
                            }
                        }
                    }
                }
            }
        }

    }

}