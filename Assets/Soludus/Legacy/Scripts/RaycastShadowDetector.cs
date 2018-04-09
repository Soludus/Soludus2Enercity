using Soludus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Uses raycasts to detect if this object is under shadow.
/// </summary>
public class RaycastShadowDetector : ShadowDetectorBase
{
    [SerializeField]
    private Light m_light = null;

    [Tooltip("Select which points are raycasted from.")]
    [EnumFlags]
    [SerializeField]
    private RaycastMode m_raycastFrom = RaycastMode.TransformPosition;

    [Tooltip("These are the points that are raycasted from when Raycast Mode includes BoundaryPoints.")]
    [SerializeField]
    private List<Transform> m_boundaryPoints = new List<Transform>();

    [SerializeField]
    private BoundsProvider m_boundsProvider = null;

    [Tooltip("Layer mask for objects that are considered to cast shadows.")]
    [SerializeField]
    private LayerMask m_ShadowCasterMask = ~0;

    [Tooltip("Enable if you want to exclude objects that have shadow casting disabled in their renderer settings.")]
    [SerializeField]
    private bool m_checkIfShadowCaster = true;

    private HashSet<GameObject> m_shadowCasters = new HashSet<GameObject>();

    private Vector3[] m_boundsWorldCorners = new Vector3[8];

    private int m_rayCount = 0;
    private int m_rayHitCount = 0;

    public bool debug;

    [System.Flags]
    public enum RaycastMode
    {
        TransformPosition = 1 << 0,
        BoundaryPoints = 1 << 1,
        BoundsCorners = 1 << 2,
        BoundsCenter = 1 << 3
    }

    /// <summary>
    /// Is this object under shadow.
    /// </summary>
    public override bool isUnderShadow
    {
        get
        {
            return m_shadowCasters.Count == 0;
        }
    }

    /// <summary>
    /// Estimate how much of this object is in shadow.
    /// </summary>
    public override float percentageInShadow
    {
        get
        {
            if (m_rayCount == 0)
                return 0;
            return m_rayHitCount / (float)m_rayCount;
        }
    }

    /// <summary>
    /// The objects that currently cast a shadow to this object.
    /// </summary>
    public override IEnumerable<GameObject> shadowCasters
    {
        get
        {
            return m_shadowCasters;
        }
    }

    private void Reset()
    {
        FindDependencies();
    }

    private void OnEnable()
    {
        FindDependencies();
    }

    private void Update()
    {
        if (m_light != null)
        {
            int prevCasterCount = m_shadowCasters.Count;
            FindCasters();
            if (prevCasterCount == 0 && m_shadowCasters.Count > 0)
                OnEnterShadow();
            if (prevCasterCount > 0 && m_shadowCasters.Count == 0)
                OnExitShadow();
        }
    }

    private void FindDependencies()
    {
        if (m_light == null)
            m_light = RenderSettings.sun;
        if (m_boundsProvider == null)
            m_boundsProvider = GetComponentInChildren<BoundsProvider>();
    }

    private void FindCasters()
    {
        m_shadowCasters.Clear();
        m_rayCount = 0;
        m_rayHitCount = 0;

        if ((m_raycastFrom & RaycastMode.TransformPosition) != 0)
        {
            RaycastFindShadowCaster(transform.position);
        }
        if ((m_raycastFrom & RaycastMode.BoundaryPoints) != 0)
        {
            foreach (var p in m_boundaryPoints)
            {
                if (p != null)
                    RaycastFindShadowCaster(p.position);
            }
        }

        if ((m_raycastFrom & (RaycastMode.BoundsCenter | RaycastMode.BoundsCorners)) != 0)
        {
            if (m_boundsProvider == null)
            {
                Debug.LogWarning("Bounds provider not found. Disabled raycasting from bounds.", gameObject);
                m_raycastFrom &= ~(RaycastMode.BoundsCenter | RaycastMode.BoundsCorners);
            }
            else
            {
                if ((m_raycastFrom & RaycastMode.BoundsCenter) != 0)
                {
                    RaycastFindShadowCaster(m_boundsProvider.worldCenter);
                }
                if ((m_raycastFrom & RaycastMode.BoundsCorners) != 0)
                {
                    m_boundsProvider.GetWorldCorners(m_boundsWorldCorners);
                    for (int i = 0; i < m_boundsWorldCorners.Length; i++)
                    {
                        RaycastFindShadowCaster(m_boundsWorldCorners[i]);
                    }
                }
            }
        }
    }

    private void RaycastFindShadowCaster(Vector3 p)
    {
        var dir = GetRayDirection(p);
        if (debug) Debug.DrawRay(p, dir, Color.red);
        m_rayCount++;
        dir = dir.normalized;
        // repeat raycast until we don't hit this gameobject anymore
        RaycastHit hit;
        while (Physics.Raycast(p, dir, out hit, float.PositiveInfinity, m_ShadowCasterMask))
        {
            var go = hit.transform.gameObject;
            if (go != gameObject && !go.transform.IsChildOf(transform))
            {
                if (!m_shadowCasters.Contains(go))
                {
                    if (IsCasterValid(go))
                    {
                        m_rayHitCount++;
                        m_shadowCasters.Add(go);
                    }
                }
                else
                {
                    m_rayHitCount++;
                }
                break;
            }
            p = hit.point + dir * 0.01f;
        }
    }

    private bool IsCasterValid(GameObject caster)
    {
        if (m_checkIfShadowCaster)
        {
            var rend = caster.GetComponentInChildren<Renderer>();
            if (rend != null)
                return rend.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        return true;
    }

    private Vector3 GetRayDirection(Vector3 p)
    {
        if (m_light.type == LightType.Directional)
            return -m_light.transform.forward * 10000;
        else
            return m_light.transform.position - p;
    }

}
