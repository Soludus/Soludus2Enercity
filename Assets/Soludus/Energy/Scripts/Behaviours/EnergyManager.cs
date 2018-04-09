using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// <para>Updates the <see cref="EnergyValue"/>s based on the <see cref="EnergyDevice"/>s in the scene.</para>
    /// <para>Calculations use the location and time as reported by a <see cref="LocationHandle"/>.</para>
    /// <para>Each frame, collects all devices with same output and formula and passes them to the formula to obtain the change in the energy values.</para>
    /// </summary>
    public class EnergyManager : MonoBehaviour
    {
        [Tooltip("The set of devices to include.")]
        public EnergyDeviceSharedSet m_devices = null;
        [Tooltip("Handle to the current map. The same handle should be referenced everywhere.")]
        public MapHandle m_mapHandle = null;
        [Tooltip("Handle to the current location. The same handle should be referenced everywhere.")]
        public LocationHandle m_locationHandle = null;
        [Tooltip("If the time scale is large enough that one frame delta time is longer than this value, extra time step iterations will be performed each frame.")]
        public float m_maxEnergyTickLengthHours = 0.02f;

        private DateTimeOffset m_lastDateTime;
        private Dictionary<OutputFormula, List<EnergyEffect>> m_energyEffectsPerOutput = new Dictionary<OutputFormula, List<EnergyEffect>>();

        public struct OutputFormula : IEquatable<OutputFormula>
        {
            public EnergyValue output;
            public Formula formula;

            public bool Equals(OutputFormula other)
            {
                return Equals(output, other.output) && Equals(formula, other.formula);
            }

            public override int GetHashCode()
            {
                int hash = 1009;
                hash = (hash * 9176) + (output != null ? output.GetHashCode() : 0);
                hash = (hash * 9176) + (formula != null ? formula.GetHashCode() : 0);
                return hash;
            }
        }

        private List<MapConfiguration.EnergyTarget> energyTargets
        {
            get { return m_mapHandle.mapConfiguration.energyTargets; }
        }

        private void OnEnable()
        {
            m_lastDateTime = m_locationHandle.location.dateTimeOffset;
            for (int i = 0; i < energyTargets.Count; ++i)
            {
                energyTargets[i].energyValue.value = 0;
                energyTargets[i].energyValue.rate = 0;
                energyTargets[i].energyValue.targetValue = energyTargets[i].targetValue;
                energyTargets[i].energyValue.targetRate = 0;
                energyTargets[i].energyValue.startDateTimeOffset = m_lastDateTime;
            }
            Debug.Log("Energy initialized");
        }

        private void Update()
        {
            CollectEnergyEffects();
            UpdateEnergyValues();
        }

        private void CollectEnergyEffects()
        {
            foreach (var item in m_energyEffectsPerOutput)
            {
                item.Value.Clear();
            }
            for (int i = 0; i < m_devices.items.Count; ++i)
            {
                var dev = m_devices.items[i];
                if (!dev.active)
                    continue;

                // calculate multipler caused by device effects
                float deviceEffectMultiplier = 1;
                for (int j = 0; j < dev.deviceEffects.Count; ++j)
                {
                    if (!dev.deviceEffects[j].source.active)
                        continue;
                    deviceEffectMultiplier *= dev.deviceEffects[j].efficiencyMultiplier;
                }

                var accumulators = dev.slot != null ? dev.slot.accumulators : null;
                var targetAccumulators = dev.targetForEffects != null ? dev.targetForEffects.accumulators : null;
                var effects = dev.type.energyEffects;
                var size = (dev.grid != null ? dev.grid.area : 1) * dev.type.size;

                for (int j = 0; j < effects.Count; ++j)
                {
                    var of = new OutputFormula
                    {
                        output = effects[j].outputTo,
                        formula = effects[j].energyFormula
                    };

                    List<EnergyEffect> list;
                    if (!m_energyEffectsPerOutput.TryGetValue(of, out list))
                    {
                        m_energyEffectsPerOutput[of] = list = new List<EnergyEffect>();
                    }

                    EnergyAccumulator acc = null;
                    if (effects[j].tryUseTargetAccumulators && targetAccumulators != null)
                        acc = targetAccumulators.Find((a) => a.output == of.output);
                    if (acc == null && accumulators != null)
                        acc = accumulators.Find((a) => a.output == of.output);

                    var ef = new EnergyEffect
                    {
                        size = size,
                        efficiency = effects[j].efficiency * effects[j].magnitude,
                        deviceEffectMultiplier = deviceEffectMultiplier,
                        accumulator = acc
                    };

                    dev.lastEnergyEffect = ef;
                    if (dev.slot != null)
                        dev.slot.lastEnergyEffect = ef;

                    list.Add(ef);
                }
            }
        }


        public float GetValue(Formula formula, DateTimeOffset from, DateTimeOffset to, List<EnergyEffect> devices, Location location)
        {
            float deltaHours = (float)(to - from).TotalHours;

            if (deltaHours > 0)
            {
                int iters = (int)(deltaHours / m_maxEnergyTickLengthHours) + 1;
                const int maxIterations = 100000;

                if (iters > maxIterations)
                {
                    Debug.LogWarning("Delta time was " + deltaHours + " hours. " + iters + " iterations (step = " + 1f / iters * deltaHours + " hours) required to update collected output.\nClamped to " + maxIterations + " iterations (step = " + (1f / maxIterations * deltaHours) + " hours)");
                    iters = maxIterations;
                }

                float delta = 1f / iters * deltaHours;

                return Formula.GetValue(formula, from, iters, delta, devices, location);
            }

            return 0;
        }


        private void UpdateEnergyValues()
        {
            var location = m_locationHandle.location;

            for (int i = 0; i < energyTargets.Count; ++i)
            {
                energyTargets[i].energyValue.rate = 0;
                energyTargets[i].energyValue.targetRate = 0;
            }

            foreach (var item in m_energyEffectsPerOutput)
            {
                var of = item.Key;
                var devs = item.Value;
                if (devs.Count > 0)
                    of.output.value += GetValue(of.formula, m_lastDateTime, location.dateTimeOffset, devs, location);
                of.output.rate += Formula.GetRate(of.formula, devs, location);
                of.output.targetRate += Formula.GetRate(of.formula, devs, location, true);
            }

            m_lastDateTime = location.dateTimeOffset;
        }

    }

}