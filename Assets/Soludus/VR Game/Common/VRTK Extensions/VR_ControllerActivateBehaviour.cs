using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class VR_ControllerActivateBehaviour : MonoBehaviour
{
    public VRTK_ControllerEvents m_controllerEvents = null;

    public VRTK_ControllerEvents.ButtonAlias m_button = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

    [System.Serializable]
    public class TouchpadSettings
    {
        public Vector2 m_touchpadAxis = new Vector2(0, 1);
        public float m_angleThreshold = 45;
    }
    public TouchpadSettings m_touchpadSettings = new TouchpadSettings();

    [System.Serializable]
    public class UnityEvents
    {
        public UnityEvent m_onPress;
        public UnityEvent m_onRelease;
    }
    public UnityEvents m_unityEvents = new UnityEvents();

    public Behaviour m_behaviourToActivate = null;
    public Behaviour m_behaviourToToggle = null;

    private bool m_isActive = false;

    private VR_ControllerActivateManager m_manager = null;

    private void OnEnable()
    {
        if (m_controllerEvents == null)
            m_controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();

        m_manager = FindObjectOfType<VR_ControllerActivateManager>();
        if (m_manager == null)
            m_manager = new GameObject("[VR_ControllerActivateManager]").AddComponent<VR_ControllerActivateManager>();

        m_manager.activators.Add(this);
    }

    private void OnDisable()
    {
        if (m_manager != null)
            m_manager.activators.Remove(this);
    }

    private bool IsTouchpad()
    {
        return (m_button == VRTK_ControllerEvents.ButtonAlias.TouchpadPress
            || m_button == VRTK_ControllerEvents.ButtonAlias.TouchpadTouch);
    }

    private bool IsTouchpadDirectionValid()
    {
        return Vector2.Angle(m_touchpadSettings.m_touchpadAxis, m_controllerEvents.GetTouchpadAxis()) < m_touchpadSettings.m_angleThreshold;
    }

    private void SetActive(bool active)
    {
        if (active)
            m_unityEvents.m_onPress.Invoke();
        else
            m_unityEvents.m_onRelease.Invoke();

        if (m_behaviourToActivate != null)
            m_behaviourToActivate.enabled = active;
        m_isActive = active;
    }

    private void Toggle()
    {
        if (m_behaviourToToggle != null)
            m_behaviourToToggle.enabled = !m_behaviourToToggle.enabled;
    }

    private bool IsPressed()
    {
        if (!m_controllerEvents.IsButtonPressed(m_button))
            return false;
        if (IsTouchpad())
            return IsTouchpadDirectionValid();
        return true;
    }


    private class VR_ControllerActivateManager : MonoBehaviour
    {
        public List<VR_ControllerActivateBehaviour> activators = new List<VR_ControllerActivateBehaviour>();

        private void Update()
        {
            // first do all deactivations
            for (int i = 0; i < activators.Count; ++i)
            {
                if (activators[i].m_isActive && !activators[i].IsPressed())
                    activators[i].SetActive(false);
            }
            // then all activations
            for (int i = 0; i < activators.Count; ++i)
            {
                if (!activators[i].m_isActive && activators[i].IsPressed())
                {
                    activators[i].SetActive(true);
                    activators[i].Toggle();
                }
            }
        }
    }
}
