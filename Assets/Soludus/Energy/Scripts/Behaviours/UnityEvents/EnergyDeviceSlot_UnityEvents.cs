using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Soludus.Energy
{

    public class EnergyDeviceSlot_UnityEvents : MonoBehaviour
    {
        [System.Serializable]
        public sealed class ControllerInteractionEvent : UnityEvent<object, EnergyDeviceSlotEventArgs> { }

        private EnergyDeviceSlot component;
        public ControllerInteractionEvent onDeviceAttached;
        public ControllerInteractionEvent onDeviceRemoved;

        private void OnEnable()
        {
            component = GetComponent<EnergyDeviceSlot>();

            component.DeviceAttached += OnDeviceAttached;
            component.DeviceRemoved += OnDeviceRemoved;
        }

        private void OnDisable()
        {
            component.DeviceAttached -= OnDeviceAttached;
            component.DeviceRemoved -= OnDeviceRemoved;
        }

        private void OnDeviceAttached(object sender, EnergyDeviceSlotEventArgs e)
        {
            onDeviceAttached.Invoke(sender, e);
        }

        private void OnDeviceRemoved(object sender, EnergyDeviceSlotEventArgs e)
        {
            onDeviceRemoved.Invoke(sender, e);
        }
    }

}