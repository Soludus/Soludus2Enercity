using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectEnumerator : MonoBehaviour
{
    public abstract IEnumerable<GameObject> GetObjects();
}
