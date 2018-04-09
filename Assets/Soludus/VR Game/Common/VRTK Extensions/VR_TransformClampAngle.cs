using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjust the rotation of this transform to keep the angle between the defined vectors above minimum angle
/// </summary>
public class VR_TransformClampAngle : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_fromDirection = Vector3.up;
    [SerializeField]
    private Vector3 m_toLocalDirection = Vector3.forward;

    [SerializeField]
    private float m_minAngle = 100;

    public Vector3 fromDirection
    {
        get
        {
            return m_fromDirection;
        }

        set
        {
            m_fromDirection = value;
        }
    }

    public float minAngle
    {
        get
        {
            return m_minAngle;
        }

        set
        {
            m_minAngle = value;
        }
    }

    private void Update()
    {
        ClampAngle();
    }

    private void ClampAngle()
    {
        var toDirection = transform.rotation * m_toLocalDirection;
        var angle = Vector3.Angle(m_fromDirection, toDirection);
        if (angle < m_minAngle)
        {
            // set transform rotation so that angle between m_fromDirection and m_toLocalDirection is m_minAngle
            transform.rotation = Quaternion.AngleAxis(m_minAngle - angle, Vector3.Cross(m_fromDirection, toDirection)) * transform.rotation;
        }

        //Debug.DrawRay(transform.position, transform.rotation * m_toLocalDirection, Color.green);
        //Debug.Log("Clamp filtered transform angle " + Time.frameCount);
    }
}
