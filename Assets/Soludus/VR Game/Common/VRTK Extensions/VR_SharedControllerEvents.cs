using Soludus.SharedObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_SharedControllerEvents : MonoBehaviour
{
    public GameObjectEvent anyButtonPressed = null;

    public VRTK_ControllerEvents controllerEvents = null;

    private bool anyButton = false;

    private void OnEnable()
    {
        if (controllerEvents == null)
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
    }

    private void Update()
    {
        bool anyButton = controllerEvents.AnyButtonPressed();
        if (!this.anyButton && anyButton)
        {
            anyButtonPressed.Raise(controllerEvents.gameObject);
        }
        this.anyButton = anyButton;
    }
}
