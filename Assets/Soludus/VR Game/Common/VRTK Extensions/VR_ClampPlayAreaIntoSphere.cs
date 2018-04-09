using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_ClampPlayAreaIntoSphere : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public float radius = 100;

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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, radius);
        }
    }

    private void Update()
    {
        //Debug.Log("sphere clamp");
        var n = playArea.position - center;
        if (n.sqrMagnitude > radius * radius)
        {
            playArea.position = center + n.normalized * radius;
        }
    }
}
