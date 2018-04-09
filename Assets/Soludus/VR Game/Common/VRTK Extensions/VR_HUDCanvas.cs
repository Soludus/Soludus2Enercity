using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(Canvas))]
public class VR_HUDCanvas : MonoBehaviour
{
    protected Canvas canvas;
    protected VRTK_SDKManager sdkManager;

    protected virtual void OnEnable()
    {
        sdkManager = VRTK_SDKManager.instance;
        if (sdkManager != null)
        {
            sdkManager.LoadedSetupChanged += LoadedSetupChanged;
        }
        InitCanvas();
    }

    protected virtual void OnDisable()
    {
        if (sdkManager != null && !gameObject.activeSelf)
        {
            sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
        }
    }

    protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
    {
        SetCanvasCamera();
    }

    protected virtual void InitCanvas()
    {
        canvas = GetComponent<Canvas>();
        canvas.planeDistance = 0.5f;

        SetCanvasCamera();
    }

    protected virtual void SetCanvasCamera()
    {
        Transform sdkCamera = VRTK_DeviceFinder.HeadsetCamera();
        if (sdkCamera != null)
        {
            canvas.worldCamera = sdkCamera.GetComponent<Camera>();
        }
    }
}
