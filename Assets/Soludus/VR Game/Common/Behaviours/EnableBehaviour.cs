using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set many gameobjects active based on the active state of this component.
/// </summary>
public class EnableBehaviour : MonoBehaviour
{
    [Tooltip("The enabled state of objects in this list are matched to this component.")]
    public List<GameObject> objectsToSet = new List<GameObject>();
    [Tooltip("Wait until the end of frame to activate.")]
    public bool waitForEndOfFrame = true;

    private void OnEnable()
    {
        DoSetAllActive(true);
    }

    private void OnDisable()
    {
        SetAllActive(false);
    }

    private void DoSetAllActive(bool active)
    {
        if (waitForEndOfFrame)
            StartCoroutine(SetAllActiveEndOfFrame(active));
        else
            SetAllActive(active);
    }

    private IEnumerator SetAllActiveEndOfFrame(bool active)
    {
        yield return new WaitForEndOfFrame();
        SetAllActive(active);
    }

    public void SetAllActive(bool active)
    {
        foreach (var o in objectsToSet)
        {
            if (o != null)
                o.SetActive(active);
        }
    }
}
