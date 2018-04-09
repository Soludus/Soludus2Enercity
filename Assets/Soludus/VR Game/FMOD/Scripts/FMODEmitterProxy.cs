using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !DISABLE_FMOD
[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
#endif
public class FMODEmitterProxy : MonoBehaviour
{

#if !DISABLE_FMOD
    private FMODUnity.StudioEventEmitter m_emitter = null;

    private void Awake()
    {
        m_emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
#endif

    public void Play()
    {
#if !DISABLE_FMOD
        m_emitter.Play();
#endif
    }

    public void Stop()
    {
#if !DISABLE_FMOD
        m_emitter.Stop();
#endif
    }

    public void SetParameter(string name, float val)
    {
#if !DISABLE_FMOD
        m_emitter.SetParameter(name, val);
#endif
    }
}
