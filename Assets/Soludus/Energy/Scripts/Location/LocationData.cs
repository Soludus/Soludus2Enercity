using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Data describing a geographic location on earth.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Location", order = -1)]
    public class LocationData : ScriptableObject
    {
        [Header("Position")]
        public float latitude = 0;
        public float longitude = 0;
        [Header("Time zone")]
        [Tooltip("The time zone.")]
        public float UTCOffsetHours = 0;
    }

}