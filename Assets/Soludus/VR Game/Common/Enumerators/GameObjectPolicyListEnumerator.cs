using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// This enumerator filters the input enumerator using VRTK_PolicyList.
/// </summary>
public class GameObjectPolicyListEnumerator : GameObjectEnumerator
{
    public GameObjectEnumerator input = null;
    public VRTK_PolicyList policyList = null;

    public override IEnumerable<GameObject> GetObjects()
    {
        foreach (var item in input.GetObjects())
        {
            if (item != null && policyList.Find(item))
                yield return item;
        }
    }
}
