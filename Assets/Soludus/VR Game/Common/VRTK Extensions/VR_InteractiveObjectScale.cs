using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[DisallowMultipleComponent]
public class VR_InteractiveObjectScale : MonoBehaviour
{
    [System.Serializable]
    private class ScaleOptions
    {
        [SerializeField]
        public bool m_enabled = true;
        [SerializeField]
        public float m_scaleOffset = 1f;

        internal bool m_active = false;
    }

    [SerializeField]
    private VRTK_InteractableObject m_interactableObject = null;
    [SerializeField]
    private float m_normalScale = 1f;
    [SerializeField]
    private float m_scalingSpeed = 1f;

    [SerializeField]
    private ScaleOptions m_whenTouching = new ScaleOptions();
    [SerializeField]
    private ScaleOptions m_whenGrabbed = new ScaleOptions();
    [SerializeField]
    private ScaleOptions m_whenInSnapDropZone = new ScaleOptions();

    private Vector3 m_scaleVector = Vector3.one;
    private float m_currScale = 1;
    private float m_targetScale = 1;

    private void OnEnable()
    {
        if (m_interactableObject == null)
            m_interactableObject = GetComponent<VRTK_InteractableObject>();

        // NOTE: may cause problems
        m_scaleVector = m_interactableObject.transform.localScale;
        m_targetScale = m_normalScale;

        m_currScale = 0;

        m_interactableObject.transform.localScale = m_scaleVector * m_currScale;
    }

    private void OnDisable()
    {

    }

    private void UpdateScale(bool a, ScaleOptions o)
    {
        if (a)
        {
            if (!o.m_active)
                m_targetScale += o.m_scaleOffset;
            o.m_active = true;
        }
        else
        {
            if (o.m_active)
                m_targetScale -= o.m_scaleOffset;
            o.m_active = false;
        }
    }

    private bool IsTouching(VRTK_InteractableObject go)
    {
        var touchingObjs = go.GetTouchingObjects();
        bool touching = false;
        for (int i = 0; i < touchingObjs.Count; ++i)
        {
            if (!go.transform.IsChildOf(touchingObjs[i].transform))
                touching = true;
        }
        return touching;
    }

    private void Update()
    {
        bool inSnapDropZone = m_whenInSnapDropZone.m_enabled && m_interactableObject.IsInSnapDropZone();
        bool grabbed = m_whenGrabbed.m_enabled && m_interactableObject.IsGrabbed();
        bool touched = !grabbed && m_whenTouching.m_enabled && IsTouching(m_interactableObject);

        UpdateScale(inSnapDropZone, m_whenInSnapDropZone);
        UpdateScale(grabbed, m_whenGrabbed);
        UpdateScale(touched, m_whenTouching);

        //m_currScale = Mathf.Lerp(m_currScale, m_targetScale, Time.deltaTime);
        m_currScale = Mathf.MoveTowards(m_currScale, m_targetScale, Time.deltaTime * m_scalingSpeed);
        m_interactableObject.transform.localScale = m_scaleVector * m_currScale;
    }
}
