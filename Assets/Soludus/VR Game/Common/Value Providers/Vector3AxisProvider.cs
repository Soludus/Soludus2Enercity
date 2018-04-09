using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3AxisProvider : SourceProvider<Vector3>, IValueProvider<float>
{
    public enum Axis
    {
        X, Y, Z
    }
    public Axis axis = Axis.Y;

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
            return GetAxisValue(source.GetValue(), axis);
        return 0;
    }

    public static float GetAxisValue(Vector3 v, Axis a)
    {
        switch (a)
        {
            case Axis.X:
                return v.x;
            case Axis.Y:
                return v.y;
            case Axis.Z:
                return v.z;
            default:
                throw null;
        }
    }
}
