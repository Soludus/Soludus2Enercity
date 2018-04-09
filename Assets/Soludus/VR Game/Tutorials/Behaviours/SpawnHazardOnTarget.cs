using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHazardOnTarget : MonoBehaviour
{
    public EnergyDeviceType type = null;
    public int rate = 1;

    private DeviceHazardManager mgr = null;
    private float spawnTime = 0;

    private void OnEnable()
    {
        mgr = FindObjectOfType<DeviceHazardManager>();
    }

    private void Update()
    {
        if (Time.time >= spawnTime + rate)
        if (mgr.TrySpawnHazardOnRandomTarget(type))
        {
            spawnTime = Time.time;
        }
    }
}
