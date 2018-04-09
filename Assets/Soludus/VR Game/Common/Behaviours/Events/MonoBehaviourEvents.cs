using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourEvents : MonoBehaviour
{
    public delegate void GameObjectDelegate(GameObject go);

    public event GameObjectDelegate onAwake;
    public event GameObjectDelegate onEnable;
    public event GameObjectDelegate onStart;
    public event GameObjectDelegate onDisable;
    public event GameObjectDelegate onDestroy;

    private void Awake()
    {
        if (onAwake != null)
            onAwake.Invoke(gameObject);
    }

    private void OnEnable()
    {
        if (onEnable != null)
            onEnable.Invoke(gameObject);
    }

    private void Start()
    {
        if (onStart != null)
            onStart.Invoke(gameObject);
    }

    private void OnDisable()
    {
        if (onDisable != null)
            onDisable.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        if (onDestroy != null)
            onDestroy.Invoke(gameObject);
    }

}
