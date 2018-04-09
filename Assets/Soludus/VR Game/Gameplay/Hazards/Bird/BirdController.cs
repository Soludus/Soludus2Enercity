using Soludus;
using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour, IVRObjectHitHandler
{
    [Header("Steering")]
    public float flyingSpeed = 1;
    public float turningSpeed = 30;

    [Header("Flapping")]
    [Range(0, 1)]
    public float flapChance = 0.5f;
    public float flapFrequency = 1;
    public float flapSeed = 0;

    [Header("References")]
    public EnergyDevice device = null;
    public PoopData pd = null;
    public Animator animator = null;
    public ParticleSystem poopPS = null;
    public ParticleSystem hitPS = null;

    public Transform debugTarget = null;

    private Vector3 targetPos;
    private bool hasTarget = false;
    private float nextPoopTime = 0;
    private List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();
    private Rigidbody rb = null;

    private static class AnimParams
    {
        public static readonly int WingFlap = Animator.StringToHash("Wing Flap");
    }

    private void OnEnable()
    {
        if (device == null)
            device = GetComponent<EnergyDevice>();
        if (pd == null)
            pd = GetComponent<PoopData>();
        rb = GetComponent<Rigidbody>();

        pd.currentCount = 0;
        pd.cleanedCount = 0;
        pd.totalCount = 0;

        if (device.targetForEffects != null)
        {
            Vector2 xz = Random.insideUnitCircle.normalized * 20;
            float y = pd.dropHeight + Random.Range(-0f, 5f);
            transform.position = device.targetForEffects.transform.position + new Vector3(xz.x, y, xz.y);
        }
    }

    private void FixedUpdate()
    {
        animator.SetBool(AnimParams.WingFlap, Mathf.PerlinNoise(Time.time * flapFrequency, flapSeed) + (flapChance - 1) > 0);

        if (!hasTarget && ReadyToGetPoopTarget())
        {
            GetNewTarget();
            nextPoopTime = Time.time + Random.Range(pd.minInterval, pd.maxInterval);
            hasTarget = true;
        }

        if (hasTarget && ReadyToPoop())
        {
            Poop();
            hasTarget = false;
        }

        if (debugTarget != null && debugTarget.gameObject.activeSelf)
        {
            targetPos = debugTarget.position;
            if (Time.time > nextPoopTime)
            {
                nextPoopTime = Time.time + Random.Range(pd.minInterval, pd.maxInterval);
                hasTarget = true;
            }
        }

        // fly
        rb.position += Time.deltaTime * flyingSpeed * transform.forward;

        if (hasTarget)
        {
            // turn
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(targetPos - rb.position), Time.deltaTime * turningSpeed);
        }
    }

    private Vector3 GetRandomPositionOverSlot(EnergyDeviceSlot slot, float yOffset)
    {
        var device = slot.attachedDevices[Random.Range(0, slot.attachedDevices.Count)];
        return GetRandomPositionInGrid(device, yOffset);
    }

    private Vector3 GetRandomPositionInGrid(EnergyDevice device, float yOffset)
    {
        Vector3 pos = device.transform.position;
        Vector3 extents = device.grid.size * 0.5f;
        float minX = pos.x - extents.x; float maxX = pos.x + extents.x;
        float minZ = pos.z - extents.z; float maxZ = pos.z + extents.z;
        return new Vector3(Random.Range(minX, maxX), pos.y + yOffset, Random.Range(minZ, maxZ));
    }

    private bool ReadyToGetPoopTarget()
    {
        return device.targetForEffects != null
            && Time.time > nextPoopTime
            && pd.currentCount < pd.maxCount
            && pd.cleanedCount < pd.requiredCleansToStop;
    }

    private bool ReadyToPoop()
    {
        return Vector3.Distance(rb.position, targetPos) < 0.1f;
    }

    private void GetNewTarget()
    {
        targetPos = GetRandomPositionOverSlot(device.targetForEffects, pd.dropHeight);
    }

    private void Poop()
    {
        poopPS.Play();
    }

    public void OnParticleCollisionWithPoop(PoopSplat poop, GameObject other)
    {
        if (other.GetComponentInParent<WaterGun>() != null)
        {
            Destroy(poop.gameObject);
            --pd.currentCount;
            ++pd.cleanedCount;
            if (pd.cleanedCount >= pd.requiredCleansToStop && pd.currentCount <= 0)
            {
                if (this != null)
                    Destroy(gameObject);
            }
        }
    }

    public void OnPoopCollision(GameObject other)
    {
        if (other.GetComponentInParent<PoopSplat>() != null) // skip hits on other poop
            return;

        collisions.Clear();
        ParticlePhysicsExtensions.GetCollisionEvents(poopPS, other, collisions);

        for (int i = 0; i < collisions.Count; ++i)
        {
            var pf = pd.splatPrefabs[Random.Range(0, pd.splatPrefabs.Count)];
            var p = Instantiate(pf);

            ++pd.currentCount;
            ++pd.totalCount;

            p.GetOrAddComponent<PoopSplat>().pooper = this;

            var pos = collisions[i].intersection;
            RaycastHit hit;
            if (Physics.Raycast(pos, collisions[i].velocity, out hit))
            {
                pos = hit.point;
            }

            p.transform.position = pos;
            StartCoroutine(EnableCollider(p));
        }
    }

    public void OnVRObjectHit(GameObject other)
    {
        Hit(other);
    }

    private void Hit(GameObject other)
    {
        hitPS.transform.SetParent(null, true);
        hitPS.Play();
        hitPS.GetOrAddComponent<DestroyParticleSystem>().delay = 0.1f;
        Destroy(gameObject);
    }

    private IEnumerator EnableCollider(GameObject obj, float time = 1)
    {
        yield return new WaitForSeconds(time);
        obj.GetComponentInChildren<Collider>().enabled = true;
    }
}
