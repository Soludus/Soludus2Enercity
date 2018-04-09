using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfViewIndicator : MonoBehaviour
{
    public Transform m_indicator = null;
    public Vector3 m_viewportInset = new Vector3(0.15f, 0.3f, 0.15f);
    public float m_viewportDist = 1;

    public Vector3 m_normalScale = 0.01f * Vector3.one;
    public Vector3 m_outOfViewScale = 0.0001f * Vector3.one;

    public bool rotateTowardsWhenOutOfView = true;
    public Vector3 forwardAxis = new Vector3(0, 0, 1);

    private void OnDisable()
    {
        m_indicator.position = transform.position;
        m_indicator.localScale = m_normalScale;
        if (rotateTowardsWhenOutOfView)
        {
            m_indicator.rotation = Quaternion.identity;
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 scale;
        Vector3 indicatorPos;

        var cam = Camera.main;
        var viewPortPos = cam.WorldToViewportPoint(pos);

        viewPortPos = Vector3Util.ScaleBias(viewPortPos, Vector3.one + (m_viewportInset * 2), -m_viewportInset);

        var vpp = viewPortPos;
        vpp.x = Mathf.Clamp01(vpp.x);
        vpp.y = Mathf.Clamp01(vpp.y);
        if (vpp.z < 0)
        {
            vpp.y = 0;
            vpp.x = .5f;
            vpp.z = 0;
        }

        bool outOfView = vpp != viewPortPos;

        if (outOfView)
        {
            vpp = Vector3Util.ScaleBias(vpp, Vector3.one - (m_viewportInset * 2), m_viewportInset);
            vpp.z = m_viewportDist;
            indicatorPos = cam.ViewportToWorldPoint(vpp);
            scale = m_outOfViewScale;
        }
        else
        {
            indicatorPos = pos;
            scale = m_normalScale;
        }

        m_indicator.position = indicatorPos;
        m_indicator.localScale = scale;

        if (rotateTowardsWhenOutOfView)
        {
            if (outOfView)
            {
                m_indicator.rotation = Quaternion.LookRotation(cam.transform.position - indicatorPos, pos - indicatorPos) * Quaternion.FromToRotation(Vector3.up, forwardAxis);
            }
            else
            {
                m_indicator.rotation = Quaternion.LookRotation(cam.transform.position - indicatorPos);
            }
        }
    }

}
