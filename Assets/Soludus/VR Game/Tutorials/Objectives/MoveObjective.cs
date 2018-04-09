using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjective : ObjectListObjective
{
    public float distance = 1;

    private Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var item in objs)
        {
            startPositions[item] = item.transform.position;
        }
    }

    public override bool ObjectIsComplete(GameObject go)
    {
        return (startPositions[go] - go.transform.position).magnitude >= distance;
    }
}
