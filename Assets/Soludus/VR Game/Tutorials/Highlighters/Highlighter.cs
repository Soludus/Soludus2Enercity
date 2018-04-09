using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Highlighter : MonoBehaviour
{
    public bool activateOnEnable = true;

    protected virtual void OnEnable()
    {
        if (activateOnEnable)
            Highlight();
    }

    protected virtual void OnDisable()
    {
        if (activateOnEnable)
            Unhighlight();
    }

    public abstract void Highlight();
    public abstract void Unhighlight();
}
