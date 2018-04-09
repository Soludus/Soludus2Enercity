using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleSystemUtil
{
    private static ParticleSystem.Particle[] psBuffer = new ParticleSystem.Particle[2048];
    private static List<ParticleSystem> psChildList = new List<ParticleSystem>();

    public static int GetParticleCountIncludingChildren(this ParticleSystem ps)
    {
        psChildList.Clear();
        ps.GetComponentsInChildren(psChildList);

        int c = 0;

        for (int i = 0; i < psChildList.Count; ++i)
        {
            c += psChildList[i].particleCount;
        }
        return c;
    }

    public static void FastStop(this ParticleSystem ps, bool withChildren = true, float deathMaxLifetime = 1, float deathVelocityMult = 30)
    {
        if (withChildren)
        {
            psChildList.Clear();
            ps.GetComponentsInChildren(psChildList);

            for (int i = 0; i < psChildList.Count; ++i)
            {
                FastStopPS(psChildList[i], deathMaxLifetime, deathVelocityMult);
            }
        }
        else
        {
            FastStopPS(ps, deathMaxLifetime, deathVelocityMult);
        }
    }

    private static void FastStopPS(ParticleSystem ps, float deathMaxLifetime, float deathVelocityMult)
    {
        ps.Stop(false);
        int pCount = ps.GetParticles(psBuffer);
        for (int i = 0; i < pCount; ++i)
        {
            psBuffer[i].remainingLifetime = Mathf.Clamp(psBuffer[i].remainingLifetime, 0, deathMaxLifetime);
            psBuffer[i].velocity *= deathVelocityMult;
        }
        ps.SetParticles(psBuffer, pCount);
    }
}
