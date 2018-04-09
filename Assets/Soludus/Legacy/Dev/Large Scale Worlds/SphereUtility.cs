using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SphereUtility
{
    // lat: north - south
    // lon: east - west

    /// <summary>
    /// Returns spherical coordinates(r, lat, lon) in radians given cartesian coordinates(x, y, z).
    /// </summary>
    /// <param name="cartesian">(x, y, z)</param>
    /// <returns></returns>
    public static Vector3 CartesianToSphericalRad(Vector3 cartesian)
    {
        if (cartesian == Vector3.zero) return Vector3.zero;
        float r = cartesian.magnitude;
        float lat = Mathf.Acos(cartesian.y / r);
        float lon = Mathf.Atan2(cartesian.z, cartesian.x);
        return new Vector3(r, lat, lon);
    }

    /// <summary>
    /// Returns cartesian coordinates(x, y, z) given spherical coordinates(r, lat, lon) in radians.
    /// </summary>
    /// <param name="spherical">(r, lat, lon)</param>
    /// <returns></returns>
    public static Vector3 SphericalRadToCartesian(Vector3 spherical)
    {
        if (spherical == Vector3.zero) return Vector3.zero;
        float rSinLat = spherical.x * Mathf.Sin(spherical.y);
        float x = rSinLat * Mathf.Cos(spherical.z);
        float z = rSinLat * Mathf.Sin(spherical.z);
        float y = spherical.x * Mathf.Cos(spherical.y);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Returns spherical coordinates(r, lat, lon) in degrees given cartesian coordinates(x, y, z).
    /// </summary>
    /// <param name="cartesian">(x, y, z)</param>
    /// <returns></returns>
    public static Vector3 CartesianToSpherical(Vector3 cartesian)
    {
        var res = CartesianToSphericalRad(cartesian);
        res.y = Mathf.Rad2Deg * res.y;
        res.z = Mathf.Rad2Deg * res.z;
        return res;
    }

    /// <summary>
    /// Returns cartesian coordinates(x, y, z) given spherical coordinates(r, lat, lon) in degrees.
    /// </summary>
    /// <param name="spherical">(r, lat, lon)</param>
    /// <returns></returns>
    public static Vector3 SphericalToCartesian(Vector3 spherical)
    {
        spherical.y = Mathf.Deg2Rad * spherical.y;
        spherical.z = Mathf.Deg2Rad * spherical.z;
        return SphericalRadToCartesian(spherical);
    }
}
