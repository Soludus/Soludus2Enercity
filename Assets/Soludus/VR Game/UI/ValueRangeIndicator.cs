using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ValueRangeIndicator : MonoBehaviour
{
    [SerializeField]
    private Slider m_slider = null;
    [SerializeField]
    private Text m_text = null;

    [SerializeField]
    [Multiline, Tooltip("{0} = current value, {1} = max value")]
    private string m_textFormat = "Value/MaxValue {0}/{1}";
    [SerializeField]
    private float m_sliderSmoothing = 0;
    [SerializeField]
    private float m_textValueSmoothing = 0;

    private float m_currentValue = 0;

    private void Update()
    {
        float cVal = GetCurrentValue();
        float maxVal = GetMaxValue();

        m_slider.value = Mathf.MoveTowards(m_slider.value, cVal / maxVal, GetDelta(m_sliderSmoothing));

        m_currentValue = Mathf.MoveTowards(m_currentValue, cVal, GetDelta(m_textValueSmoothing * maxVal));
        m_text.text = string.Format(m_textFormat, m_currentValue, maxVal);
    }

    private float GetDelta(float smoothing)
    {
        if (smoothing <= 0)
        {
            return float.PositiveInfinity;
        }
        else
        {
            return Time.deltaTime / smoothing;
        }
    }

    protected abstract float GetCurrentValue();
    protected abstract float GetMaxValue();
}
