using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectObjective : Objective
{
    public GameObject m_object = null;

    public override bool IsComplete()
    {
        return m_object.activeInHierarchy;
    }
}
