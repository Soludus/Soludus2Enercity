using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class PunchDestroyable : MonoBehaviour
{
    public int m_requiredPunches = 5;
    public float m_minScale = 0.3f;
    public float m_scalingSpeed = 2f;
    public float m_minPunchInterval = 1;
    public float m_punchVelocityThreshold = 1;
    public Transform m_mesh;
    public ParticleSystem m_particlesOnDestroy = null;

    private int m_receivedPunches;
    private float m_nextPunchAvailableTime;
    private float m_startScale;
    private float m_currentScale;
    private float m_targetScale;
    private bool m_shrinking;

    private void OnEnable()
    {
        if (m_mesh == null)
        {
            var r = GetComponentInChildren<Renderer>();
            if (r != null)
                m_mesh = r.transform;
        }

        m_currentScale = m_targetScale = m_startScale = m_mesh.localScale.magnitude;

        GetComponent<VRTK_InteractableObject>().InteractableObjectTouched += OnVRTKTouch;
    }

    private void OnDisable()
    {
        GetComponent<VRTK_InteractableObject>().InteractableObjectTouched -= OnVRTKTouch;
    }

    private void OnVRTKTouch(object sender, InteractableObjectEventArgs e)
    {
        Vector3 velocity = new Vector3();
        var ve = e.interactingObject.GetComponent<VelocityEstimator>();
        if (ve != null)
            velocity = ve.velocity;

        //Debug.Log(e.interactingObject);
        //Debug.Log(velocity);

        Punch(e.interactingObject.transform.position, velocity);
    }

    public void Punch(Vector3 point, Vector3 velocity)
    {
        if (Time.time >= m_nextPunchAvailableTime && velocity.magnitude > m_punchVelocityThreshold)
        {
            m_receivedPunches++;
            m_targetScale = Mathf.Lerp(m_startScale, m_minScale, m_receivedPunches / (float)m_requiredPunches);
            m_shrinking = true;
            m_nextPunchAvailableTime = Time.time + m_minPunchInterval;
        }
    }

    private void Update()
    {
        if (m_shrinking)
        {
            if (Mathf.Abs(m_targetScale - m_currentScale) > 0.02f)
            {
                //m_currentScale = Mathf.Lerp(m_currentScale, m_targetScale, m_scalingSpeed * Time.deltaTime);
                m_currentScale = Mathf.MoveTowards(m_currentScale, m_targetScale, m_scalingSpeed * Time.deltaTime);

                Vector3 unitScale = m_mesh.localScale.normalized;
                m_mesh.localScale = m_currentScale * unitScale;
            }
            else
            {
                m_shrinking = false;
                if (m_receivedPunches >= m_requiredPunches)
                    Destroy();
            }
        }
    }

    private void Destroy()
    {
        var particles = Instantiate(m_particlesOnDestroy, transform.position, transform.rotation);
        particles.gameObject.SetActive(true);
        Destroy(particles.gameObject, 5);
        Destroy(gameObject);
    }
}
