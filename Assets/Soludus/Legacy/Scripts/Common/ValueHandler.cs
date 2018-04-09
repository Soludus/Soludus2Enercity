using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ValueHandler<T> : MonoBehaviour where T : System.IEquatable<T>
{
    public abstract UnityEvent<T> onValueChanged { get; }

    [SerializeField]
    private T m_value = default(T);
    private bool m_changed = true;

    public T value
    {
        get { return m_value; }
        set
        {
            m_changed |= !m_value.Equals(value);
            m_value = value;
            if (m_changed)
            {
                onValueChanged.Invoke(m_value);
                m_changed = false;
            }
        }
    }
}
