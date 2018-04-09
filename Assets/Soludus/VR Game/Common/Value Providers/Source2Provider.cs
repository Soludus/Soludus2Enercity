using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source2Provider<T> : MonoBehaviour
{
    public Object sourceA = null;
    public Object sourceB = null;

    public IValueProvider<T> GetSourceAProvider()
    {
        return GetSourceProvider(sourceA);
    }

    public IValueProvider<T> GetSourceBProvider()
    {
        return GetSourceProvider(sourceB);
    }

    public IValueProvider<T> GetSourceProvider(Object source)
    {
        return SourceProvider<T>.GetValueProvider(source, this);
    }
}
