using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireData : MonoBehaviour
{
    /// <summary>
    /// How many fires are active.
    /// </summary>
    public int burningFires;

    /// <summary>
    /// The duration of each intensity level in seconds. After it has passed, the intensity level increases.
    /// </summary>
    public float levelDuration;
}
