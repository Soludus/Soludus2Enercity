using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    public void Log(string message)
    {
        Debug.Log(message);
    }

    public void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }

    public void Log(Object message)
    {
        Debug.Log(message);
    }

    public void LogWarning(Object message)
    {
        Debug.LogWarning(message);
    }

    public void LogError(Object message)
    {
        Debug.LogError(message);
    }
}
