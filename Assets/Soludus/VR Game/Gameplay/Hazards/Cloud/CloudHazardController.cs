using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudHazardController : MonoBehaviour
{
    public EnergyDevice device;
    public List<CloudController> clouds = new List<CloudController>();
    public float height = 70;

    private void OnEnable()
    {
        if (device == null)
            device = GetComponent<EnergyDevice>();

        clouds.Clear();
        GetComponentsInChildren(true, clouds);

        if (device.targetForEffects != null)
        {
            var pos = device.targetForEffects.transform.position;
            pos.y = height;
            transform.position = pos;
        }

        for (int i = 0; i < clouds.Count; ++i)
        {
            if (clouds[i].enabled)
            {
                clouds[i].enabled = false;
                clouds[i].enabled = true;
            }
        }
    }

    private void Update()
    {
        // hazard is destroyed when all child clouds are deactivated
        for (int i = 0; i < clouds.Count; ++i)
        {
            if (clouds[i].isActiveAndEnabled)
                return;
        }

        DestroyCloudHazard();
        enabled = false;
    }

    public void DestroyCloudHazard()
    {
        gameObject.AddComponent<DestroyParticleSystem>();
    }
}
