using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObservedObject : MonoBehaviour
{
    /// <summary>
    /// The Unity world space position of the observer. This point is at the surface in world space.
    /// </summary>
    public abstract Vector3 observerWorldPosition { get; set; }

    /// <summary>
    /// The direction the object surface is facing at the observer position.
    /// </summary>
    public abstract Vector3 observerWorldSurfaceNormal { get; set; }

    /// <summary>
    /// The observedPosition is clamped to this minimum distance from surface.
    /// </summary>
    public abstract float minAltitude { get; set; }
    /// <summary>
    /// The observedPosition is clamped to this maximum distance from surface.
    /// </summary>
    public abstract float maxAltitude { get; set; }


    /// <summary>
    /// The position of the observer relative to the object.
    /// </summary>
    public abstract Vector3 observerPosition { get; set; }

    /// <summary>
    /// The observed distance from the surface of the object.
    /// </summary>
    public abstract float observerAltitude { get; set; }

    /// <summary>
    /// Convert a direction from world space to observed space.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public abstract Vector3 WorldToObservedDirection(Vector3 dir);
}
