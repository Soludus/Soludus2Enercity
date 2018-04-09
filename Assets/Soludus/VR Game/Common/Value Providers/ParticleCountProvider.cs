using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCountProvider : MonoBehaviour, IValueProvider<int>, IValueProvider<float>
{
    public ParticleSystem ps = null;
    public bool useIsPlaying = false;
    public bool useParticleCountIncludingChildren = true;
    public bool useParticleCount = true;

    public void InitValue()
    {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    public int GetValue()
    {
        if (useIsPlaying && !ps.isPlaying)
            return 0;
        if (useParticleCountIncludingChildren)
            return ps.GetParticleCountIncludingChildren();
        if (useParticleCount)
            return ps.particleCount;
        return 1;
    }

    float IValueProvider<float>.GetValue()
    {
        return GetValue();
    }
}
