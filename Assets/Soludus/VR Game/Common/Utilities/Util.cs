using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    /// <summary>
    /// Move elements from buffer to list. Elements in buffer are set to null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="list"></param>
    public static void MoveBuffer<T>(T[] buffer, List<T> list, bool stopAtNull = false) where T : class
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] != null)
            {
                list.Add(buffer[i]);
                buffer[i] = null;
            }
            else if (stopAtNull)
            {
                break;
            }
        }
    }

    public static void Destroy(Object o)
    {
        if (Application.isPlaying)
        {
            if (o is GameObject)
                ((GameObject)o).SetActive(false);
            Object.Destroy(o);
        }
        else
        {
            Object.DestroyImmediate(o);
        }
    }
}
