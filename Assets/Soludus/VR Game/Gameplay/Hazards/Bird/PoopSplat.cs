using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSplat : MonoBehaviour
{
    public BirdController pooper = null;

    private void OnParticleCollision(GameObject other)
    {
        pooper.OnParticleCollisionWithPoop(this, other);
    }
}
