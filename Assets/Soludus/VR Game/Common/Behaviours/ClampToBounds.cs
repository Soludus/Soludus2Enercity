using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clamps this object to a specific point on the provided bounds.
/// </summary>
public class ClampToBounds : MonoBehaviour
{
    public BoundsProvider boundsProvider;
    public Vector3 offset;
    public Side side;
    public bool update;

    public enum Side
    {
        Top,
        Bottom,
        Front,
        Back,
        Right,
        Left
    }

    private void Start()
    {
        UpdatePosition();
    }

    private void Update()
    {
        if (update)
        {
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        var bounds = boundsProvider.worldBounds;
        var cpob = bounds.center;

        switch (side)
        {
            case Side.Top:
                cpob.y = bounds.max.y;
                break;
            case Side.Bottom:
                cpob.y = bounds.min.y;
                break;
            case Side.Front:
                cpob.z = bounds.max.z;
                break;
            case Side.Back:
                cpob.z = bounds.min.z;
                break;
            case Side.Right:
                cpob.x = bounds.max.x;
                break;
            case Side.Left:
                cpob.x = bounds.min.x;
                break;
            default:
                break;
        }

        transform.position = cpob + offset;
    }
}
