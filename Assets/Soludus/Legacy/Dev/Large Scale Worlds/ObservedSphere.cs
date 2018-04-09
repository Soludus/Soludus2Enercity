// WIP
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scale, translate and rotate this sphere to make it appear that the "observerWorldPosition" is at "observerPosition" relative to a sphere with radius equal to "radius".
/// In Unity world space, the distance from surface stays at 0.
/// </summary>
[ExecuteInEditMode]
public class ObservedSphere : ObservedObject
{
    // NOTE: to correctly represent the orientation of the sphere, observerPosition cannot be used. A Quaternion and a distance would be enough.

    [SerializeField]
    private float m_observedRadius = 1;
    [SerializeField]
    private Vector3 m_observerWorldPosition = Vector3.zero;
    [SerializeField]
    private Vector3 m_observerWorldSurfaceNormal = Vector3.up;

    //private float observerRotationAroundNormal = 0;

    [SerializeField]
    private float m_minSurfaceDistance = 0;
    [SerializeField]
    private float m_maxSurfaceDistance = float.PositiveInfinity;

    [SerializeField]
    private Vector3 m_observerPosition = new Vector3(0, 1f, 0); // Default: North pole

    private Vector3 m_lastObserverNormal;

    /// <summary>
    /// The Unity world space position of the observer.
    /// </summary>
    public override Vector3 observerWorldPosition
    {
        get
        {
            return m_observerWorldPosition;
        }

        set
        {
            m_observerWorldPosition = value;
            UpdateTransformation();
        }
    }

    public override Vector3 observerWorldSurfaceNormal
    {
        get
        {
            return m_observerWorldSurfaceNormal;
        }

        set
        {
            m_observerWorldSurfaceNormal = value;
            UpdateTransformation();
        }
    }

    /// <summary>
    /// The position of the observer relative to the sphere.
    /// </summary>
    public override Vector3 observerPosition
    {
        get
        {
            return m_observerPosition;
        }

        set
        {
            m_observerPosition = value;
            m_observerPosition = ClampMagnitude(m_observerPosition, observedRadius + minAltitude, observedRadius + maxAltitude);
            UpdateTransformation();
        }
    }

    /// <summary>
    /// The observed radius of the sphere. Matches Unity world space radius ONLY when the observerPosition is at the surface (distance to origin == radius)
    /// </summary>
    public float observedRadius
    {
        get
        {
            return m_observedRadius;
        }

        set
        {
            m_observedRadius = value;
            observerPosition = observerPosition;
        }
    }

    public override float minAltitude
    {
        get
        {
            return m_minSurfaceDistance;
        }

        set
        {
            m_minSurfaceDistance = value;
            observerPosition = observerPosition;
        }
    }

    public override float maxAltitude
    {
        get
        {
            return m_maxSurfaceDistance;
        }

        set
        {
            m_maxSurfaceDistance = value;
            observerPosition = observerPosition;
        }
    }

    public override float observerAltitude
    {
        get
        {
            return observerPosition.magnitude - observedRadius;
        }

        set
        {
            observerPosition *= (value + observedRadius) / observerPosition.magnitude;
        }
    }

    private void OnValidate()
    {
        EnsureValues();
    }

    private void OnEnable()
    {
        EnsureValues();
    }

    private void Update()
    {
        //Debug.DrawRay(observerWorldPosition, WorldToObservedDirection(Vector3.forward) * 100, Color.blue);
        //Debug.DrawRay(observerWorldPosition, WorldToObservedDirection(Vector3.up) * 100, Color.green);
        //Debug.DrawRay(observerWorldPosition, WorldToObservedDirection(Vector3.right) * 100, Color.red);
    }

    private void EnsureValues()
    {
        observerPosition = observerPosition;
    }

    private void UpdateTransformation()
    {
        float objectLocalRadius = 0.5f;
        float unitySurfDist = 0.0f;

        float cameraDistFromObserverWorldPosition = 1;

        // adjust scale and position
        float unityRadius = observedRadius / (observerAltitude + cameraDistFromObserverWorldPosition);
        float unityScale = unityRadius / objectLocalRadius;
        transform.localScale = unityScale * Vector3.one;
        transform.position = observerWorldPosition - observerWorldSurfaceNormal * (unitySurfDist + unityRadius);

        UpdateRotationQuaternion();
        //ApplyRotationDifference();
    }

    private void UpdateRotationQuaternion()
    {
        Vector3 observerNormal = observerPosition.normalized;
        Vector3 poleNormal = observerWorldSurfaceNormal;

        //Debug.Log(observerNormal.ToString("G10"));

        // forward direction always facing towards pole, this should be used when near equator
        //Vector3 perpendicularToBoth = Vector3.Cross(observerNormal, poleNormal);
        //Vector3 toPoleTangent = Vector3.Cross(perpendicularToBoth, observerNormal);
        //transform.rotation = Quaternion.Inverse(Quaternion.LookRotation(toPoleTangent, observerNormal));

        // same as above?
        //transform.rotation = Quaternion.Inverse(Quaternion.LookRotation(Vector3.Cross(observerWorldSurfaceNormal, observerNormal), observerNormal));

        // this should be used when near poles
        // FIXME: error at opposite pole, can't know which way we have come to that point
        // the rotation around surfaceNormal is not defined
        transform.rotation = Quaternion.FromToRotation(observerNormal, observerWorldSurfaceNormal);
    }

    // does not work because precision problems?
    private void ApplyRotationDifference()
    {
        Vector3 observerNormal = Normalize(observerPosition); //observedPosition.normalized;
        Vector3 poleNormal = m_lastObserverNormal != observerNormal ? Vector3.Cross(observerNormal, m_lastObserverNormal) : Vector3.up;
        //Vector3 poleNormal = Vector3.up;
        float angle = (float)Angle(m_lastObserverNormal, observerNormal);
        Debug.Log(m_lastObserverNormal.x * m_lastObserverNormal.x + " " + observerNormal.x * observerNormal.x + " " + angle);
        transform.Rotate(poleNormal, angle, Space.Self);
        m_lastObserverNormal = observerNormal;

        // adjust rotation

        // forward direction always facing towards pole, this should be used when near equator
        //Vector3 perpendicularToBoth = Vector3.Cross(observerNormal, poleNormal);
        //Vector3 toPoleTangent = Vector3.Cross(perpendicularToBoth, observerNormal);
        //transform.rotation = Quaternion.Inverse(Quaternion.LookRotation(toPoleTangent, observerNormal));
    }

    public override Vector3 WorldToObservedDirection(Vector3 dir)
    {
        // inverse of rotation
        // is this more complicated?, depends on the position on the sphere?
        return Quaternion.Inverse(transform.rotation) * dir;
    }


    /* Utility */

    private static Vector3 ClampMagnitude(Vector3 v, float min, float max)
    {
        float mag = v.magnitude;
        if (mag < min)
            v *= min / mag;
        else if (mag > max)
            v *= max / mag;
        return v;
    }

    /* Double precision vector operations */

    private static Vector3 Normalize(Vector3 value)
    {
        double mag = Magnitude(value);
        if (mag > 1E-05f)
        {
            return Div(value, mag); //value / mag;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private static Vector3 Div(Vector3 value, double d)
    {
        return new Vector3((float)(value.x / d), (float)(value.y / d), (float)(value.z / d));
    }

    private static double Magnitude(Vector3 a)
    {
        return Math.Sqrt((double)a.x * a.x + (double)a.y * a.y + (double)a.z * a.z);
    }

    private static double Angle(Vector3 from, Vector3 to)
    {
        double dot = (double)from.x * to.x + (double)from.y * to.y + (double)from.z * to.z;

        if (dot < -1)
            dot = -1;
        else if (dot > 1)
            dot = 1;

        return (Math.Acos(dot) * 180 / Math.PI);
    }
}
