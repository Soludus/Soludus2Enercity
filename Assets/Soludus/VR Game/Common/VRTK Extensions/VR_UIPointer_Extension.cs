using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_UIPointer_Extension : MonoBehaviour
{
    public bool togglePointerOnHit = false;

    private void Start()
    {
        if (GetComponent<VRTK_UIPointer>() == null)
        {
            VRTK_Logger.Error("VR_UIPointer_Extension needs VRTK_UIPointer attached.");
            return;
        }

        if (togglePointerOnHit)
        {
            GetComponent<VRTK_UIPointer>().activationMode = VRTK_UIPointer.ActivationMethods.AlwaysOn;
        }

        //Setup controller event listeners
        GetComponent<VRTK_UIPointer>().UIPointerElementEnter += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementEnter;
        GetComponent<VRTK_UIPointer>().UIPointerElementExit += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementExit;
        GetComponent<VRTK_UIPointer>().UIPointerElementClick += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementClick;
        GetComponent<VRTK_UIPointer>().UIPointerElementDragStart += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragStart;
        GetComponent<VRTK_UIPointer>().UIPointerElementDragEnd += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragEnd;
    }

    private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementEnter(object sender, UIPointerEventArgs e)
    {
        //VRTK_Logger.Info("UI Pointer entered " + e.currentTarget.name + " on Controller index [" + VRTK_ControllerReference.GetRealIndex(e.controllerReference) + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
        if (togglePointerOnHit && GetComponent<VRTK_Pointer>())
        {
            GetComponent<VRTK_Pointer>().Toggle(true);
        }
    }

    private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementExit(object sender, UIPointerEventArgs e)
    {
        //VRTK_Logger.Info("UI Pointer exited " + e.previousTarget.name + " on Controller index [" + VRTK_ControllerReference.GetRealIndex(e.controllerReference) + "] and the state was " + e.isActive);
        if (togglePointerOnHit && GetComponent<VRTK_Pointer>())
        {
            GetComponent<VRTK_Pointer>().Toggle(false);
        }
    }
    private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementClick(object sender, UIPointerEventArgs e)
    {
        //VRTK_Logger.Info("UI Pointer clicked " + e.currentTarget.name + " on Controller index [" + VRTK_ControllerReference.GetRealIndex(e.controllerReference) + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
    }

    private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragStart(object sender, UIPointerEventArgs e)
    {
        //VRTK_Logger.Info("UI Pointer started dragging " + e.currentTarget.name + " on Controller index [" + VRTK_ControllerReference.GetRealIndex(e.controllerReference) + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
    }

    private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragEnd(object sender, UIPointerEventArgs e)
    {
        //VRTK_Logger.Info("UI Pointer stopped dragging " + e.currentTarget.name + " on Controller index [" + VRTK_ControllerReference.GetRealIndex(e.controllerReference) + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
    }
}
