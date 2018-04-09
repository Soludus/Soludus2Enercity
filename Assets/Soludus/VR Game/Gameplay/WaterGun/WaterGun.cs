using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class WaterGun : MonoBehaviour
{
    public VRTK_ControllerEvents m_controllerEvents = null;
    public ParticleSystem m_waterParticleSystem = null;

    [Header("Haptics")]
    [Range(0,1)]
    public float m_hapticStrenght = 0.1f;
    [Range(0.001f, 0.1f)]
    public float m_hapticInterval = 0.01f;

    [Header("Sound")]
    public FMODEmitterProxy m_soundEmitter = null;
    public string m_hitSoundParamName = ""; 

    private VRTK_ControllerReference m_controller = null;
    private int m_waterHitFrame = 0;

    private void OnEnable()
    {
        if (m_controllerEvents == null)
            m_controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();

        TurnOn();
    }

    private void OnDisable()
    {
        TurnOff();
    }

    public void OnWaterParticleCollision(GameObject other)
    {
        m_soundEmitter.SetParameter(m_hitSoundParamName, 1);
        m_waterHitFrame = Time.frameCount;
    }

    private void Update()
    {
        if (Time.frameCount > m_waterHitFrame + 2)
            m_soundEmitter.SetParameter(m_hitSoundParamName, 0);
    }

    private void TurnOn()
    {
        m_waterParticleSystem.Play();
        m_controller = VRTK_ControllerReference.GetControllerReference(m_controllerEvents.gameObject);
        VRTK_ControllerHaptics.TriggerHapticPulse(m_controller, m_hapticStrenght, float.PositiveInfinity, m_hapticInterval);
        m_soundEmitter.Play();
    }

    private void TurnOff()
    {
        if (m_controller != null)
        {
            VRTK_ControllerHaptics.CancelHapticPulse(m_controller);
        }
        m_waterParticleSystem.Stop();
        m_soundEmitter.Stop();
    }
}
