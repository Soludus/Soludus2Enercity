using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[DisallowMultipleComponent]
public class VR_ShrinkGrabbedObject : MonoBehaviour
{
    [System.Serializable]
    private class ShrinkOptions
    {
        [SerializeField]
        public bool m_enabled = true;
        [SerializeField]
        public float m_shrinkScale = 0.1f;
        [SerializeField]
        public float m_scalingDurationUp = 0;
        [SerializeField]
        public float m_scalingDurationDown = 0;

        internal bool m_active = false;
    }

    [SerializeField]
    private VRTK_InteractableObject m_interactableObject = null;
    [SerializeField]
    private float m_normalScale = 1f;
    [SerializeField]
    private ShrinkOptions m_whenGrabbed = new ShrinkOptions();
    [SerializeField]
    private ShrinkOptions m_whenInSnapDropZone = new ShrinkOptions();

    private Vector3 m_initialScale = Vector3.one;
    private float m_scalingT = 0;

    private void OnEnable()
    {
        if (m_interactableObject == null)
            m_interactableObject = GetComponent<VRTK_InteractableObject>();

        // NOTE: may cause problems
        //m_initialScale = m_interactableObject.transform.localScale.normalized / Mathf.Sqrt(3);
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        bool grabbed = m_whenGrabbed.m_enabled && m_interactableObject.IsGrabbed();
        bool inSnapDropZone = m_whenInSnapDropZone.m_enabled && m_interactableObject.IsInSnapDropZone();

        if (grabbed)
        {
            m_whenGrabbed.m_active = true;
            m_whenInSnapDropZone.m_active = false;
        }
        if (inSnapDropZone)
        {
            m_whenGrabbed.m_active = false;
            m_whenInSnapDropZone.m_active = true;
        }

        float scalingDurationUp = m_whenGrabbed.m_active ? m_whenGrabbed.m_scalingDurationUp : m_whenInSnapDropZone.m_active ? m_whenInSnapDropZone.m_scalingDurationUp : 0;
        float scalingDurationDown = m_whenGrabbed.m_active ? m_whenGrabbed.m_scalingDurationDown : m_whenInSnapDropZone.m_active ? m_whenInSnapDropZone.m_scalingDurationDown : 0;
        float shrinkScale = m_whenGrabbed.m_active ? m_whenGrabbed.m_shrinkScale : m_whenInSnapDropZone.m_active ? m_whenInSnapDropZone.m_shrinkScale : m_normalScale;

        if (grabbed || inSnapDropZone)
        {
            if (scalingDurationDown > 0)
            {
                m_scalingT += Time.deltaTime / scalingDurationDown;
            }
            else
            {
                m_scalingT = 1;
            }
        }
        else
        {
            if (scalingDurationUp > 0)
            {
                m_scalingT -= Time.deltaTime / scalingDurationUp;
            }
            else
            {
                m_scalingT = 0;
            }
        }
        m_scalingT = Mathf.Clamp01(m_scalingT);

        //m_interactableObject.transform.localScale = Vector3.Lerp(m_interactableObject.transform.localScale, targetScale, Time.deltaTime / m_scalingDuration);
        m_interactableObject.transform.localScale = Vector3.Lerp(m_normalScale * m_initialScale, shrinkScale * m_initialScale, m_scalingT);
    }
}
