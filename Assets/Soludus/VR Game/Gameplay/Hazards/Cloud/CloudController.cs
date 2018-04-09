using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour, IVRObjectHitHandler
{
    public ParticleSystem cloudPS;
    public ParticleSystem cloudHitPS;

    public float startCooldown = 0.5f;
    public float hitCooldown = 0.2f;
    public int hitsRequired = 1;
    public bool ignorePointerHit = true;
    public List<GameObject> activateOnDestroy = new List<GameObject>();

    public float deathMaxLifetime = 1;
    public float deathVelocityMult = 30;

    private Collider coll;
    private Rigidbody rigid;
    private Vector3 initPos;
    private float startTime;
    private float hitTime;
    private int hitCount = 0;

    private void OnEnable()
    {
        if (coll == null)
            coll = GetComponentInChildren<Collider>();
        if (rigid == null)
            rigid = GetComponent<Rigidbody>();
        if (cloudPS == null)
            cloudPS = transform.GetChild(0).GetComponent<ParticleSystem>();
        if (cloudHitPS == null)
            cloudHitPS = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();

        startTime = Time.time;
        initPos = transform.position;
        coll.enabled = false;
    }

    private void Update()
    {
        if (Time.time >= startTime + startCooldown)
        {
            coll.enabled = true;
        }

        //rigid.MovePosition(Vector3.MoveTowards(rigid.position, initPos, Time.deltaTime * 10));
        rigid.MovePosition(Vector3.Lerp(rigid.position, initPos, Time.deltaTime * 1));
    }

    public void OnVRObjectHit(GameObject other)
    {
        if (Time.time >= hitTime + hitCooldown)
        {
            hitTime = Time.time;
            ++hitCount;
            cloudHitPS.Play();
            if (hitCount >= hitsRequired)
            {
                DestroyCloud();
            }
        }
    }

    private void DestroyCloud()
    {
        cloudPS.Stop();
        cloudPS.FastStop(false, deathMaxLifetime, deathVelocityMult);

        //var pSRain = cloudPS.transform.GetChild(0).GetComponent<ParticleSystem>();
        //pSRain.Stop();

        coll.enabled = false;
        enabled = false;

        for (int i = 0; i < activateOnDestroy.Count; ++i)
        {
            activateOnDestroy[i].SetActive(true);
        }
    }
}
