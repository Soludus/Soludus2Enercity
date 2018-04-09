using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    public float delay = 0;

    private ParticleSystem[] ps;
    private float enableTime = 0;

    private void OnEnable()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
        enableTime = Time.time;
    }

    private void Update ()
    {
        if (Time.time < enableTime + delay)
            return;

        int particleCount = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            particleCount += ps[i].particleCount;
        }

        if (particleCount == 0)
        {
            //gameObject.SetActive(false);
            Util.Destroy(gameObject);
        }
	}
}
