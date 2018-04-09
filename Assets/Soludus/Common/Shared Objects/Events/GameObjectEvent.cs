using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.SharedObjects
{

    [CreateAssetMenu(menuName = "Shared/Event/GameObject Event")]
    public class GameObjectEvent : ScriptableObject
    {
        private List<GameObjectEventListener> listeners = new List<GameObjectEventListener>();

        public void Raise(GameObject value)
        {
            for (int i = 0; i < listeners.Count; ++i)
            {
                listeners[i].OnEventRaised(value);
            }
        }

        public void RegisterListener(GameObjectEventListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnregisterListener(GameObjectEventListener listener)
        {
            listeners.Remove(listener);
        }
    }

}