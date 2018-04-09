using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates the velocity of this object based on change in position.
/// </summary>
public class VelocityEstimator : MonoBehaviour
{
    private Transform m_transform;
    private Vector3 m_lastVelocity;
    private Vector3 m_lastDelta;
    private Vector3 m_lastPosition;

    public Vector3 velocity
    {
        get
        {
            return m_lastVelocity;
        }
    }

    public Vector3 delta
    {
        get
        {
            return m_lastDelta;
        }
    }

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Update()
    {
        var currentPosition = m_transform.position;

        m_lastDelta = currentPosition - m_lastPosition;
        m_lastVelocity = m_lastDelta / Time.deltaTime;
        m_lastPosition = currentPosition;
    }
}
