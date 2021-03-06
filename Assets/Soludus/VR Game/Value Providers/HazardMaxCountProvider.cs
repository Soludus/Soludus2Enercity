﻿using Soludus.Energy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardMaxCountProvider : MonoBehaviour, IValueProvider<int>, IValueProvider<float>
{
    public MapHandle mapHandle = null;

    public void InitValue() { }

    public int GetValue()
    {
        return mapHandle.mapConfiguration.hazards.maxHazardCount;
    }

    float IValueProvider<float>.GetValue()
    {
        return GetValue();
    }
}
