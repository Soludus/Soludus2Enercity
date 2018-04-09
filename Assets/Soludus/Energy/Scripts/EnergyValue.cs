using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// <para>Defines a value that will be simulated. E.g. Electricity.</para>
    /// <para>Has a <see cref="value"/> (e.g. kWh) and a <see cref="rate"/> (e.g. kW). The rate is calculated and value changed over time by the <see cref="EnergyManager"/>.</para>
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Energy Value", order = -1)]
    public class EnergyValue : EnergyType
    {
        [Tooltip("A unit to be displayed for the value.")]
        public string valueUnitString = "";
        [Tooltip("A unit to be displayed for the rate.")]
        public string rateUnitString = "";

        /// <summary>
        /// The current value.
        /// </summary>
        internal float value = 0;
        /// <summary>
        /// The current rate.
        /// </summary>
        internal float rate = 0;

        /// <summary>
        /// The value this value ideally moves towards.
        /// </summary>
        internal float targetValue = 0;
        /// <summary>
        /// The ideal achievable rate.
        /// </summary>
        internal float targetRate = 0;

        internal System.DateTimeOffset startDateTimeOffset;

        public System.TimeSpan elapsedTime
        {
            get
            {
                return LocationHandle.current.location.dateTimeOffset - startDateTimeOffset;
            }
        }

        public void Set(float v)
        {
            value = v;
            rate = 0;
        }

        public void Increment(float v)
        {
            value += v;
            rate = v;
        }
    }

}