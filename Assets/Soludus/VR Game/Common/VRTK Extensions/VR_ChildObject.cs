using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Places this object as a child of the defined VRTK object.
/// </summary>
public class VR_ChildObject : MonoBehaviour
{
    public enum ObjectType
    {
        Headset,
        LeftController,
        RightController,
        PlayArea
    }

    public ObjectType target;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    private void OnEnable()
    {
        switch (target)
        {
            case ObjectType.Headset:
                SetParent(VRTK_DeviceFinder.HeadsetTransform());
                break;
            case ObjectType.LeftController:
                SetParent(VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.LeftController));
                break;
            case ObjectType.RightController:
                SetParent(VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.RightController));
                break;
            case ObjectType.PlayArea:
                SetParent(VRTK_DeviceFinder.PlayAreaTransform());
                break;
        }
    }

    private void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
    }
}
