using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunTransform : MonoBehaviour
{
    public LocationHandle m_location = null;

    private void Update()
    {
        var location = m_location.location;
        double elevation, azimuth;
        SolarEnergy.SolarAngles(location.dateTimeOffset.UtcDateTime, location.latitude, location.longitude, out elevation, out azimuth);
        transform.eulerAngles = new Vector3((float)elevation, (float)azimuth);
    }
}
