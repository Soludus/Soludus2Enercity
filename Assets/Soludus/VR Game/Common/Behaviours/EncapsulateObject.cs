using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates the object and places this object as it's child.
/// </summary>
public class EncapsulateObject : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_encapsulateWith = null;
    [SerializeField]
    private string m_replacedChildName = null;
    [SerializeField]
    private bool m_copyPosition = true;
    [SerializeField]
    private bool m_copyRotation = true;
    [SerializeField]
    private bool m_copyName = true;

    [SerializeField]
    private bool m_encapsulateOnAwake = true;
    [SerializeField]
    private bool m_setActiveAfterEncapsulate = true;

    private bool m_encapsulated = false;

    private void OnEnable()
    {
        if (!m_encapsulated && m_encapsulateOnAwake)
        {
            m_encapsulated = true; // prevent recursion
            Encapsulate();
        }
    }

    public virtual void Encapsulate()
    {
        Encapsulate(gameObject, m_encapsulateWith);
    }

    public void Encapsulate(GameObject go, GameObject encapsulateWith)
    {
        if (encapsulateWith != null)
        {
            var self = go.transform;
            Vector3 position = m_copyPosition ? self.position : encapsulateWith.transform.position;
            Quaternion rotation = m_copyRotation ? self.rotation : encapsulateWith.transform.rotation;
            var instance = Instantiate(encapsulateWith, position, rotation);

            if (m_copyName)
                instance.name = go.name;

            var selfParent = self.parent;
            var selfSiblingIndex = self.GetSiblingIndex();

            if (!string.IsNullOrEmpty(m_replacedChildName))
            {
                var child = instance.transform.Find(m_replacedChildName);
                if (child == null)
                    throw new System.NullReferenceException("A child with the name " + m_replacedChildName + " was not found.");
                var siblingIndex = child.GetSiblingIndex();
                var parent = child.parent;
                DestroyImmediate(child.gameObject);
                self.SetParent(parent, true);
                self.SetSiblingIndex(siblingIndex);
            }
            else
            {
                self.SetParent(instance.transform, false);
            }
            instance.transform.SetParent(selfParent, true);
            instance.transform.SetSiblingIndex(selfSiblingIndex);

            if (m_setActiveAfterEncapsulate)
                instance.gameObject.SetActive(true);
        }
    }

}
