using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds all colliders inside the defined area.
/// </summary>
public class ColliderFinder : MonoBehaviour
{
    [Header("Area")]
    public Vector3 extents;
    public float radius;
    [Header("Options")]
    public LayerMask layerMask = ~0;

    private Collider[] m_buffer = new Collider[64];

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (extents != Vector3.zero)
            Gizmos.DrawWireCube(Vector3.zero, 2 * extents);
        if (radius > 0)
            Gizmos.DrawWireSphere(Vector3.zero, radius);
    }

    public int GetColliders(List<Collider> cols)
    {
        int count = 0;

        if (extents != Vector3.zero)
        {
            count += Physics.OverlapBoxNonAlloc(transform.position, extents, m_buffer, transform.rotation, layerMask);
            Util.MoveBuffer(m_buffer, cols, true);
        }

        if (radius > 0)
        {
            count += Physics.OverlapSphereNonAlloc(transform.position, radius, m_buffer, layerMask);
            Util.MoveBuffer(m_buffer, cols, true);
        }

        return count;
    }
}
