using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus
{

    /// <summary>
    /// A DateTime struct serializable by Unity and displayed in the inspector.
    /// </summary>
    [System.Serializable]
    public struct UDateTime
    {
        public int year;
        public int month;
        public int day;

        public int hour;
        public int minute;
        public float seconds;

        public static implicit operator System.DateTime(UDateTime a)
        {
            return new System.DateTime(a.year, a.month, a.day, a.hour, a.minute, 0).AddSeconds(a.seconds);
        }

        public static implicit operator UDateTime(System.DateTime a)
        {
            float secs = (float)a.TimeOfDay.TotalSeconds;
            secs -= Mathf.Floor(secs / 60) * 60;
            return new UDateTime { year = a.Year, month = a.Month, day = a.Day, hour = a.Hour, minute = a.Minute, seconds = secs };
        }
    }

}