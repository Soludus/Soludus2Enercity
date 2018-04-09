using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_FallbackRaycast : VRTK_CustomRaycast
{
    public Vector4 hitPlane = new Vector4(0, 1, 0, 0);

    public override bool CustomRaycast(Ray ray, out RaycastHit hitData, float length = float.PositiveInfinity)
    {
        if (base.CustomRaycast(ray, out hitData, length))
        {
            return true;
        }

        var hitPlane = new Plane(new Vector3(this.hitPlane.x, this.hitPlane.y, this.hitPlane.z), this.hitPlane.w);
        float hitPlaneEnter;
        if (hitPlane.Raycast(ray, out hitPlaneEnter) && hitPlaneEnter <= length)
        {
            hitData = new RaycastHit
            {
                distance = hitPlaneEnter,
                normal = hitPlane.normal,
                point = ray.GetPoint(hitPlaneEnter)
            };
            return true;
        }
        return false;
    }
}
