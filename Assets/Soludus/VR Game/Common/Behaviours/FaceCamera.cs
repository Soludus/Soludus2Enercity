using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component ensures that the object is rotated to face the camera.
/// </summary>
public class FaceCamera : MonoBehaviour
{
    [Tooltip("If null, Camera.main is used.")]
    [SerializeField]
    private Transform m_camera;
    [SerializeField]
    private Mode m_mode = Mode.MatchRotation;
    [SerializeField]
    private bool m_flip = false;

    private Transform m_transform;

    public enum Mode
    {
        MatchRotation,
        LookAt
    }

    private void OnEnable()
    {
        m_transform = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        if (m_camera == null)
            m_camera = GetMainCamera();

        if (m_camera != null)
        {
            Vector3 forward;
            if (m_mode == Mode.MatchRotation)
                forward = m_camera.forward;
            else
                forward = m_transform.position - m_camera.position;

            if (m_flip)
                forward = -forward;
            m_transform.forward = forward;
        }
    }

    private Transform GetMainCamera()
    {
        var m = Camera.main;
        if (m != null)
            return m.transform;
        return null;
    }
}
