using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Fly towards the direction where this object is pointed at.
/// </summary>
public class VR_ControllerPointFly : MonoBehaviour
{
    [SerializeField]
    private VRTK_ControllerEvents m_controllerEvents;

    [SerializeField]
    private float speedMultiplier = 1;

    [SerializeField]
    private float brakeMultiplier = 0.5f;

    [SerializeField]
    private float speedPow = 1;

    [SerializeField]
    private float altitudeMultiplier = 1;
    [SerializeField]
    private float groundHeight = 0;
    [SerializeField]
    private float clampHeightMin = 80;
    [SerializeField]
    private float clampHeightMax = 80;

    private Transform m_playArea;
    private Transform m_headset;

    private void OnEnable()
    {
        m_controllerEvents = m_controllerEvents ? m_controllerEvents : GetComponent<VRTK_ControllerEvents>();
        m_headset = VRTK_DeviceFinder.HeadsetTransform();
        m_playArea = VRTK_DeviceFinder.PlayAreaTransform();
    }

    private void Update()
    {
        if (m_headset != null && m_playArea != null)
        {
            float inputValue = m_controllerEvents.touchpadPressed ? (m_controllerEvents.GetTouchpadAxis().y > 0 ? 1 : -1) : 0;

            if (inputValue != 0)
            {
                inputValue *= m_controllerEvents.GetTriggerAxis() > 0.5f ? brakeMultiplier : 1;
                inputValue += inputValue * (m_headset.position.y - groundHeight) * altitudeMultiplier;
                inputValue = Mathf.Pow(inputValue + 1, speedPow) - 1;
                Vector3 dir = transform.forward;

                Vector3 diff = inputValue * speedMultiplier * Time.deltaTime * dir;
                Vector3 pos = m_playArea.position;

                pos += diff;

                if (diff != Vector3.zero)
                {
                    pos.y = Mathf.Clamp(pos.y, clampHeightMin, clampHeightMax);
                }

                m_playArea.position = pos;
            }
        }
    }
}
