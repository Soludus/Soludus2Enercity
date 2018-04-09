using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Util
{
    public static Vector3 ScaleBias(Vector3 v, float scale, float bias)
    {
        return new Vector3(
            v.x * scale + bias,
            v.y * scale + bias,
            v.z * scale + bias
            );
    }

    public static Vector3 ScaleBias(Vector3 v, Vector3 scale, Vector3 bias)
    {
        return new Vector3(
            v.x * scale.x + bias.x,
            v.y * scale.y + bias.y,
            v.z * scale.z + bias.z
            );
    }
}
