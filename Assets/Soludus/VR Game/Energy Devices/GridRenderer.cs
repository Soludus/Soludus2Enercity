using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Render a Grid. Rendering each unit is defined in a subclass.
/// </summary>
[ExecuteInEditMode]
public abstract class GridRenderer : MonoBehaviour
{
    [SerializeField]
    protected bool m_centered = true;
    [SerializeField]
    protected bool m_meshEnabled = true;
    [SerializeField]
    protected bool m_colliderEnabled = true;
    [SerializeField]
    protected Material m_material = null;

    private GridData m_gridData = null;
    private Transform m_meshContainer = null;

    // dirty checking fields
    private int m_renderedWidth = 0;
    private int m_renderedHeight = 0;
    private Vector3 m_renderedUnitSize = Vector3.zero;
    private bool m_renderedCentered = false;
    private bool m_renderedMesh = false;
    private bool m_renderedCollider = false;
    private Material m_renderedMaterial = null;

    private List<GameObject> m_renderedObjects = new List<GameObject>();

    public bool centered
    {
        get { return m_centered; }
        set { m_centered = value; }
    }

    public bool meshEnabled
    {
        get { return m_meshEnabled; }
        set { m_meshEnabled = value; }
    }

    public bool colliderEnabled
    {
        get { return m_colliderEnabled; }
        set { m_colliderEnabled = value; }
    }

    public Material material
    {
        get { return m_material; }
        set { m_material = value; }
    }

    public GridData gridData
    {
        get { return m_gridData; }
    }

    protected virtual void OnEnable()
    {
        //Debug.Log("render enable");
        FindGridData();
        Render();
        UpdateChildLayer();
    }

    protected virtual void OnDisable()
    {
        //Debug.Log("render disable");
        UnRender(true);
    }

    protected virtual void Update()
    {
        FindGridData();
        if (Dirty())
        {
            //Debug.Log("render");
            if (ObjectsDirty())
                UnRender(false);
            if (Render())
                RenderComplete();
        }
        UpdateChildLayer();
    }

    private void UpdateChildLayer()
    {
        if (m_meshContainer != null)
        {
            m_meshContainer.gameObject.layer = gameObject.layer;
        }
    }

    private void FindGridData()
    {
        if (m_gridData == null)
        {
            m_gridData = GetGridData();
        }
    }

    private bool Dirty()
    {
        if (m_meshContainer == null)
            return CanRender();
        if (m_gridData == null)
            return true;
        return DataDirty();
    }

    protected virtual GridData GetGridData()
    {
        return GetComponent<GridData>();
    }

    protected virtual bool CanRender()
    {
        return m_gridData != null;
    }

    protected virtual bool DataDirty()
    {
        return
            m_renderedWidth != m_gridData.width
            || m_renderedHeight != m_gridData.height
            || m_renderedUnitSize != m_gridData.unitSize
            || m_renderedMesh != m_meshEnabled
            || m_renderedCollider != m_colliderEnabled
            || m_renderedMaterial != m_material
            || m_renderedCentered != m_centered;
    }

    protected virtual bool ObjectsDirty()
    {
        return
            m_renderedWidth != m_gridData.width
            || m_renderedHeight != m_gridData.height;
    }

    public bool Render()
    {
        if (CanRender())
        {
            if (m_meshContainer == null)
            {
                m_meshContainer = transform.Find("[" + GetType().Name + "] [Instantiated Meshes]");
            }
            if (m_meshContainer == null)
            {
                m_meshContainer = new GameObject("[" + GetType().Name + "] [Instantiated Meshes]") { hideFlags = HideFlags.DontSave }.transform;
                m_meshContainer.SetParent(transform, false);
            }

            Vector3 unitSize = m_gridData.unitSize;
            Vector3 panelHalfOffset = new Vector3((m_gridData.width - 1) * unitSize.x * 0.5f, 0, (m_gridData.height - 1) * unitSize.z * 0.5f);
            Vector3 unitOffset = m_centered ? panelHalfOffset : Vector3.zero;

            if (m_meshEnabled)
            {
                for (int ix = 0; ix < m_gridData.width; ++ix)
                {
                    for (int iz = 0; iz < m_gridData.height; ++iz)
                    {
                        var i = ix * m_gridData.height + iz;
                        while (m_renderedObjects.Count <= i) m_renderedObjects.Add(null);
                        var go = m_renderedObjects[i];
                        go = RenderUnit(go, new Vector3(ix * unitSize.x - unitOffset.x, 0, iz * unitSize.z - unitOffset.z), m_meshContainer);
                        m_gridData.SetElementObject(ix, iz, go);
                        m_renderedObjects[i] = go;
                    }
                }
            }

            var col = m_meshContainer.GetComponent<BoxCollider>();
            if (m_colliderEnabled)
            {
                if (col == null)
                    col = m_meshContainer.gameObject.AddComponent<BoxCollider>();
                col.size = m_gridData.size;
                col.center = m_centered ? Vector3.zero : panelHalfOffset;
            }
            else
            {
                if (col != null)
                    DestroyImmediate(col);
            }

            return true;
        }
        return false;
    }

    protected virtual void RenderComplete()
    {
        m_renderedWidth = m_gridData.width;
        m_renderedHeight = m_gridData.height;
        m_renderedUnitSize = m_gridData.unitSize;
        m_renderedMesh = m_meshEnabled;
        m_renderedCollider = m_colliderEnabled;
        m_renderedMaterial = m_material;
        m_renderedCentered = m_centered;
    }

    protected abstract GameObject RenderUnit(GameObject go, Vector3 position, Transform container);

    public virtual void UnRender(bool full)
    {
        if (m_meshContainer != null)
        {
            for (int i = m_meshContainer.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(m_meshContainer.GetChild(i).gameObject);
            }

            m_renderedObjects.Clear();

            if (full)
            {
                if (m_meshContainer.GetComponent<BoxCollider>() != null)
                    DestroyImmediate(m_meshContainer.GetComponent<BoxCollider>());

                // CRASHES 2017.1.1p1 e.g when removing a component and undoing
                //DestroyImmediate(m_meshContainer.gameObject);
                //m_meshContainer = null;
            }
        }
    }
}
