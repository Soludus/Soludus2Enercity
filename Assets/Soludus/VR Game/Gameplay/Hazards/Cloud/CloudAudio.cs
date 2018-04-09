using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soludus;

public class CloudAudio : MonoBehaviour
{
    public CloudController largeCloud = null;
    public List<CloudController> smallClouds = new List<CloudController>();

    public FMODEmitterProxy emitter = null;
    public string countParameterName = "";
    [ReadOnly]
    public float countValue = 0;

    private bool CloudIsActive(CloudController cloud)
    {
        return cloud != null && cloud.isActiveAndEnabled && cloud.cloudPS.particleCount > 0;
    }

    private void Update()
    {
        countValue = 0;
        if (CloudIsActive(largeCloud))
            countValue = smallClouds.Count + 1;
        else
        {
            for (int i = 0; i < smallClouds.Count; ++i)
            {
                if (CloudIsActive(smallClouds[i]))
                    ++countValue;
            }
        }
        emitter.SetParameter(countParameterName, countValue);
    }
}
