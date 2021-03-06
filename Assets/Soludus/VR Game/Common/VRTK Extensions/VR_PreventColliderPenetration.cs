﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_PreventColliderPenetration : MonoBehaviour
{
    public LayerMask m_layerMask = ~0;
    [SerializeField]
    private int m_maxNeighbours = 16; // maximum amount of neighbours visualised

    private Transform m_playArea = null;
    private BoxCollider m_boxCollider = null;

    private Collider[] m_neighbours;

    private void Reset()
    {
        m_layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
    }

    protected virtual void Awake()
    {
        m_neighbours = new Collider[m_maxNeighbours];
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    private void OnEnable()
    {
        m_playArea = VRTK_DeviceFinder.PlayAreaTransform();
        if (m_playArea != null)
        {
            var container = new GameObject("[Autogenerated][VR_PlayAreaBoxCollider]");
            container.transform.SetParent(m_playArea, false);
            m_boxCollider = container.AddComponent<BoxCollider>();
            m_boxCollider.size = new Vector3(2.5f, 2, 2.5f);
            m_boxCollider.transform.localPosition = new Vector3(0, 1, 0);
            m_boxCollider.isTrigger = true;
        }
    }

    private void OnDisable()
    {
        if (m_boxCollider != null)
        {
            Destroy(m_boxCollider.gameObject);
        }
    }

    private void LateUpdate()
    {
        if (m_playArea != null)
        {
            Vector3 penetration;
            if (CalculatePenetration(out penetration))
            {
                m_playArea.position += penetration;
            }
        }
    }

    private bool CalculatePenetration(out Vector3 penetration)
    {
        var size = m_boxCollider.size;
        size.Scale(m_boxCollider.transform.lossyScale);
        var position = m_boxCollider.transform.TransformPoint(m_boxCollider.center);
        var rotation = m_boxCollider.transform.rotation;

        int count = Physics.OverlapBoxNonAlloc(position, size * 0.5f, m_neighbours, rotation, m_layerMask);

        //float maxComp = Mathf.Max(size.x, size.y, size.z) + 0.1f;
        //int count = Physics.OverlapSphereNonAlloc(m_playArea.position, maxComp, neighbours, ~(1 << LayerMask.NameToLayer("Ignore Raycast")));

        penetration = Vector3.zero;
        int overlapCount = 0;

        for (int i = 0; i < count; ++i)
        {
            var collider = m_neighbours[i];

            if (collider == m_boxCollider)
                continue; // skip ourself

            Vector3 otherPosition = collider.transform.position;
            Quaternion otherRotation = collider.transform.rotation;

            Vector3 direction;
            float distance;

            bool overlapped = Physics.ComputePenetration(
                    m_boxCollider, position, rotation,
                    collider, otherPosition, otherRotation,
                    out direction, out distance
                    );

            if (overlapped)
            {
                penetration += direction * distance;
                ++overlapCount;
            }

            //// draw a line showing the depenetration direction if overlapped
            //if (overlapped)
            //{
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawRay(transform.position, direction * distance);
            //}
        }

        penetration /= overlapCount;

        return overlapCount > 0;
    }
}
