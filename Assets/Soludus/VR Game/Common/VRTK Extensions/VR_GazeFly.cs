using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Fly towards the direction where the user is looking.
/// </summary>
public class VR_GazeFly : MonoBehaviour
{
    [SerializeField]
    private VRTK_ControllerEvents m_controllerEvents;

    [SerializeField]
    private float speedMultiplier = 1;

    [SerializeField]
    private float speedPow = 1;

    [SerializeField]
    private float rollTurnMultiplier = 1;

    [SerializeField]
    private float inputDeadZone = 0.05f;

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
            float inputValue = m_controllerEvents.GetTriggerAxis();

            if (inputValue > inputDeadZone)
            {
                Vector3 dir = m_headset.forward;

                if (rollTurnMultiplier != 0)
                {
                    var euler = m_headset.eulerAngles;
                    euler.z = 0;

                    var worldUp = Vector3.up;
                    worldUp = Quaternion.Euler(euler) * worldUp;

                    var headUp = m_headset.up;
                    var tilt = Vector3.ProjectOnPlane(headUp, worldUp);

                    dir += tilt * rollTurnMultiplier;
                }

                m_playArea.position += (Mathf.Pow(inputValue + 1, speedPow) - 1) * speedMultiplier * Time.deltaTime * dir;
            }
        }
    }
}
