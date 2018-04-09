using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolarPanelIndicator : MonoBehaviour
{
    [SerializeField]
    private SolarPanel m_panel;

    [SerializeField]
    private Slider m_slider;
    [SerializeField]
    private Text m_text;
    [SerializeField]
    private string m_textFormat = "{0}/{1}";

    private void Update()
    {
        if (m_panel != null)
        {
            m_slider.value = m_panel.power / m_panel.maxPower;
            m_text.text = string.Format(m_textFormat, m_panel.power, m_panel.maxPower);
        }
    }
}
