using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_PlayAreaScaler : MonoBehaviour
{
    [SerializeField, Range(0.01f, 100f)]
    private float m_scale = 1;
    [SerializeField]
    private bool m_keepHeadsetInPosition = true;

    private Transform m_playArea;
    private Transform m_headset;

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
        m_headset = VRTK_DeviceFinder.HeadsetTransform();
    }

    private void Update()
    {
        if (m_playArea != null)
        {
            var headsetLastPos = m_headset.position;

            var newScale = m_scale * Vector3.one;
            m_playArea.localScale = newScale;

            if (m_keepHeadsetInPosition)
            {
                var headsetNewPos = m_headset.position;
                m_playArea.position += headsetLastPos - headsetNewPos;
            }
        }
    }
}
