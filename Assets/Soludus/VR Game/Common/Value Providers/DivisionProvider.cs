using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisionProvider : Source2Provider<float>, IValueProvider<float>
{
    public void InitValue()
    {
        var a = GetSourceAProvider();
        if (a == null) return;
        var b = GetSourceBProvider();
        if (b == null) return;

        a.InitValue();
        b.InitValue();
    }

    public float GetValue()
    {
        var a = GetSourceAProvider();
        if (a == null) return 0;
        var b = GetSourceBProvider();
        if (b == null) return 0;

        return a.GetValue() / b.GetValue();
    }
}
