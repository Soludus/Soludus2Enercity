using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transform_SetParent : MonoBehaviour
{
    public bool worldPositionStays = true;

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent, worldPositionStays);
    }

    public void Unparent()
    {
        transform.SetParent(null, worldPositionStays);
    }
}
