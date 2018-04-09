using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystem_UnityEvents : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    public GameObjectEvent onParticleCollision = new GameObjectEvent();
    public UnityEvent onParticleTrigger = new UnityEvent();

    private void OnParticleCollision(GameObject other)
    {
        onParticleCollision.Invoke(other);
    }

    private void OnParticleTrigger()
    {
        onParticleTrigger.Invoke();
    }
}
