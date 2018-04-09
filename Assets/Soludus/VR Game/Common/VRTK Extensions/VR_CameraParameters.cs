using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_CameraParameters : MonoBehaviour
{
    [SerializeField]
    private CameraClearFlags m_clearFlags = CameraClearFlags.Skybox;
    [SerializeField]
    private Color m_background = Color.black;
    [SerializeField]
    private LayerMask m_cullingMask = ~0;
    [SerializeField]
    private float m_nearClip = 0.1f;
    [SerializeField]
    private float m_farClip = 1000.0f;

    private Camera m_camera;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    private void OnEnable()
    {
        m_camera = VRTK_DeviceFinder.HeadsetCamera() != null ? VRTK_DeviceFinder.HeadsetCamera().GetComponentInChildren<Camera>() : null;
    }

    private void Update()
    {
        if (m_camera != null)
        {
            m_camera.clearFlags = m_clearFlags;
            m_camera.backgroundColor = m_background;
            m_camera.cullingMask = m_cullingMask;
            m_camera.nearClipPlane = m_nearClip;
            m_camera.farClipPlane = m_farClip;
        }
    }
}
