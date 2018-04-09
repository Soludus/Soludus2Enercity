using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Moves the play area to this point.
/// </summary>
public class VR_StartPoint : MonoBehaviour
{
    [SerializeField]
    private Transform m_point = null;
    [SerializeField]
    private bool m_activateOnAwake = true;

    private Transform m_playArea = null;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    private void OnEnable()
    {
        m_playArea = VRTK_DeviceFinder.PlayAreaTransform();
        if (m_activateOnAwake)
        {
            MovePlayAreaToPoint();
        }
    }

    public void MovePlayAreaToPoint()
    {
        if (m_playArea != null)
        {
            m_playArea.position = m_point != null ? m_point.position : transform.position;
        }
    }
}
