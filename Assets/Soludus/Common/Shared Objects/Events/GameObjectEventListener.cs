using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Soludus.SharedObjects
{

    public class GameObjectEventListener : MonoBehaviour
    {
        [System.Serializable]
        public class GameObjectUnityEvent : UnityEvent<GameObject> { }

        public GameObjectEvent Event;
        public GameObjectUnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(GameObject go)
        {
            Response.Invoke(go);
        }
    }

}