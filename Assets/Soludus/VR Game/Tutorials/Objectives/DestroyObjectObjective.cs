using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectObjective : ObjectListObjective
{
    public override bool ObjectIsComplete(GameObject go)
    {
        if (go == null)
            return true;
        return !go.activeInHierarchy;
    }
}
