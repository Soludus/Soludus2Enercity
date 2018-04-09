using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocity3Provider : MonoBehaviour, IValueProvider<Vector3>, IValueProvider<float>
{
    private Rigidbody rb = null;

    private Quaternion lastRot;
    private Vector3 angularVelocity;

    public void InitValue()
    {
        rb = GetComponentInParent<Rigidbody>();
        lastRot = transform.rotation;
    }

    public Vector3 GetValue()
    {
        if (rb != null)
        {
            return rb.angularVelocity;
        }
        else
        {
            var rot = transform.rotation;
            var deltaRot = rot * Quaternion.Inverse(lastRot);
            var eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));

            angularVelocity = eulerRot / Time.deltaTime;
            lastRot = rot;
            return angularVelocity;
        }
    }

    float IValueProvider<float>.GetValue()
    {
        return GetValue().magnitude;
    }
}
