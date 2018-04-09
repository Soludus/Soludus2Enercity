using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{
    [System.Serializable]
    public struct Location
    {
        public string name;
        public float latitude;
        public float longitude;
        public System.DateTimeOffset dateTimeOffset;
    }

    /// <summary>
    /// Contains the current location and time on earth and timescale.
    /// </summary>
    [CreateAssetMenu(menuName = "Soludus/Internal/LocationHandle")]
    public class LocationHandle : ScriptableObject
    {
        public static LocationHandle current = null;

        internal Location location;
        internal float timeScale = 1;

        public void LoadLocation(MapConfiguration map)
        {
            location.name = map.location.name;
            location.latitude = map.location.latitude;
            location.longitude = map.location.longitude;
            location.dateTimeOffset = new System.DateTimeOffset(map.beginDateTime, System.TimeSpan.FromHours(map.location.UTCOffsetHours));
            timeScale = map.timeScale;
        }
    }

}