using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_ClampPlayAreaIntoPlane : MonoBehaviour
{
    public Vector3 normal = new Vector3(0, 1, 0);
    public float distance = 0;

    private Transform playArea = null;

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
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
    }

    private void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.matrix = Matrix4x4.Rotate(Quaternion.LookRotation(normal));
            Gizmos.DrawCube(Vector3.forward * distance, new Vector3(10, 10, 0.1f));
        }
    }

    private void Update()
    {
        normal.Normalize();
        var pos = playArea.position;
        playArea.position = Vector3.ProjectOnPlane(pos, normal) + normal * distance;
    }
}
