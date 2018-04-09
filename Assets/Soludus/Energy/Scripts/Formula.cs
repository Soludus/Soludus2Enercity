using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Abstract base class for formulas taking the info of the active devices and location and returning the rate at a point in time or value accumulated over time.
    /// </summary>
    public abstract class Formula : ScriptableObject
    {
        /// <summary>
        /// Calculates a rate of change based on the device and location data.
        /// </summary>
        /// <param name="devices">The data of all the devices using this formula and outputting to the same energy value.</param>
        /// <param name="location">The data for the location the devices are in.</param>
        /// <param name="ideal">Ideal usually means that device multipliers and negative energy effects should not be counted. The <see cref="GetNormalDeviceRate(float, List{EnergyEffect}, bool)"/> method handles this automatically.</param>
        /// <returns>The calculated rate.</returns>
        public abstract float GetRate(List<EnergyEffect> devices, Location location, bool ideal = false);

        /// <summary>
        /// Calculates a change in value over a specified time period based on the device and location data.
        /// </summary>
        /// <param name="from">Last date and time.</param>
        /// <param name="iters">Number of iterations that should be performed. Simple way is to multiply delta by iterations if the formula does not need iterating.</param>
        /// <param name="delta">Delta time per one iteration in hours.</param>
        /// <param name="devices">The data of all the devices using this formula and outputting to the same energy value.</param>
        /// <param name="location">The data for the location the devices are in.</param>
        /// <param name="ideal">Ideal usually means that device multipliers and negative energy effects should not be counted. The <see cref="GetNormalDeviceValue(float, List{EnergyEffect}, bool)"/> method handles this automatically.</param>
        /// <returns>The calculated change in value.</returns>
        public abstract float GetValue(DateTimeOffset from, int iters, float delta, List<EnergyEffect> devices, Location location, bool ideal = false);


        public static float GetRate(Formula formula, List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            if (formula != null)
                return formula.GetRate(devices, location, ideal);
            return ConstantFormula.GetConstantRate(devices, location, ideal);
        }

        public static float GetValue(Formula formula, DateTimeOffset from, int iters, float delta, List<EnergyEffect> devices, Location location, bool ideal = false)
        {
            if (formula != null)
                return formula.GetValue(from, iters, delta, devices, location, ideal);
            return ConstantFormula.GetConstantValue(from, iters, delta, devices, location, ideal);
        }

        /// <summary>
        /// Most formulas can use this method unless they want a to handle the individual devices in a custom way.
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="devices"></param>
        /// <param name="ideal"></param>
        /// <returns></returns>
        public static float GetNormalDeviceRate(float baseValue, List<EnergyEffect> devices, bool ideal)
        {
            float a = 0;
            for (int i = 0; i < devices.Count; ++i)
            {
                float f = devices[i].size * devices[i].efficiency;

                if (ideal)
                {
                    if (f < 0)
                        continue;
                }
                else
                {
                    f *= devices[i].deviceEffectMultiplier;
                }

                a += f;
            }
            return a * baseValue;
        }

        /// <summary>
        /// Most formulas can use this method unless they want a to handle the individual devices in a custom way.
        /// Using this method enables the use of accumulators.
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="devices"></param>
        /// <param name="ideal"></param>
        /// <returns></returns>
        public static float GetNormalDeviceValue(float baseValue, List<EnergyEffect> devices, bool ideal)
        {
            float a = 0;

            for (int i = 0; i < devices.Count; ++i)
            {
                var f = baseValue * devices[i].size * devices[i].efficiency;

                if (ideal)
                {
                    if (f < 0)
                        continue;
                }
                else
                {
                    f *= devices[i].deviceEffectMultiplier;
                }

                if (devices[i].accumulator != null)
                    a += devices[i].accumulator.Accumulate(f);
                else
                    a += f;
            }

            return a;
        }
    }

}