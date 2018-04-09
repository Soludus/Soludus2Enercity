using System;
using UnityEngine;

namespace Soludus
{

    /// <summary>
    /// Draws a Vector2 as MinMaxSlider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float max;
        public readonly float min;

        public MinMaxSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

}