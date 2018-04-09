using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalRotator : MonoBehaviour
{
    public SphericalTransform target = null;

    //private Vector3 lastPos;

    //private void Start()
    //{
    //    lastPos = transform.position;
    //}

    private void Update()
    {
        // 1
        var pos = transform.position;
        var s = SphereUtility.CartesianToSpherical(pos);
        target.r = s.x;
        target.lat = s.y;
        target.lon = s.z;


        // 2
        //var pos = transform.position;
        // NOTE: spherical positions can't be subtracted like this
        //var s = SphereUtility.CartesianToSpherical(pos) - SphereUtility.CartesianToSpherical(lastPos);
        //lastPos = pos;
        //target.r += s.x;
        //target.lat += s.y;
        //target.lon += s.z;


        // 3
        //var pos = transform.position;
        //var s = SphereUtility.CartesianToSpherical(pos) - SphereUtility.CartesianToSpherical(lastPos);
        //target.r += s.x;
        ////target.lat += s.y;
        ////target.lon += s.z;

        //var quat = Quaternion.FromToRotation(lastPos, pos);
        ////Debug.Log(quat.eulerAngles);

        //target.transform.rotation = quat * target.transform.rotation;
        //lastPos = pos;
    }
}
