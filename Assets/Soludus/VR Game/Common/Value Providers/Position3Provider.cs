using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position3Provider : MonoBehaviour, IValueProvider<Vector3>
{
    public void InitValue()
    {
    }

    public Vector3 GetValue()
    {
        return transform.position;
    }
}
