using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatValue : ValueHandler<float>
{
    [SerializeField]
    private ChangedEvent m_onValueChanged;

    public override UnityEvent<float> onValueChanged
    {
        get { return m_onValueChanged; }
    }

    [System.Serializable]
    public class ChangedEvent : UnityEvent<float> { }

}
