// WIP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Fly towards the direction where this object is pointed at.
/// </summary>
public class VR_FlyControl_ObservedObject : MonoBehaviour
{
    public enum Button
    {
        TouchPad,
        Trigger
    }

    [SerializeField]
    private ObservedObject observedObject;

    [SerializeField]
    private VRTK_ControllerEvents m_controllerEvents;

    [SerializeField]
    private Button button = Button.TouchPad;

    [SerializeField]
    private float speedMultiplier = 1;

    [SerializeField]
    private float speedPow = 1;

    [SerializeField]
    private float altitudeMultiplier = 1;

    [SerializeField]
    private float inputDeadZone = 0.05f;

    private Transform m_playArea;

    private void OnEnable()
    {
        m_controllerEvents = m_controllerEvents ? m_controllerEvents : GetComponent<VRTK_ControllerEvents>();
        m_playArea = VRTK_DeviceFinder.PlayAreaTransform();
    }

    private void Update()
    {
        if (m_playArea != null)
        {
            float inputValue = 0;

            switch (button)
            {
                case Button.TouchPad:
                    inputValue = m_controllerEvents.touchpadPressed ? (m_controllerEvents.GetTouchpadAxis().y > 0 ? 1 : -1) : 0;
                    break;
                case Button.Trigger:
                    inputValue = m_controllerEvents.GetTriggerAxis();
                    if (inputValue < inputDeadZone)
                        inputValue = 0;
                    break;
            }

            if (inputValue != 0)
            {
                inputValue += inputValue * observedObject.observerAltitude * altitudeMultiplier;

                observedObject.observerPosition += (Mathf.Pow(inputValue + 1, speedPow) - 1) * speedMultiplier * Time.deltaTime * observedObject.WorldToObservedDirection(transform.forward);
            }
        }
    }
}
