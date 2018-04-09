using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [System.Serializable]
    public class Listener
    {
        public string id;
        public UnityEvent onEvent;
    }

    public List<Listener> listeners = new List<Listener>();

    public void OnAnimationEvent(string id)
    {
        for (int i = 0; i < listeners.Count; ++i)
        {
            if (string.Equals(id, listeners[i].id))
                listeners[i].onEvent.Invoke();
        }
    }
}
