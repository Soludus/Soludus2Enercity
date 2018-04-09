using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; --i)
        {
            Util.Destroy(t.GetChild(i).gameObject);
        }
    }
}
