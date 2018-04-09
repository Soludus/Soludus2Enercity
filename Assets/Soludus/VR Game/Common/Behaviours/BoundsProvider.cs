using Soludus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component calculates the bounds for a GameObject.
/// </summary>
public class BoundsProvider : MonoBehaviour
{
    [EnumFlags]
    [SerializeField]
    private GetBoundsMode m_includedBounds = GetBoundsMode.Colliders;

    [Tooltip("Enable to find and combine all bounds in this gameobject and it's children.")]
    [SerializeField]
    private bool m_combineAll = false;

    [Tooltip("Enable to update local bounds each frame")]
    [SerializeField]
    private bool m_updateLocalBounds = false;

    private Bounds m_localBounds;

    private List<Collider> m_colliders = new List<Collider>();
    private List<Renderer> m_renderers = new List<Renderer>();

    /// <summary>
    /// Get the cached bounds of the object aligned in local space.
    /// </summary>
    public Bounds localBounds
    {
        get { return m_localBounds; }
    }

    /// <summary>
    /// Calculate the bounds of the object aligned in world space. This is ensured to return up to date bounds.
    /// </summary>
    public Bounds worldBounds
    {
        get { return GetWorldBounds(); }
    }

    /// <summary>
    /// localBounds.center transformed to world space.
    /// </summary>
    public Vector3 worldCenter
    {
        get { return transform.TransformPoint(m_localBounds.center); }
    }

    /// <summary>
    /// Fill the array with local bounds corners. Size must be >= 8.
    /// </summary>
    /// <param name="c"></param>
    public void GetLocalCorners(Vector3[] c)
    {
        var min = m_localBounds.min;
        var max = m_localBounds.max;
        c[0] = min;
        c[1] = max;
        c[2] = new Vector3(min.x, min.y, max.z);
        c[3] = new Vector3(min.x, max.y, min.z);
        c[4] = new Vector3(min.x, max.y, max.z);
        c[5] = new Vector3(max.x, min.y, min.z);
        c[6] = new Vector3(max.x, min.y, max.z);
        c[7] = new Vector3(max.x, max.y, min.z);
    }

    /// <summary>
    /// Fill the array with local bounds corners transformed to world space. Size must be >= 8.
    /// </summary>
    /// <param name="c"></param>
    public void GetWorldCorners(Vector3[] c)
    {
        GetLocalCorners(c);
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = transform.TransformPoint(c[i]);
        }
    }

    private void Start()
    {
        UpdateLocalBounds();
    }

    private void Update()
    {
        if (m_updateLocalBounds)
            UpdateLocalBounds();
    }

    /// <summary>
    /// Update the cached local bounds. This method temporarily changes the object's position and rotation. Should only be called if collider, mesh or children have changed.
    /// </summary>
    public void UpdateLocalBounds()
    {
        m_localBounds = GetLocalBounds();
    }

    private Bounds GetWorldBounds(bool ignoreTriggerColliders = true, int layerMask = ~0)
    {
        Bounds bounds = new Bounds();
        if ((m_includedBounds & GetBoundsMode.Colliders) != 0)
        {
            m_colliders.Clear();
            if (m_combineAll)
            {
                GetComponentsInChildren(m_colliders);
            }
            else
            {
                var col = GetComponentInChildren<Collider>();
                if (col != null) m_colliders.Add(col);
            }
            foreach (var col in m_colliders)
            {
                if (ignoreTriggerColliders && col.isTrigger) continue;
                if ((1 << col.gameObject.layer & layerMask) == 0) continue;
                if (bounds.extents == Vector3.zero)
                    bounds = col.bounds;
                bounds.Encapsulate(col.bounds);
            }
        }
        if ((m_includedBounds & GetBoundsMode.Renderers) != 0)
        {
            m_renderers.Clear();
            if (m_combineAll)
            {
                GetComponentsInChildren(m_renderers);
            }
            else
            {
                var col = GetComponentInChildren<Renderer>();
                if (col != null) m_renderers.Add(col);
            }
            foreach (var rend in m_renderers)
            {
                if ((1 << rend.gameObject.layer & layerMask) == 0) continue;
                if (bounds.extents == Vector3.zero)
                    bounds = rend.bounds;
                bounds.Encapsulate(rend.bounds);
            }
        }
        return bounds;
    }

    private Bounds GetLocalBounds(bool ignoreTriggerColliders = true, int layerMask = ~0)
    {
        var t = transform;
        var oPos = t.position;
        var oRot = t.rotation;
        //var oScale = t.localScale;
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        //t.localScale = Vector3.one;

        var b = GetWorldBounds(ignoreTriggerColliders, layerMask);

        t.position = oPos;
        t.rotation = oRot;
        //t.localScale = oScale;
        return b;
    }


    /// <summary>
    /// Mode for BoundsProvider.GetBounds().
    /// </summary>
    public enum GetBoundsMode
    {
        Colliders = 1 << 0,
        Renderers = 1 << 1
    }

    /// <summary>
    /// Get the bounds of the GameObject using collider and renderer bounds in all children. Avoid using this method heavily.
    /// Renderers like ParticleSystems may cause undesired results.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="mode"></param>
    /// <param name="ignoreTriggerColliders"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static Bounds GetBounds(GameObject obj, GetBoundsMode mode = GetBoundsMode.Colliders, bool ignoreTriggerColliders = true, int layerMask = ~0)
    {
        Bounds bounds = new Bounds();
        if ((mode & GetBoundsMode.Colliders) != 0)
        {
            var cols = obj.GetComponentsInChildren<Collider>();
            foreach (var col in cols)
            {
                if (ignoreTriggerColliders && col.isTrigger) continue;
                if ((1 << col.gameObject.layer & layerMask) == 0) continue;
                if (bounds.extents == Vector3.zero)
                    bounds = col.bounds;
                bounds.Encapsulate(col.bounds);
            }
        }
        if ((mode & GetBoundsMode.Renderers) != 0)
        {
            var rends = obj.GetComponentsInChildren<Renderer>();
            foreach (var rend in rends)
            {
                if ((1 << rend.gameObject.layer & layerMask) == 0) continue;
                if (bounds.extents == Vector3.zero)
                    bounds = rend.bounds;
                bounds.Encapsulate(rend.bounds);
            }
        }
        return bounds;
    }

    /// <summary>
    /// Like BoundsProvider.GetBounds() but rotation, position or local scale won't affect the bounds.
    /// (The transform of the object is modified temporarily)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="mode"></param>
    /// <param name="ignoreTriggerColliders"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static Bounds GetLocalBounds(GameObject obj, GetBoundsMode mode = GetBoundsMode.Colliders, bool ignoreTriggerColliders = true, int layerMask = ~0)
    {
        var t = obj.transform;
        var oPos = t.position;
        var oRot = t.rotation;
        var oScale = t.localScale;
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;

        var b = GetBounds(obj, mode, ignoreTriggerColliders, layerMask);

        t.position = oPos;
        t.rotation = oRot;
        t.localScale = oScale;
        return b;
    }
}
