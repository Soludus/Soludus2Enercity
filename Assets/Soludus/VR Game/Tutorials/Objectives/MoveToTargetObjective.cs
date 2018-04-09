using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetObjective : ObjectListObjective
{
    public Transform targetPoint = null;
    public float distance = 1;

    public override bool ObjectIsComplete(GameObject go)
    {
        return (go.transform.position - targetPoint.transform.position).magnitude < distance;
    }
}
