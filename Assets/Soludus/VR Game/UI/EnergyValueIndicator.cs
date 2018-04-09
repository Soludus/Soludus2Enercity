using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyValueIndicator : ValueRangeIndicator
{
    public EnergyValue m_value;

    public enum Property
    {
        Value,
        Rate
    }

    public Property m_property = Property.Value;

    protected override float GetCurrentValue()
    {
        switch (m_property)
        {
            case Property.Value:
                return m_value.value;
            case Property.Rate:
                return m_value.rate;
            default:
                throw null;
        }
    }

    protected override float GetMaxValue()
    {
        switch (m_property)
        {
            case Property.Value:
                return m_value.targetValue;
            case Property.Rate:
                return m_value.targetRate;
            default:
                throw null;
        }
    }
}
