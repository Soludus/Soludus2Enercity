using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDeviceTotalAreaProvider : MonoBehaviour, IValueProvider<float>
{
    public List<EnergyDeviceType> types = new List<EnergyDeviceType>();

    public void InitValue() { }

    public float GetValue()
    {
        float ta = 0;
        for (int i = 0; i < types.Count; ++i)
        {
            ta += types[i].totalSlotGridArea;
        }
        return ta;
    }
}
