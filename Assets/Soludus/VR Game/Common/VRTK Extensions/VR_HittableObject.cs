using Soludus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public interface IVRObjectHitHandler
{
    void OnVRObjectHit(GameObject other);
}

public class VR_HittableObject : MonoBehaviour
{
    public enum HitTypes
    {
        Other = 1,
        Controller = 2,
        Pointer = 4
    }

    [EnumFlags]
    public HitTypes m_allowedHits = (HitTypes)0xfff;
    public bool m_allowTriggers = false;
    public bool m_triggerEnter = true;
    public bool m_collisionEnter = true;

    private List<IVRObjectHitHandler> m_handlers = new List<IVRObjectHitHandler>();
    private List<VRTK_PlayerObject> m_vrPlayerObjs = new List<VRTK_PlayerObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (m_triggerEnter && (m_allowTriggers || !other.isTrigger))
            HandleHit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_collisionEnter)
            HandleHit(collision.transform.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        GameObject realObj;
        if (IsValidHitObject(other, out realObj))
        {
            //Debug.Log(gameObject + " hit by " + other);
            GetHandlers();
            for (int i = 0; i < m_handlers.Count; ++i)
            {
                m_handlers[i].OnVRObjectHit(realObj);
            }
        }
    }

    private void GetHandlers()
    {
        m_handlers.Clear();
        GetComponents(m_handlers);
    }

    private bool IsValidHitObject(GameObject other, out GameObject realObj)
    {
        var tc = other.GetComponentInParent<VRTK_TrackedController>();
        if (tc != null)
        {
            if ((m_allowedHits & HitTypes.Controller) != 0)
            {
                realObj = tc.gameObject;
                return true;
            }
            else
            {
                realObj = null;
                return false;
            }
        }

        m_vrPlayerObjs.Clear();
        other.GetComponentsInParent(true, m_vrPlayerObjs);

        for (int i = 0; i < m_vrPlayerObjs.Count; ++i)
        {
            if (m_vrPlayerObjs[i].objectType == VRTK_PlayerObject.ObjectTypes.Pointer)
            {
                if ((m_allowedHits & HitTypes.Pointer) != 0)
                {
                    realObj = m_vrPlayerObjs[i].gameObject;
                    return true;
                }
                else
                {
                    realObj = null;
                    return false;
                }
            }
            if (m_vrPlayerObjs[i].objectType == VRTK_PlayerObject.ObjectTypes.Controller)
            {
                if ((m_allowedHits & HitTypes.Controller) != 0)
                {
                    realObj = m_vrPlayerObjs[i].gameObject;
                    return true;
                }
                else
                {
                    realObj = null;
                    return false;
                }
            }
        }

        if ((m_allowedHits & HitTypes.Other) != 0)
        {
            realObj = other;
            return true;
        }

        realObj = null;
        return false;
    }
}
