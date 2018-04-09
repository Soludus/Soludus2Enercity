using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatRigidbodies : MonoBehaviour
{
    public float addedDrag = 5;
    public float addedAngularDrag = 1;

    public struct RigidParams
    {
        public float addedDrag;
        public float addedAngularDrag;
    }

    Dictionary<Rigidbody, RigidParams> affectedRigids = new Dictionary<Rigidbody, RigidParams>();

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            if (!affectedRigids.ContainsKey(rb))
            {
                affectedRigids.Add(rb, new RigidParams { addedDrag = addedDrag, addedAngularDrag = addedAngularDrag });
                rb.drag += addedDrag;
                rb.angularDrag += addedAngularDrag;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            RigidParams p;
            if (affectedRigids.TryGetValue(rb, out p))
            {
                rb.drag -= p.addedDrag;
                rb.angularDrag -= p.addedAngularDrag;
                affectedRigids.Remove(rb);
            }
        }
    }
}
