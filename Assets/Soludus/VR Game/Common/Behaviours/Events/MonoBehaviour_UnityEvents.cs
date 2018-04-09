using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviour_UnityEvents : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    public GameObjectEvent onAwake = new GameObjectEvent();
    public GameObjectEvent onEnable = new GameObjectEvent();
    public GameObjectEvent onStart = new GameObjectEvent();
    public GameObjectEvent onDisable = new GameObjectEvent();
    public GameObjectEvent onDestroy = new GameObjectEvent();

    private void Awake()
    {
        onAwake.Invoke(gameObject);
    }

    private void OnEnable()
    {
        onEnable.Invoke(gameObject);
    }

    private void Start()
    {
        onStart.Invoke(gameObject);
    }

    private void OnDisable()
    {
        onDisable.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        onDestroy.Invoke(gameObject);
    }

}
