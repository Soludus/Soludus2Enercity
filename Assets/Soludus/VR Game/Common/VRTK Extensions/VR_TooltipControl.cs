using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_TooltipControl : MonoBehaviour
{
    public VRTK_ObjectTooltip m_objectTooltip = null;

    public float m_defaultDuration = float.PositiveInfinity;

    private float m_tooltipActivatedTime = 0;
    private float m_duration = float.PositiveInfinity;

    public void ShowTooltip(string text)
    {
        ShowTooltip(text, m_defaultDuration);
    }

    public void ShowTooltip(string text, float duration)
    {
        m_duration = duration > 0 ? duration : float.PositiveInfinity;
        m_objectTooltip.gameObject.SetActive(true);
        m_objectTooltip.UpdateText(text);
        m_tooltipActivatedTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > m_tooltipActivatedTime + m_duration)
        {
            HideTooltip();
        }
    }

    public void HideTooltip()
    {
        m_objectTooltip.gameObject.SetActive(false);
    }
}
