using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectListObjective : Objective
{
    public GameObjectEnumerator objects = null;
    public bool updateList = true;

    public int requiredCount = 0;

    protected IEnumerable<GameObject> objs = null;

    protected override void OnEnable()
    {
        base.OnEnable();
         objs = new List<GameObject>(objects.GetObjects());
    }

    public override bool IsComplete()
    {
        if (updateList)
            objs = objects.GetObjects();

        int c = 0;
        int count = 0;

        foreach (var item in objs)
        {
            ++count;
            if (ObjectIsComplete(item))
                ++c;
        }

        return c >= (requiredCount > 0 ? requiredCount : count);
    }

    public abstract bool ObjectIsComplete(GameObject go);
}
