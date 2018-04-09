using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// Object that collects "energy" over time and upon reaching capacity, releases it to the output <see cref="EnergyValue"/>.
    /// </summary>
    public class EnergyAccumulator : MonoBehaviour
    {
        [Tooltip("How much “energy” this accumumulator will collect before releasing it to the output Energy Value.")]
        public float capacity = 2;
        [Tooltip("The Energy Value this accumulator will collect for.")]
        public EnergyValue output = null;
        [Tooltip("How fast the release happens.")]
        public float releaseSpeed = 1;
        [Tooltip("If true, the release will continue until empty, otherwise only capacity will be released.")]
        public bool releaseAll = false;
        [Tooltip("Release all when Energy Value rate reaches zero.")]
        public bool releaseWhenZeroRate = true;
        [Tooltip("The capacity will be adjusted based on timescale to keep the apparent accumulation speed constant.")]
        public bool adjustToTimeScale = false;

        public float realCapacity
        {
            get
            {
                float c = capacity;
                if (adjustToTimeScale)
                    c *= LocationHandle.current.timeScale;
                return c;
            }
        }

        public float capacityThreshold
        {
            get
            {
                return 0.1f * realCapacity;
            }
        }

        internal float currentValue = 0;
        internal float lastDelta = 0;

        private float toRelease = 0;

        /// <summary>
        /// Returns value that could not be accumulated.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float Accumulate(float value)
        {
            float newVal = currentValue + value;

            if (newVal < 0)
            {
                newVal = 0;
            }
            if (newVal > realCapacity)
            {
                newVal = realCapacity;
            }

            lastDelta = newVal - currentValue;
            currentValue = newVal;
            return value - lastDelta;
        }

        private void Update()
        {
            if (toRelease > 0)
            {
                float delta = releaseSpeed * LocationHandle.current.timeScale * Time.deltaTime;
                if (delta < lastDelta * 10)
                {
                    delta = Mathf.Abs(lastDelta) * 10;
                }

                delta = -delta;
                delta = delta - Accumulate(delta);

                toRelease += delta;
                output.value -= delta;

                if (releaseAll)
                    toRelease = currentValue;
                else if (toRelease <= 0)
                    toRelease = 0;
            }
            else
            {
                var c = realCapacity;
                if (releaseWhenZeroRate && output.targetRate == 0)
                    toRelease = currentValue;
                else if (currentValue >= c)
                    toRelease = c;
            }
        }
    }

}