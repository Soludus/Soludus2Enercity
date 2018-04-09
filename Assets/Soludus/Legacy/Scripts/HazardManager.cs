using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates hazards to disrupt the function of solar panels.
/// </summary>
public class HazardManager : MonoBehaviour
{
    public int m_maxHazards = 10;
    public int m_maxPerTarget = 1; // a target should not have multiple hazards of the same type regardless of this value

    public float m_minHazardIntervalSeconds = 2;
    public float m_maxHazardIntervalSeconds = 8;

    // TODO: list of possible hazards to spawn with propabilities
    public List<Hazard> m_hazardPrefabs = new List<Hazard>();

    public SolarPowerGameManager m_gameManager = null;

    private List<Hazard> m_hazards = new List<Hazard>();
    private float m_nextSpawnTime;

    private void Awake()
    {
        if (m_gameManager == null)
            m_gameManager = GetComponent<SolarPowerGameManager>();
    }

    private void Start()
    {
        m_hazards.Clear();
    }

    private void Update()
    {
        if (Time.time >= m_nextSpawnTime)
        {
            TrySpawnHazard();
        }

        CheckPanelsToRetargetHazards();
    }

    public void OnHazardDestroyed(Hazard h)
    {
        h.target = null;
        m_hazards.Remove(h);
    }

    /// <summary>
    /// If output is null, count is still returned.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public int GetHazards(GameObject target, ICollection<Hazard> output)
    {
        int i = 0;
        foreach (var item in m_hazards)
        {
            if (item.target == target)
            {
                if (output != null)
                    output.Add(item);
                i++;
            }
        }
        return i;
    }

    public bool TrySpawnHazard()
    {
        if (m_hazards.Count < m_maxHazards)
        {
            if (m_hazardPrefabs.Count == 0)
            {
                Debug.LogError("No hazard prefabs.");
            }
            else
            {
                // get target
                GameObject target = null;
                foreach (var item in m_gameManager.panels)
                {
                    if (GetHazards(item.gameObject, null) < m_maxPerTarget)
                    {
                        target = item.gameObject;
                        break;
                    }
                }

                if (target == null)
                {
                    Debug.LogError("No targets or all targets have max hazards.");
                }
                else
                {
                    var hazard = Instantiate(GetHazardPrefab());
                    hazard.target = target;
                    hazard.spawner = this;

                    m_hazards.Add(hazard);
                    hazard.gameObject.SetActive(true);

                    m_nextSpawnTime = Time.time + Random.Range(m_minHazardIntervalSeconds, m_maxHazardIntervalSeconds);
                    return true;
                }

            }
        }
        return false;
    }

    // TODO: callback when a panel is disabled -> more optimized way possible
    private void CheckPanelsToRetargetHazards()
    {
        foreach (var h in m_hazards)
        {
            var currentPanel = h.target.GetComponent<SolarPanel>();
            if (!currentPanel.isActiveAndEnabled)
            {
                foreach (var panel in m_gameManager.panels)
                {
                    if (panel == currentPanel)
                        continue;
                    if (!panel.isActiveAndEnabled)
                        break;
                    int hCount = GetHazards(panel.gameObject, null);
                    if (hCount < m_maxPerTarget)
                    {
                        Debug.Log("Hazard retargeted to " + panel);
                        h.target = panel.gameObject;
                        break;
                    }
                }
            }
        }
    }

    private Hazard GetHazardPrefab()
    {
        return m_hazardPrefabs[Random.Range(0, m_hazardPrefabs.Count)];
    }
}
