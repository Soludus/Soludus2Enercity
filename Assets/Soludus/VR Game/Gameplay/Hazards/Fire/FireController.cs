/* OBSOLETE Not in use. */

using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public EnergyDevice device;
    public FireData fd;
    public int waterParticlesHit;
    public int waterHitsBeforeDecreasingFire;
    public float IncreaseFireTimer;

    /// <summary>
    /// The intensity of the fire. The fire is put out when the level is reduced to 0.
    /// </summary>
    public int burningIntensityLevel;

    private GameObject particleSystemFlames;

    private void Awake()
    {
        if (device == null)
            device = GetComponent<EnergyDevice>();
        if (fd == null)
            fd = GetComponentInParent<FireData>();
        particleSystemFlames = transform.GetChild(0).GetChild(0).gameObject;
        IncreaseFireTimer = Time.time + fd.levelDuration;
    }

    private void OnEnable()
    {
        // FIXME:
        //if (fd != null && fd.targetedHolder.fireSpots != null)
        //{
        //    for (int i = 0; i < fd.targetedHolder.fireSpots.childCount; i++)
        //    {
        //        if (fd.targetedHolder.fireSpots.GetChild(i).childCount == 0)
        //        {
        //            StartFire(fd.targetedHolder.fireSpots.GetChild(i));
        //            return;
        //        }
        //    }
        //}
        fd = null;
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (fd != null)
        {
            --fd.burningFires;
            if (fd.burningFires <= 0 && device.targetForEffects != null)
            {
                GameObject.Destroy(fd.gameObject);
            }
        }
    }

    private void Update()
    {
        transform.localScale = new Vector3(0.333f, 0.333f, 0.333f) * burningIntensityLevel;
        if (Time.time >= IncreaseFireTimer && burningIntensityLevel < 3)
        {
            IncreaseFireTimer = Time.time + fd.levelDuration;
            IncreaseFireIntensity();   
        }
    }

    private void StartFire(Transform position)
    {
        ++fd.burningFires;

        transform.parent = position;
        transform.localPosition = Vector3.zero;

        particleSystemFlames.SetActive(true);
    }

    private void Extinguish()
    {
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<ParticleSystem>() != null && other.GetComponent<ParticleSystem>().name == "fx_WaterSpout")
        {
            //Debug.Log("Water hit fire");
            other.GetComponent<ParticleSystem>().trigger.SetCollider(0, GetComponent<Collider>());
            waterParticlesHit++;

            if (waterParticlesHit >= waterHitsBeforeDecreasingFire)
            {
                waterParticlesHit = 0;
                DecreaseFireIntensityLevel();
            }
        }
    }

    private void IncreaseFireIntensity()
    {
        if (burningIntensityLevel < 3)
        {
            burningIntensityLevel += 1;
        }
    }

    private void DecreaseFireIntensityLevel()
    {
        if (burningIntensityLevel > 0)
        {
            burningIntensityLevel -= 1;

            IncreaseFireTimer = Time.time + fd.levelDuration;
        }

        if (burningIntensityLevel == 0)
        {
            Extinguish();
        }
    }
}