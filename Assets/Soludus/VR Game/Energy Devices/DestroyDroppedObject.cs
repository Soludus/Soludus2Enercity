using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DestroyDroppedObject : MonoBehaviour
{
    public float m_time = 5;
    public int m_afterCollisions = 1;
    public bool m_deactivateWhenAttached = true;

    private float m_timer = 0;
    private int m_collisions = 0;

    private VRTK_InteractableObject m_interactableObj = null;
    private EnergyDevice m_device;

    private void OnEnable()
    {
        m_interactableObj = GetComponent<VRTK_InteractableObject>();
        m_device = GetComponent<EnergyDevice>();
        m_timer = 0;
        m_collisions = 0;
    }

    private void Update()
    {
        bool timerOn = m_interactableObj != null && !m_interactableObj.IsGrabbed() && !m_interactableObj.IsInSnapDropZone();
        if (m_device != null)
        {
            if (m_deactivateWhenAttached && m_device.slot != null)
            {
                enabled = false;
            }
            timerOn &= m_device.slot == null;
        }

        if (!timerOn)
        {
            m_collisions = 0;
        }

        if (timerOn && m_collisions >= m_afterCollisions)
        {
            m_timer += Time.deltaTime;
        }
        else
        {
            m_timer = 0;
        }
        if (m_timer > m_time)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ++m_collisions;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Water Trigger")
        {
            ++m_collisions;
        }
    }
}
