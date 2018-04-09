using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_ControllerMovementForce : MonoBehaviour
{
    private Vector3 lastPos;
    private Vector3 velocity;

    private HashSet<Rigidbody> hits = new HashSet<Rigidbody>();

    private void Update()
    {
        var pos = transform.position;
        velocity = (pos - lastPos) / Time.deltaTime;
        lastPos = pos;

        foreach (var rb in hits)
        {
            //Debug.Log(rb + " added force " + velocity + " : frame " + Time.frameCount, rb);
            if (rb == null)
                continue;
            rb.AddForce(Vector3.ClampMagnitude(velocity * 0.1f, 20), ForceMode.Impulse);
        }
        hits.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<VRTK_PlayerObject>() != null)
            return;
        var rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            //rb.AddForce(Vector3.ClampMagnitude(velocity * 0.1f, 20));
            hits.Add(rb);
        }
    }

}
