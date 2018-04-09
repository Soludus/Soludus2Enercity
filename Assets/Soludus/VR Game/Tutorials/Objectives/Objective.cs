using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour, ITextSource
{
    [Multiline]
    public string text = "";

    public bool returnIfNotComplete = true;

    protected virtual void OnEnable()
    {
        //Debug.Log("start objective: " + name);
    }

    protected virtual void OnDisable()
    {

    }

    public string GetText()
    {
        return text;
    }

    public virtual bool IsComplete()
    {
        return true;
    }
}
