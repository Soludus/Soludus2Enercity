using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Define a format string and use SetArgument to set the argument values.
/// </summary>
public class FormatText : MonoBehaviour
{
    [Multiline]
    [SerializeField]
    private string m_format = "";
    [SerializeField]
    private int m_numArgs = 0;
    [SerializeField]
    public TextEvent m_onTextChanged = new TextEvent();

    private object[] m_args;
    private string m_text = "";

    [System.Serializable]
    public class TextEvent : UnityEvent<string> { }

    public string text
    {
        get
        {
            return m_text;
        }
    }

    public string format
    {
        get
        {
            return m_format;
        }

        set
        {
            if (!Equals(m_format, value))
            {
                m_format = value;
                UpdateText();
            }
        }
    }

    public void SetArgument(int i, object value)
    {
        if (m_args == null || m_args.Length != m_numArgs)
        {
            var args = new object[m_numArgs];
            if (m_args != null)
                m_args.CopyTo(args, 0);
            m_args = args;
        }
        if (!Equals(m_args[i], value))
        {
            m_args[i] = value;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        m_text = string.Format(m_format, m_args);
        m_onTextChanged.Invoke(m_text);
    }
}
