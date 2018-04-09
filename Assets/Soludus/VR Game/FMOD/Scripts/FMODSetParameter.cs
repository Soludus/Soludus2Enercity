using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_FMOD
using FMODUnity;

public class FMODSetParameter : MonoBehaviour
{
    public StudioEventEmitter target = null;
    public string parameterName = "param1";

    private void OnEnable()
    {
        if (target == null)
            target = GetComponentInParent<StudioEventEmitter>();
    }

    public float GetParameter()
    {
        return target.GetParameter(parameterName);
    }

    public void SetParameter(float val)
    {
        target.SetParameter(parameterName, val);
    }

    public void IncrementParameter(float val)
    {
        target.IncrementParameter(parameterName, val);
    }
}

public static class StudioEventEmitterExtension
{
    public static float GetParameter(this StudioEventEmitter e, string name)
    {
        if (e.EventInstance.isValid())
        {
            float val, finalVal;
            e.EventInstance.getParameterValue(name, out val, out finalVal);
            return val;
        }
        return 0;
    }

    public static float IncrementParameter(this StudioEventEmitter e, string name, float val)
    {
        float p = e.GetParameter(name) + val;
        e.SetParameter(name, p);
        return p;
    }
}
#endif
