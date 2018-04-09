using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.SharedObjects
{

    [CreateAssetMenu(menuName = "Shared/Event/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = 0; i < listeners.Count; ++i)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }

}