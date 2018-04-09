using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationText : MonoBehaviour
{
    [SerializeField]
    private Text m_text = null;

    public LocationHandle m_location = null;

    public bool date = false;
    public bool time = true;

    private void Update()
    {
        string text = null;
        var dateTime = m_location.location.dateTimeOffset;

        if (date)
            text += dateTime.ToString("d MMM ");
        if (time)
            text += dateTime.ToString("HH:mm");

        m_text.text = text;
    }
}
