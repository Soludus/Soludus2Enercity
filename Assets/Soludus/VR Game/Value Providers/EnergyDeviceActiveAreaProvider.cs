using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDeviceActiveAreaProvider : MonoBehaviour, IValueProvider<float>
{
    public List<EnergyDeviceType> types = new List<EnergyDeviceType>();

    public void InitValue() { }

    public float GetValue()
    {
        float a = 0;
        for (int i = 0; i < types.Count; ++i)
        {
            a += types[i].totalActiveGridArea;
        }
        return a;
    }
}
