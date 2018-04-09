using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows to control a color value using hue, saturation and value (HSV).
/// </summary>
[ExecuteInEditMode]
public class HSVToRGB : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float m_hue = 1;
    [SerializeField]
    [Range(0, 1)]
    private float m_saturation = 1;
    [SerializeField]
    [Range(0, 1)]
    private float m_value = 1;
    [SerializeField]
    [Range(0, 1)]
    private float m_alpha = 1;
    [SerializeField]
    private bool m_hdr = false;

    public ColorEvent onValueChanged = new ColorEvent() { };

    private Color m_prevColor;

    [System.Serializable]
    public class ColorEvent : UnityEvent<Color> { }

    public float hue
    {
        get { return m_hue; }
        set
        {
            m_hue = value;
            UpdateColor();
        }
    }

    public float saturation
    {
        get { return m_saturation; }
        set
        {
            m_saturation = value;
            UpdateColor();
        }
    }

    public float value
    {
        get { return m_value; }
        set
        {
            m_value = value;
            UpdateColor();
        }
    }

    public float alpha
    {
        get { return m_alpha; }
        set
        {
            m_alpha = value;
            UpdateColor();
        }
    }

    public bool hdr
    {
        get { return m_hdr; }
        set
        {
            m_hdr = value;
            UpdateColor();
        }
    }

    private void OnValidate()
    {
        UpdateColor();
    }

    private void OnDidApplyAnimationProperties()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        var c = Color.HSVToRGB(m_hue, m_saturation, m_value, m_hdr);
        c.a = m_alpha;
        if (m_prevColor != c)
        {
            onValueChanged.Invoke(c);
            m_prevColor = c;
        }
    }
}
