using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity3Provider : MonoBehaviour, IValueProvider<Vector3>, IValueProvider<float>
{
    private Rigidbody rb = null;

    private Vector3 lastPos;
    private Vector3 velocity;

    public void InitValue()
    {
        rb = GetComponentInParent<Rigidbody>();
        lastPos = transform.position;
    }

    public Vector3 GetValue()
    {
        if (rb != null)
        {
            return rb.velocity;
        }
        else
        {
            var pos = transform.position;
            velocity = (pos - lastPos) / Time.deltaTime;
            lastPos = pos;
            return velocity;
        }
    }

    float IValueProvider<float>.GetValue()
    {
        return GetValue().magnitude;
    }
}
