using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleEvent : MonoBehaviour
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent onValueSet = new BoolEvent();

    public bool state = false;

    public void Toggle()
    {
        Set(!state);
    }

    public void Set(bool val)
    {
        state = val;
        onValueSet.Invoke(state);
    }
}
