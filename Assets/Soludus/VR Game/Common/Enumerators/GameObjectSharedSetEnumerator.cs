using Soludus.SharedObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSharedSetEnumerator : GameObjectEnumerator
{
    public GameObjectSharedSet m_sharedSet = null;

    public override IEnumerable<GameObject> GetObjects()
    {
        return m_sharedSet.items;
    }
}
