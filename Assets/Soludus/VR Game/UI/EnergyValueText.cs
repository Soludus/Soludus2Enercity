using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyValueText : MonoBehaviour
{
    [SerializeField]
    private Text m_text = null;

    public EnergyValue m_value = null;

    public enum Property
    {
        Value,
        Rate
    }

    public Property m_property = Property.Value;

    private void Update()
    {
        switch (m_property)
        {
            case Property.Value:
                m_text.text = Mathf.FloorToInt(m_value.value) + m_value.valueUnitString;
                break;
            case Property.Rate:
                m_text.text = Mathf.FloorToInt(m_value.rate) + m_value.rateUnitString;
                break;
        }
    }
}
