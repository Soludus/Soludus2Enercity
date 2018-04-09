using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerPixelPerUnitDistanceAdjust : MonoBehaviour
{
    [Tooltip("If null, Camera.main is used.")]
    public Transform m_camera;
    public CanvasScaler m_canvasScaler = null;

    public float m_scale = 10.0f;
    public float m_min = 0.05f;
    public float m_max = 10.0f;

    public float m_distancePrecision = 1;

    private void Update()
    {
        if (m_canvasScaler == null)
            m_canvasScaler = GetComponent<CanvasScaler>();
        if (m_camera == null)
            m_camera = GetMainCamera();

        if (m_camera != null)
        {
            float distance = Vector3.Distance(m_camera.position, transform.position);
            distance = Mathf.Round(distance * m_distancePrecision) / m_distancePrecision;
            m_canvasScaler.dynamicPixelsPerUnit = Mathf.Clamp(m_scale / distance, m_min, m_max);
        }
        else
        {
            m_canvasScaler.dynamicPixelsPerUnit = 1.0f;
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
