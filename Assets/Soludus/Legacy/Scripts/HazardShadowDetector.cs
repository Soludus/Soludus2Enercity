using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects a shadow when a hazard is targeting this object.
/// </summary>
public class HazardShadowDetector : ShadowDetectorBase, IHazardEventHandler
{
    private List<Hazard> m_hazards = new List<Hazard>();

    public override bool isUnderShadow
    {
        get
        {
            return m_hazards.Count > 0;
        }
    }

    public override float percentageInShadow
    {
        get
        {
            return m_hazards.Count > 0 ? 1 : 0;
        }
    }

    public override IEnumerable<GameObject> shadowCasters
    {
        get
        {
            foreach (var h in m_hazards)
            {
                yield return h.gameObject;
            }
        }
    }

    public void OnHazardReceived(Hazard hazard)
    {
        m_hazards.Add(hazard);
    }

    public void OnHazardRemoved(Hazard hazard)
    {
        m_hazards.Remove(hazard);
    }
}
