using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void ShadowDetectorEvent();

/// <summary>
/// Base for a tool that detects if this object is under shadow.
/// </summary>
public abstract class ShadowDetectorBase : MonoBehaviour
{
    public event ShadowDetectorEvent onEnterShadow;
    public event ShadowDetectorEvent onExitShadow;

    protected void OnEnterShadow()
    {
        if (onEnterShadow != null)
            onEnterShadow();
    }

    protected void OnExitShadow()
    {
        if (onExitShadow != null)
            onExitShadow();
    }

    /// <summary>
    /// Is this object under shadow.
    /// </summary>
    public abstract bool isUnderShadow
    {
        get;
    }

    /// <summary>
    /// Estimate how much of this object is in shadow.
    /// </summary>
    public abstract float percentageInShadow
    {
        get;
    }

    /// <summary>
    /// The objects that currently cast a shadow to this object.
    /// </summary>
    public abstract IEnumerable<GameObject> shadowCasters
    {
        get;
    }

}
