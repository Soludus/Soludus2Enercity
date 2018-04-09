using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the info of a hazard.
/// </summary>
public class Hazard : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    [SerializeField]
    private HazardManager m_spawner;

    public GameObject target
    {
        get { return m_target; }
        set
        {
            if (m_target != value)
            {
                if (m_target != null)
                {
                    var heh = m_target.GetComponent<IHazardEventHandler>();
                    if (heh != null)
                        heh.OnHazardRemoved(this);
                }

                m_target = value;

                if (m_target != null)
                {
                    var heh = m_target.GetComponent<IHazardEventHandler>();
                    if (heh != null)
                        heh.OnHazardReceived(this);
                }
            }
        }
    }

    public HazardManager spawner
    {
        get { return m_spawner; }
        set { m_spawner = value; }
    }
}
