using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    /// <summary>
    /// <para>Optional manager for spawning specific devices randomly over time and assigning a random device slot as their target.</para>
    /// <para>Spawning parameters are defined in a <see cref="HazardConfiguration"/> which is assigned to the <see cref="MapConfiguration.hazards"/> field.</para>
    /// </summary>
    public class DeviceHazardManager : MonoBehaviour
    {
        [Tooltip("Handle to the current map. The same handle should be referenced everywhere.")]
        public MapHandle m_mapHandle = null;
        [Tooltip("The set of slots to consider as targets.")]
        public EnergyDeviceSlotSharedSet m_deviceSlots = null;

        private float m_time = 0;
        private float m_nextSpawnTime = 0;

        private HazardConfiguration hazardConfig
        {
            get { return m_mapHandle.mapConfiguration.hazards; }
        }

        public int totalHazardCount
        {
            get
            {
                int c = 0;
                var haz = hazardConfig.spawnedHazards;
                for (int i = 0; i < haz.Count; ++i)
                {
                    c += haz[i].type.allDevices.Count;
                }
                return c;
            }
        }

        private void Update()
        {
            if (hazardConfig.hazardsEnabled)
            {
                if (m_time >= m_nextSpawnTime)
                {
                    TrySpawnHazard();
                }
                m_time += Time.deltaTime * hazardConfig.spawnSpeedScale;
            }
        }

        private void AddHazard(EnergyDevice hazard, EnergyDeviceSlot target)
        {
            hazard.SetTarget(target);
            hazard.gameObject.SetActive(true);
        }

        private void RemoveHazard(EnergyDevice hazard)
        {
            hazard.gameObject.SetActive(false);
            hazard.SetTarget(null);
        }

        /// <summary>
        /// Selects a valid target from all Energy Device Slots.
        /// </summary>
        /// <returns></returns>
        public EnergyDeviceSlot GetValidTarget()
        {
            List<EnergyDeviceSlot> validTargets = new List<EnergyDeviceSlot>();

            for (int i = 0; i < m_deviceSlots.items.Count; ++i)
            {
                var item = m_deviceSlots.items[i];
                if (item.targetedBy == null && item.deviceEffects.Count < item.acceptedType.maxReceivedEffects && item.HasActiveDevices() && Time.time > item.lastEffectTime + hazardConfig.targetCooldown / hazardConfig.spawnSpeedScale)
                {
                    validTargets.Add(item);
                }
            }

            return validTargets.Count > 0 ? validTargets[Random.Range(0, validTargets.Count)] : null;
        }

        /// <summary>
        /// Spawns a hazard with the specified <paramref name="type"/> and sets it's target to <paramref name="target"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public void SpawnHazardOnTarget(EnergyDeviceType type, EnergyDeviceSlot target)
        {
            GameObject hazardGo;
            if (type.model != null)
            {
                var modelActive = type.model.activeSelf;
                type.model.SetActive(false);

                hazardGo = Instantiate(type.model);

                type.model.SetActive(modelActive);
            }
            else
            {
                hazardGo = new GameObject(type.name);
                hazardGo.SetActive(false);
            }

            var hazard = hazardGo.GetOrAddComponent<EnergyDevice>();
            hazardGo.transform.position = target.transform.position + new Vector3(0, 60, 0);
            hazard.type = type;

            AddHazard(hazard, target);
        }

        /// <summary>
        /// If a random target can be found, spawns a hazard with the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TrySpawnHazardOnRandomTarget(EnergyDeviceType type)
        {
            var target = GetValidTarget();
            if (target == null)
                return false;
            SpawnHazardOnTarget(type, target);
            return true;
        }

        private bool TrySpawnHazard()
        {
            if (totalHazardCount < hazardConfig.maxHazardCount)
            {
                var target = GetValidTarget();

                if (target != null)
                {
                    var hazards = FilterHazards(target.acceptedType, hazardConfig.spawnedHazards);
                    var hType = SelectRandomHazard(hazards);
                    if (hType >= 0)
                    {
                        SpawnHazardOnTarget(hazards[hType].type, target);

                        var interval = Random.Range(hazardConfig.spawnIntervalRange.x, hazardConfig.spawnIntervalRange.y);
                        m_nextSpawnTime = m_time + interval;
                        return true;
                    }
                }
                //else
                //{
                //    Debug.LogWarning("No targets or all targets have max hazards.");
                //}
            }
            return false;
        }

        private List<HazardConfiguration.DeviceHazard> FilterHazards(EnergyDeviceType targetType, List<HazardConfiguration.DeviceHazard> hazards)
        {
            var filtered = new List<HazardConfiguration.DeviceHazard>();
            for (int i = 0; i < hazards.Count; ++i)
            {
                if (hazards[i].type.enabled &&
                    (hazards[i].maxCount < 0 || hazards[i].type.allDevices.Count < hazards[i].maxCount) &&
                    hazards[i].type.GetEffectForTarget(targetType) != null)
                    filtered.Add(hazards[i]);
            }
            return filtered;
        }

        private int SelectRandomHazard(List<HazardConfiguration.DeviceHazard> hazards)
        {
            float max = 0;
            for (int i = 0; i < hazards.Count; ++i)
            {
                max += hazards[i].chance;
            }

            if (max <= 0)
            {
                return -1;
            }

            float rand = Random.Range(0, max - float.Epsilon);
            float pos = 0;

            for (int i = 0; i < hazards.Count; ++i)
            {
                if (rand >= pos & rand < (pos += hazards[i].chance))
                {
                    return i;
                }
            }

            throw new System.InvalidOperationException("Invalid data for selecting a random hazard.");
        }
    }

}