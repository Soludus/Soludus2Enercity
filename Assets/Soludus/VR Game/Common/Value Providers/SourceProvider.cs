using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SourceProvider<T> : MonoBehaviour
{
    public Object source = null;

    public IValueProvider<T> GetSourceProvider()
    {
        return GetSourceProvider(source);
    }

    public IValueProvider<T> GetSourceProvider(Object source)
    {
        return GetValueProvider(source, this);
    }

    public static IValueProvider<T> GetValueProvider(Object source, Object owner = null)
    {
        IValueProvider<T> vp = source as IValueProvider<T>;
        if (vp != null)
            return vp;

        var go = source as GameObject;
        if (go != null)
            vp = go.GetComponent<IValueProvider<T>>();
        if (vp != null)
            return vp;

        Debug.LogWarning(source == null ?
            owner + ": No Source" :
            owner + ": " + source + " does not implement " + typeof(IValueProvider<T>), owner);
        return null;
    }
}
