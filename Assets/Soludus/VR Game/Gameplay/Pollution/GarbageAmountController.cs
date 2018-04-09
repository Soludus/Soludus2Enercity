using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageAmountController : MonoBehaviour
{
    public GarbageData m_gd = null;
    public Transform m_content = null;

    public ParticleSystem m_fumePs = null;
    public ParticleSystem m_garbagePs = null;

    private void OnEnable()
    {
        m_gd.garbageAmount = m_gd.initialGarbageAmount;
    }

    private void Update()
    {
        int count = m_content.childCount;
        int amount = Mathf.CeilToInt(m_gd.garbageAmount / m_gd.initialGarbageAmount * count);

        if (amount > 0)
        {
            if (!m_fumePs.isPlaying)
                m_fumePs.Play();
        }
        else
        {
            m_fumePs.Stop();
            m_garbagePs.Stop();

            gameObject.AddComponent<DestroyParticleSystem>();
            enabled = false;
        }

        for (int i = 0; i < count; ++i)
        {
            m_content.GetChild(i).gameObject.SetActive(i < amount);
        }
    }
}
