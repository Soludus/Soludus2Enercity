using Soludus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSucker : MonoBehaviour
{
    [Header("Particle Systems")]
    public List<ParticleSystem> m_particleSystems = new List<ParticleSystem>();
    public bool m_includeChildren = true;

    [Header("Sucking")]
    public float m_affectDistance = 20;
    public float m_suckForce = 20;
    public float m_minSpeed = 0;

    [Header("Other")]
    public bool m_setParticleLifetime = true;
    public bool m_rotatePsTowards = true;
    public bool m_resetLifetimeOnDisable = false;

    private class ParticleBuffer
    {
        public ParticleSystem ps;
        public ParticleSystem.Particle[] particles;
    }

    private ParticleBuffer[] m_pBuffers = null;
    private List<ParticleSystem> m_psList = new List<ParticleSystem>();

    private void Reset()
    {
        AddFoundParticleSystems();
    }

    private void OnEnable()
    {
        if (m_particleSystems.Count == 0)
            AddFoundParticleSystems();
        FetchParticleSystems();
        UpdateParticleBuffers();
    }

    private void OnDisable()
    {
        if (m_resetLifetimeOnDisable)
        {
            ResetParticleLifetime();
        }
    }

    private void Update()
    {
        //if (!Application.isPlaying)
        {
            FetchParticleSystems();
            UpdateParticleBuffers();
        }

        UpdateParticles();
        if (m_rotatePsTowards && m_particleSystems.Count > 0)
        {
            for (int i = 0; i < m_particleSystems.Count; ++i)
            {
                if (m_particleSystems[i] == null)
                    continue;

                m_particleSystems[i].transform.LookAt(transform.position);
            }
        }
    }

    private void AddFoundParticleSystems()
    {
        var p = GetComponentInParent<ParticleSystem>();
        if (p != null)
            m_particleSystems.Add(p);
    }

    private void FetchParticleSystems()
    {
        m_psList.Clear();

        for (int i = 0; i < m_particleSystems.Count; ++i)
        {
            if (m_particleSystems[i] == null)
                continue;

            if (m_includeChildren)
            {
                m_particleSystems[i].AddComponentsInChildrenToList(true, m_psList);
            }
            else
            {
                m_psList.Add(m_particleSystems[i]);
            }
        }
    }

    private void UpdateParticleBuffers()
    {
        if (m_pBuffers == null || m_pBuffers.Length != m_psList.Count)
        {
            var newBuffers = new ParticleBuffer[m_psList.Count];
            if (m_pBuffers != null && m_pBuffers.Length > 0)
            {
                for (int i = 0; i < newBuffers.Length; ++i)
                {
                    if (i < m_pBuffers.Length)
                        newBuffers[i] = m_pBuffers[i];
                }
            }
            m_pBuffers = newBuffers;
        }
        for (int i = 0; i < m_pBuffers.Length; ++i)
        {
            ParticleBuffer pb = m_pBuffers[i];
            if (pb == null)
                pb = new ParticleBuffer();

            pb.ps = m_psList[i];
            if (pb.ps != null)
            {
                int pCount = pb.ps.main.maxParticles;
                if (pb.particles == null || pb.particles.Length != pCount)
                    pb.particles = new ParticleSystem.Particle[pCount];
            }

            m_pBuffers[i] = pb;
        }
    }

    private void UpdateParticles()
    {
        if (m_suckForce == 0 && m_minSpeed == 0)
        {
            return;
        }

        float sqrAffectDist = m_affectDistance * m_affectDistance;
        bool setLifetime = m_setParticleLifetime;
        float moveMult = Time.deltaTime * m_suckForce;
        float invSuckForce = 1 / m_suckForce;
        float minDistanceDelta = m_minSpeed * Time.deltaTime;
        float invMinSpeed = 1 / m_minSpeed;
        Vector3 worldSuckPos = transform.position;

        for (int i = 0; i < m_pBuffers.Length; ++i)
        {
            var ps = m_pBuffers[i].ps;
            if (ps == null)
            {
                m_pBuffers[i].particles = null;
                continue;
            }

            Vector3 suckPos;
            switch (ps.main.simulationSpace)
            {
                case ParticleSystemSimulationSpace.Local:
                    suckPos = ps.transform.InverseTransformPoint(worldSuckPos);
                    break;
                case ParticleSystemSimulationSpace.World:
                    suckPos = worldSuckPos;
                    break;
                case ParticleSystemSimulationSpace.Custom:
                    suckPos = ps.main.customSimulationSpace.InverseTransformPoint(worldSuckPos);
                    break;
                default:
                    suckPos = Vector3.zero;
                    break;
            }

            var particles = m_pBuffers[i].particles;
            int particleCount = ps.GetParticles(particles);

            for (int j = 0; j < particleCount; ++j)
            {
                float sqrDist = Vector3.SqrMagnitude(suckPos - particles[j].position);

                if (sqrDist < 1)
                {
                    // reset velocity to prevent particles staying near the suck point without reaching it
                    particles[j].velocity = Vector3.zero;
                }

                if (sqrDist < sqrAffectDist)
                {
                    float dist = Mathf.Sqrt(sqrDist);
                    float maxDistanceDelta = moveMult / dist;

                    if (maxDistanceDelta < minDistanceDelta)
                    {
                        maxDistanceDelta = minDistanceDelta;
                        if (setLifetime)
                            particles[j].remainingLifetime = dist * invMinSpeed;
                    }
                    else
                    {
                        if (setLifetime)
                            particles[j].remainingLifetime = sqrDist * invSuckForce;
                    }

                    particles[j].position = Vector3.MoveTowards(particles[j].position, suckPos, maxDistanceDelta);
                }
            }

            ps.SetParticles(particles, particleCount);
        }
    }

    private void ResetParticleLifetime()
    {
        for (int i = 0; i < m_pBuffers.Length; ++i)
        {
            var ps = m_pBuffers[i].ps;
            if (ps == null)
            {
                m_pBuffers[i].particles = null;
                continue;
            }

            var particles = m_pBuffers[i].particles;
            int particleCount = ps.GetParticles(particles);

            for (int j = 0; j < particleCount; ++j)
            {
                particles[j].remainingLifetime = particles[j].startLifetime;
            }

            ps.SetParticles(particles, particleCount);
        }
    }

}
