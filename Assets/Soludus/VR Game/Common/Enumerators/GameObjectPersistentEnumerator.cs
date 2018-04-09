using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enumerator keeps all the items that have been received from the input when calling GetObjects().
/// The returned collection will not contain duplicate items.
/// </summary>
public class GameObjectPersistentEnumerator : GameObjectEnumerator
{
    public GameObjectEnumerator input = null;

    public bool persistItems = true;

    private List<GameObject> items = new List<GameObject>();

    private void OnEnable()
    {
        items.Clear();
    }

    public override IEnumerable<GameObject> GetObjects()
    {
        if (!persistItems)
            items.Clear();

        foreach (var item in input.GetObjects())
        {
            if (!persistItems || !items.Contains(item))
                items.Add(item);
        }

        return items;
    }
}
