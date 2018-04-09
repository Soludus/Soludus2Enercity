using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectListEnumerator : GameObjectEnumerator
{
    public List<GameObject> objects = new List<GameObject>();

    public override IEnumerable<GameObject> GetObjects()
    {
        return objects;
    }
}
