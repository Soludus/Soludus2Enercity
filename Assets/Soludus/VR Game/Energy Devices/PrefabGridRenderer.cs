using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Render a grid of prefabs.
/// </summary>
[ExecuteInEditMode]
public class PrefabGridRenderer : GridRenderer
{
    [SerializeField]
    private GameObject m_gridUnitPrefab = null;

    // dirty checking
    private GameObject m_renderedPrefab = null;

    public GameObject gridUnitPrefab
    {
        get { return m_gridUnitPrefab; }
        set { m_gridUnitPrefab = value; }
    }

    protected override bool CanRender()
    {
        return base.CanRender() && m_gridUnitPrefab != null;
    }

    protected override bool DataDirty()
    {
        return
            base.DataDirty() ||
            m_renderedPrefab != m_gridUnitPrefab;
    }

    protected override bool ObjectsDirty()
    {
        return
            base.ObjectsDirty()
            || m_renderedPrefab != m_gridUnitPrefab;
    }

    protected override void RenderComplete()
    {
        base.RenderComplete();
        m_renderedPrefab = m_gridUnitPrefab;
    }

    protected override GameObject RenderUnit(GameObject go, Vector3 position, Transform container)
    {
        if (go == null)
        {
            go = Instantiate(m_gridUnitPrefab, container);
            go.hideFlags = HideFlags.DontSave;
        }

        go.transform.localPosition = position;
        if (m_material != null)
            go.GetComponent<Renderer>().sharedMaterial = m_material;
        //go.transform.localScale = new Vector3(m_size.x, 1, m_size.z);
        //go.transform.localRotation = Quaternion.identity;
        return go;
    }

}
