using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Highlighters;

/// <summary>
/// Add a material to the renderer's material list.
/// </summary>
public class VR_MaterialAddHighlighter : VRTK_BaseHighlighter
{
    public Material highlightMaterial;

    private Renderer m_renderer;
    private Material m_prevHighlightMaterial;

    // FIXME: a problem with finding only the first Renderer is that if Highlight is called multiple times and the first renderer changes, the old will be left with the highlight material.

    public override void Highlight(Color? color = null, float duration = 0)
    {
        if (duration != 0)
            Debug.LogWarning(this + " highlight duration not implemented.");
        if (color != null)
        {
            m_renderer = GetComponentInChildren<Renderer>();

            var mats = new List<Material>(m_renderer.sharedMaterials);
            if (m_prevHighlightMaterial != null && m_prevHighlightMaterial != highlightMaterial)
                mats.Remove(m_prevHighlightMaterial);
            if (!mats.Contains(highlightMaterial))
                mats.Add(highlightMaterial);
            m_renderer.sharedMaterials = mats.ToArray();
            m_prevHighlightMaterial = highlightMaterial;
        }
    }

    public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
    {
        ResetHighlighter();
    }

    public override void ResetHighlighter()
    {
        //if (m_renderer == null)
        //	m_renderer = GetComponentInChildren<Renderer>();
        //m_prevHighlightMaterial = null;
    }

    public override void Unhighlight(Color? color = null, float duration = 0)
    {
        if (m_renderer != null)
        {
            if (m_prevHighlightMaterial != null)
            {
                var mats = new List<Material>(m_renderer.sharedMaterials);
                if (mats.Remove(m_prevHighlightMaterial))
                    m_renderer.sharedMaterials = mats.ToArray();
            }
        }
    }

}
