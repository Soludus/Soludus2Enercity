using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalTransform : MonoBehaviour
{
    public float r = 1;
    public float lat = 0;
    public float lon = 0;

    public Transform translated;

    private void Update()
    {
        transform.eulerAngles = new Vector3(0, lon, -lat);

        if (translated)
            translated.position = SphereUtility.SphericalToCartesian(new Vector3(r, lat, lon));
    }
}
