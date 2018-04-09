using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// This formula returns a constant value over time equal to the device size.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Formula/Constant")]
    public class ConstantFormula : Formula
    {
        [Multiline]
        public string description = "Returns a constant value over time \nequal to the device size.";

        public override float GetRate(List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            return GetConstantRate(devices, location, ideal);
        }

        public override float GetValue(DateTimeOffset from, int iters, float delta, List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            return GetConstantValue(from, iters, delta, devices, location, ideal);
        }

        public static float GetConstantRate(List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            return GetNormalDeviceRate(1, devices, ideal);
        }

        public static float GetConstantValue(DateTimeOffset from, int iters, float delta, List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            return GetNormalDeviceValue(iters * delta, devices, ideal);
        }
    }

}