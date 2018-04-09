using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyValueProvider : MonoBehaviour, IValueProvider<float>
{
    public EnergyValue value = null;

    public float GetValue()
    {
        return value.value;
    }

    public void InitValue()
    {
    }
}
