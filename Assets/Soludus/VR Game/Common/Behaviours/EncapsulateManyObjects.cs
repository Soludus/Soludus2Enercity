using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulate all defined objects.
/// </summary>
public class EncapsulateManyObjects : EncapsulateObject
{
    [SerializeField]
    private List<GameObject> m_objects = new List<GameObject>();
    [SerializeField]
    private bool m_useColliderFinders = true;
    [SerializeField]
    private List<ColliderFinder> m_colliderFinders = new List<ColliderFinder>();

    private HashSet<GameObject> m_objectSet = new HashSet<GameObject>();

    public override void Encapsulate()
    {
        m_objectSet.Clear();

        foreach (var go in m_objects)
            m_objectSet.Add(go);

        if (m_useColliderFinders && m_colliderFinders.Count > 0)
        {
            var cols = new List<Collider>();

            foreach (var cf in m_colliderFinders)
                cf.GetColliders(cols);

            foreach (var c in cols)
                m_objectSet.Add(c.gameObject);
        }

        foreach (var go in m_objectSet)
        {
            Encapsulate(go, m_encapsulateWith);
        }
    }
}
