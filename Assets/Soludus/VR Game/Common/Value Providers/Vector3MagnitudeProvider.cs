using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3MagnitudeProvider : SourceProvider<Vector3>, IValueProvider<float>
{
    public void InitValue()
    {
        var source = GetSourceProvider();
        if (source != null)
            source.InitValue();
    }

    public float GetValue()
    {
        var source = GetSourceProvider();
        if (source != null)
            return source.GetValue().magnitude;
        return 0;
    }
}
