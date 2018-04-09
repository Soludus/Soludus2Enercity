using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Updates a <see cref="LocationHandle"/> based on a <see cref="MapConfiguration"/> and current timescale.
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        [Tooltip("Handle to the current map. The same handle should be referenced everywhere.")]
        public MapHandle m_mapHandle = null;
        [Tooltip("Handle to the current location. The same handle should be referenced everywhere.")]
        public LocationHandle m_locationHandle = null;
        [Tooltip("If checked, game time will be set to current local time at start instead of the Begin Date Time set in Map Configuration.")]
        public bool m_setToLocalTimeOnAwake = true;

        private void Awake()
        {
            LocationHandle.current = m_locationHandle;
            m_locationHandle.LoadLocation(m_mapHandle.mapConfiguration);
            if (m_setToLocalTimeOnAwake)
            {
                m_locationHandle.location.dateTimeOffset = new System.DateTimeOffset(new System.DateTime(System.DateTime.UtcNow.Ticks, System.DateTimeKind.Unspecified), System.TimeSpan.FromHours(m_mapHandle.mapConfiguration.location.UTCOffsetHours));
            }
            Debug.Log("Location initialized");
        }

        private void Update()
        {
            LocationHandle.current = m_locationHandle;

            if (m_locationHandle.timeScale != 0)
            {
                m_locationHandle.location.dateTimeOffset = m_locationHandle.location.dateTimeOffset.AddSeconds(Time.deltaTime * m_locationHandle.timeScale);
            }
        }
    }

}