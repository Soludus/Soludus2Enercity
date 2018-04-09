using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectsObjective : ObjectListObjective
{
    public override bool ObjectIsComplete(GameObject go)
    {
        return go != null && go.activeInHierarchy;
    }
}
