using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !DISABLE_FMOD
public class FMODCollisionSound : MonoBehaviour, IVRObjectHitHandler
{
    public FMODUnity.StudioEventEmitter emitter = null;
    public bool destroy = true;

    private void OnEnable()
    {
        emitter.transform.SetParent(null, false);
    }

    private void OnDestroy()
    {
        if (destroy && emitter != null)
            Destroy(emitter.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collide(collision.contacts[0].point);
    }

    public void OnVRObjectHit(GameObject other)
    {
        Collide(other.transform.position);
    }

    public void Collide(Vector3 position)
    {
        emitter.transform.position = position;
        emitter.Play();
    }
}
#endif
