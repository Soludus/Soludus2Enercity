using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class VR_ControllerModes : MonoBehaviour
{
    public List<ControllerMode> modes = new List<ControllerMode>();

    [Header("Init")]
    public int m_initMode = 0;

    private int m_currentMode = 0;

    [System.Serializable]
    public class ModeEvent : UnityEvent<string> { }

    public ModeEvent onModeChanged;

    [System.Serializable]
    public class ControllerMode
    {
        public string name = "Mode";
        public UnityEvent onEnterMode;
        public UnityEvent onExitMode;

        //[Header("Pointer")]
        //public VRTK_BasePointerRenderer pointerRenderer;
        //public VRTK_ControllerEvents.ButtonAlias activationButton;

        //public void ApplyMode(GameObject controller)
        //{
        //    var pointer = controller.GetComponent<VRTK_Pointer>();
        //    pointer.Toggle(false);
        //    pointer.activationButton = activationButton;
        //    pointer.pointerRenderer = pointerRenderer;
        //}
    }

    private void OnEnable()
    {
        SetMode(m_initMode);
    }

    public void SetMode(int mode)
    {
        modes[m_currentMode].onExitMode.Invoke();
        m_currentMode = mode;
        modes[m_currentMode].onEnterMode.Invoke();
        onModeChanged.Invoke(modes[m_currentMode].name);
        //modes[mode].ApplyMode(gameObject);
    }

    public void IncrementMode()
    {
        SetMode((m_currentMode + 1) % modes.Count);
    }
}
