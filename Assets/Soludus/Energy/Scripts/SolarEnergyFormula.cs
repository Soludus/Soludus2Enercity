using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// This formula returns a value based on the elevation angle of the sun at the location of the device.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Formula/Solar Energy")]
    public class SolarEnergyFormula : Formula
    {
        [Multiline]
        public string description = "The returned value depends on \nthe elevation angle of the sun \nat the location of the device.";

        public override float GetRate(List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            float solarIntensity = (float)SolarEnergy.SolarElevationAngleSine(location.dateTimeOffset.UtcDateTime, location.latitude, location.longitude);
            if (solarIntensity < 0)
                solarIntensity = 0;
            return GetNormalDeviceRate(solarIntensity, devices, ideal);
        }

        public override float GetValue(DateTimeOffset from, int iters, float delta, List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            double sinLat = System.Math.Sin(location.latitude * Mathf.Deg2Rad);
            double cosLat = System.Math.Cos(location.latitude * Mathf.Deg2Rad);
            double radLon = location.longitude * Mathf.Deg2Rad;

            double solarAccumulation = 0;

            DateTime begin = from.UtcDateTime;

            for (int i = 0; i < iters; ++i)
            {
                var date = begin.AddHours((i + 1) * delta);
                double solarIntensity = SolarEnergy.SolarElevationAngleSine(date.DayOfYear, date.TimeOfDay.TotalHours, sinLat, cosLat, radLon);
                if (solarIntensity > 0)
                {
                    solarAccumulation += solarIntensity;
                }
            }

            float p = (float)solarAccumulation * delta;
            return GetNormalDeviceValue(p, devices, ideal);
        }
    }

}