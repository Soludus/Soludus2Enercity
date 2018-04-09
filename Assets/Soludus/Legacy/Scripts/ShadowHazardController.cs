using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component controls the movement of a shadow hazard to maximize the shadowing of the target.
/// </summary>
public class ShadowHazardController : MonoBehaviour
{
    public Hazard m_hazard;

    public float m_height = 2;

    /// <summary>
    /// The Hazard will no longer be efficent after it's this far from target.
    /// </summary>
    public float m_maxDistance = 10;

    public float m_maxSpeed = 5;

    public Transform m_sun;

    public bool m_moveToBlockSun = true;


    private Vector3 m_destination;

    private void OnEnable()
    {
        if (m_hazard == null)
            m_hazard = GetComponentInChildren<Hazard>();
        if (m_sun == null)
            m_sun = RenderSettings.sun.transform;
        Update();
    }

    private void Update()
    {
        if (m_hazard != null && m_hazard.target != null)
        {
            Vector3 d = m_moveToBlockSun ? -m_sun.forward : Vector3.up;
            Vector3 r = (m_height / d.y) * d;
            m_destination = m_hazard.target.transform.position + r;

            //Debug.DrawRay(target.position, Vector3.up * height, Color.blue);
            //Debug.DrawRay(target.position, r, Color.green);

            if ((m_destination - m_hazard.target.transform.position).sqrMagnitude > m_maxDistance * m_maxDistance)
                Destroy(gameObject);
            else
                transform.position = Vector3.Lerp(transform.position, m_destination, m_maxSpeed * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        m_hazard.spawner.OnHazardDestroyed(m_hazard);
    }

}
