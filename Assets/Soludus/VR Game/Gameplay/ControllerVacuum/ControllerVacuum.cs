using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerVacuum : MonoBehaviour
{
    public VRTK_ControllerEvents m_controllerEvents = null;

    [Header("Vacuum")]
    public float m_force = 1;
    public float m_garbageSuckSpeed = 1;
    public float m_range = 100;
    public float m_vacuumWidth = 5;
    public float m_suckCutoffAngle = 30;
    public LayerMask m_ignoredLayers = 0;

    [Header("Particles")]
    public ParticleSucker m_particleSucker = null;
    public ParticleSystem m_windParticles = null;
    public float m_windMoveSpeed = 10;

    [Header("Haptics")]
    [Range(0, 1)]
    public float m_hapticStrenght = 0.1f;
    [Range(0.001f, 0.1f)]
    public float m_hapticInterval = 0.01f;
    [Range(0, 1)]
    public float m_bigHapticStrenght = 1.0f;
    [Range(0, 10)]
    public float m_bigHapticChance = 4.0f;

    [Header("Sound")]
    public FMODEmitterProxy m_soundEmitter = null;
    public string m_garbageParamName = "";
    
    private bool m_suckingGarbage = false;

    private VRTK_ControllerReference m_controller = null;
    private List<Rigidbody> m_suckedRigidbodies = new List<Rigidbody>();
    private List<GarbageData> m_suckedGarbages = new List<GarbageData>();

    private static RaycastHit[] hitBuffer = new RaycastHit[128];

    private void OnEnable()
    {
        if (m_controllerEvents == null)
            m_controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();

        Activate();
    }

    private void OnDisable()
    {
        for (int i = 0; i < m_suckedGarbages.Count; ++i)
        {
            DeactivateOnGarbage(m_suckedGarbages[i]);
        }

        DeactivateGarbageEffects();
        Deactivate();
    }

    private bool FindAimedObject(out RaycastHit hit)
    {
        return Physics.Raycast(transform.position, transform.forward, out hit, m_range, ~m_ignoredLayers);
    }

    private GarbageData FindGarbage(GameObject go)
    {
        var gd = go.GetComponent<GarbageData>();
        if (gd != null && gd.garbageAmount > 0)
        {
            //Debug.Log("Found garbage");
            return gd;
        }
        return null;
    }

    private void Activate()
    {
        m_controller = VRTK_ControllerReference.GetControllerReference(m_controllerEvents.gameObject);
        VRTK_ControllerHaptics.TriggerHapticPulse(m_controller, m_hapticStrenght, float.PositiveInfinity, m_hapticInterval);

        //m_windParticles.transform.SetParent(null, false);
        m_soundEmitter.Play();
    }

    private void Deactivate()
    {
        m_suckedRigidbodies.Clear();
        m_suckedGarbages.Clear();
        if (m_controller != null)
        {
            VRTK_ControllerHaptics.CancelHapticPulse(m_controller);
        }
        m_windParticles.FastStop(true, 0, 1);
        m_soundEmitter.Stop();
    }

    private void ActivateOnGarbage(GarbageData gd)
    {
        var ps = gd.GetComponentInChildren<ParticleSystem>();
        gd.pullParticles = ps;
        m_particleSucker.m_particleSystems.Add(ps);
        ps.Play();
        var psm = ps.main;
        psm.gravityModifier = 0;
    }

    private void DeactivateOnGarbage(GarbageData gd)
    {
        var ps = gd.pullParticles;
        if (ps != null)
        {
            var psm = ps.main;
            psm.gravityModifier = 1;

            ps.Stop();
            m_particleSucker.m_particleSystems.Remove(ps);
        }
    }

    private void ActivateGarbageEffects()
    {
        m_particleSucker.enabled = true;
        m_soundEmitter.SetParameter(m_garbageParamName, 1);
        m_suckingGarbage = true;

        VRTK_ControllerHaptics.TriggerHapticPulse(m_controller, m_hapticStrenght * 2, float.PositiveInfinity, m_hapticInterval);
    }

    private void DeactivateGarbageEffects()
    {
        m_particleSucker.enabled = false;
        m_soundEmitter.SetParameter(m_garbageParamName, 0);
        m_suckingGarbage = false;

        VRTK_ControllerHaptics.TriggerHapticPulse(m_controller, m_hapticStrenght, float.PositiveInfinity, m_hapticInterval);
    }

    private bool CatchObjects()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        var hitCount = Physics.SphereCastNonAlloc(ray, m_vacuumWidth, hitBuffer, m_range, ~m_ignoredLayers);
        bool changed = false;

        for (int i = 0; i < hitCount; ++i)
        {
            var go = hitBuffer[i].transform.gameObject;
            var gd = FindGarbage(go);
            if (gd != null)
            {
                if (!m_suckedGarbages.Contains(gd))
                {
                    ActivateOnGarbage(gd);
                    m_suckedGarbages.Add(gd);
                    changed = true;
                }
                continue;
            }

            var rb = hitBuffer[i].rigidbody;
            if (rb != null)
            {
                if (!m_suckedRigidbodies.Contains(rb))
                {
                    m_suckedRigidbodies.Add(rb);
                    changed = true;
                }
            }
        }
        return changed;
    }

    private void UpdateSuckedGarbages()
    {
        for (int i = m_suckedGarbages.Count - 1; i >= 0; --i)
        {
            var gd = m_suckedGarbages[i];
            if (gd == null
                || !gd.isActiveAndEnabled
                || (!m_suckedGarbages[i].pullParticles.isPlaying && m_suckedGarbages[i].pullParticles.particleCount == 0)
                || Vector3.Angle(transform.forward, gd.transform.position - transform.position) > m_suckCutoffAngle)
            {
                DeactivateOnGarbage(m_suckedGarbages[i]);
                m_suckedGarbages.RemoveAt(i);
            }
            else if (gd.garbageAmount > 0)
            {
                gd.garbageAmount -= Time.deltaTime * m_garbageSuckSpeed;
            }
        }
    }

    private void UpdateSuckedRigidbodies()
    {
        for (int i = m_suckedRigidbodies.Count - 1; i >= 0; --i)
        {
            var rb = m_suckedRigidbodies[i];
            if (rb == null)
            {
                m_suckedRigidbodies.RemoveAt(i);
                continue;
            }

            var vector = rb.transform.position - transform.position;
            if (Vector3.Angle(transform.forward, vector) > m_suckCutoffAngle)
            {
                m_suckedRigidbodies.RemoveAt(i);
            }
            else
            {
                rb.AddForce(-vector * m_force);
            }
        }
    }

    private void UpdateSuckedObjects()
    {
        if (m_suckingGarbage)
        {
            UpdateSuckedGarbages();
        }
        UpdateSuckedRigidbodies();
    }

    private void UpdateVacuuming()
    {
        var garbageAmount = m_suckedGarbages.Count;
        var rbAmount = m_suckedGarbages.Count;
        CatchObjects();
        UpdateSuckedObjects();
        if (garbageAmount == 0 && m_suckedGarbages.Count > 0)
            ActivateGarbageEffects();
        else if (garbageAmount > 0 && m_suckedGarbages.Count == 0)
            DeactivateGarbageEffects();
    }

    private void Update()
    {
        m_particleSucker.m_affectDistance = m_range;
        m_particleSucker.m_suckForce = m_garbageSuckSpeed * m_range;

        UpdateVacuuming();

        if (m_suckingGarbage)
        {
            if (Random.value < m_bigHapticChance * Time.deltaTime)
            {
                VRTK_SDK_Bridge.HapticPulse(m_controller, m_bigHapticStrenght);
            }
        }

        UpdateWindParticles();
    }

    private void UpdateWindParticles()
    {
        if (!m_windParticles.isPlaying)
        {
            m_windParticles.Play();
        }

        //var targetPos = transform.position + transform.forward * 3;
        //UpdateWindParticles(targetPos);
    }

    //private void UpdateWindParticles(Vector3 targetPos)
    //{
    //    var dist = targetPos - transform.position;

    //    targetPos = transform.position + Vector3.ClampMagnitude(dist, 40);

    //    Vector3 pos;

    //    if (!m_windParticles.isPlaying)
    //    {
    //        pos = targetPos;
    //        m_windParticles.Play();
    //    }
    //    else
    //    {
    //        pos = Vector3.Lerp(m_windParticles.transform.position, targetPos, m_windMoveSpeed * Time.deltaTime);
    //    }

    //    dist = transform.position - pos;
    //    m_windParticles.transform.position = pos;
    //    m_windParticles.transform.up = dist;
    //    m_windParticles.transform.localScale = dist.magnitude / 3 * Vector3.one;
    //}
}
