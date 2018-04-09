using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_ClampPlayAreaIntoColliderBounds : MonoBehaviour
{
    public Collider clampInto = null;

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
            var bounds = clampInto.bounds;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

    private void Update()
    {
        //Debug.Log("bounds clamp");
        var bounds = clampInto.bounds;
        var pos = playArea.position;
        if (!bounds.Contains(pos))
        {
            playArea.position = bounds.ClosestPoint(pos);
        }
    }
}
