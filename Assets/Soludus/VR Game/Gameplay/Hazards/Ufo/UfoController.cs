using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UfoController : MonoBehaviour
{
    public EnergyDevice device;
    public UfoData ud;
    public DeviceHazardManager hazardManager;
    public ParticleSystem ufoFireParticles;
    public ParticleSystem ufoDrainParticles;

    private Rigidbody rigid;
    private Vector3 currentDestination;
    private Vector3 aboveDestination;
    private Vector3 hoveringStartPoint;
    private Vector3 hoveringMaxHeight;
    private int randomedWanderingAmount;
    private int timesWanderedAtStart;
    private float lastSqrMag;
    private float hoverTimer;
    private bool hoverUp;
    private bool destroyed;

    private VRTK_InteractableObject interactable;
    private bool hasControl;
    private bool regainControl;
    private float timeUngrabbed;

    private void OnEnable()
    {
        if (device == null)
            device = GetComponent<EnergyDevice>();
        if (ud == null)
            ud = GetComponent<UfoData>();
        if (hazardManager == null)
            hazardManager = FindObjectOfType<DeviceHazardManager>();
        rigid = GetComponent<Rigidbody>();

        device.forceDeactive = true;

        interactable = GetComponent<VRTK_InteractableObject>();
        interactable.InteractableObjectGrabbed += OnGrabbed;
        interactable.InteractableObjectUngrabbed += OnUnGrabbed;
    }

    private void OnDisable()
    {
        interactable.InteractableObjectGrabbed -= OnGrabbed;
        interactable.InteractableObjectUngrabbed -= OnUnGrabbed;
    }

    private void Start()
    {
        hasControl = true;
        lastSqrMag = Mathf.Infinity;
        randomedWanderingAmount = Random.Range(ud.minTimesWanderingAtStart, ud.maxTimesWanderingAtStart);
        GetNewDestination("Descend");
    }

    private void OnGrabbed(object sender, InteractableObjectEventArgs e)
    {
        EndHovering();
        hasControl = false;
        StopAllCoroutines();
        ud.state = "";
    }

    private void OnUnGrabbed(object sender, InteractableObjectEventArgs e)
    {
        timeUngrabbed = Time.time;
        if (!CanGetControl())
        {
            rigid.useGravity = true;
        }
    }

    private bool CanGetControl()
    {
        return !interactable.IsGrabbed() && rigid.velocity.magnitude < ud.throwSpeedTreshold && rigid.angularVelocity.magnitude < ud.throwRotationTreshold;
    }

    private void FixedUpdate()
    {
        if (destroyed)
            return;

        if (!hasControl)
        {
            const float regainDrag = 1;
            const float regainAngularDrag = 2;

            if (!regainControl && Time.time > timeUngrabbed + 1 && CanGetControl())
            {
                regainControl = true;
                rigid.drag += regainDrag;
                rigid.angularDrag += regainAngularDrag;
                rigid.useGravity = false;
            }
            if (regainControl)
            {
                var targetRot = Quaternion.Euler(0f, ud.bodyRotateSpeed * Time.time, (ud.maxRotateAngle * Mathf.Sin(Time.time * ud.swayingSpeed)));

                var delta = 100 * Time.deltaTime;

                rigid.rotation = Quaternion.RotateTowards(rigid.rotation, targetRot, delta);
                float angle = Quaternion.Angle(targetRot, rigid.rotation);

                if (rigid.velocity.magnitude < 0.1f && angle < 0.1f)
                {
                    hasControl = true;
                    regainControl = false;
                    rigid.drag -= regainDrag;
                    rigid.angularDrag -= regainAngularDrag;
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                    GetNewDestination("GetHeight");
                }
            }
        }
        if (hasControl)
        {

            var directionalVector = (currentDestination - transform.position).normalized * ud.moveSpeed;

            if (currentDestination != Vector3.zero && (ud.state == "Moving" || ud.state == "Descend From Above Target"))
            {
                float sqrMag = (currentDestination - transform.position).sqrMagnitude;

                if (sqrMag <= 0.1f)
                {
                    DestinationReached();
                }
                else
                {
                    rigid.velocity = directionalVector;
                    lastSqrMag = sqrMag;
                }
            }
            else if (aboveDestination != Vector3.zero && ud.state == "Move Above")
            {
                float sqrMag = (aboveDestination - transform.position).sqrMagnitude;

                if (sqrMag >= lastSqrMag || sqrMag <= 0f)
                {
                    DescendFromAboveTarget();
                }
                else
                {
                    rigid.velocity = (aboveDestination - transform.position).normalized * ud.moveSpeed;
                    lastSqrMag = sqrMag;
                }
            }
            else if (ud.state == "Hovering")
            {
                hoverTimer += Time.deltaTime;

                if (transform.position.y >= hoveringMaxHeight.y)
                {
                    hoverUp = true;
                }
                else if (transform.position.y <= hoveringStartPoint.y)
                {
                    hoverUp = false;
                }

                if (!hoverUp)
                {
                    rigid.velocity = new Vector3(0, ud.hoverSpeed, 0);
                }
                else
                {
                    rigid.velocity = new Vector3(0, -ud.hoverSpeed, 0);
                }

                if (hoverTimer > ud.timeDrainingEnergy)
                {
                    hoverTimer = 0;
                    EndHovering();
                    GetNewDestination("GetHeight");
                }
            }
            else if (currentDestination != Vector3.zero && ud.state == "Getting Height")
            {
                if (transform.position.y < currentDestination.y)
                {
                    rigid.velocity = directionalVector;
                }
                else
                {
                    ResetDestinationVariables();
                    StartCoroutine(WaitBeforeStateTransition("WanderAround"));
                }
            }
            else if (currentDestination != Vector3.zero && ud.state == "Descending")
            {
                if (transform.position.y > currentDestination.y)
                {
                    rigid.velocity = directionalVector;
                }
                else
                {
                    ResetDestinationVariables();
                    StartCoroutine(WaitBeforeStateTransition("WanderAround"));
                }
            }
            else if (currentDestination != Vector3.zero && ud.state == "Wander Around")
            {
                float sqrMag = (currentDestination - transform.position).sqrMagnitude;

                if (sqrMag > lastSqrMag)
                {
                    ResetDestinationVariables();
                    StartCoroutine(WaitBeforeStateTransition("WanderAround"));
                }
                else
                {
                    rigid.velocity = directionalVector;
                    lastSqrMag = sqrMag;
                }
            }

            rigid.rotation = Quaternion.Euler(0f, ud.bodyRotateSpeed * Time.time, (ud.maxRotateAngle * Mathf.Sin(Time.time * ud.swayingSpeed)));
        }
    }

    private void ResetDestinationVariables()
    {
        aboveDestination = Vector3.zero;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        lastSqrMag = Mathf.Infinity;
    }

    private void GetNewDestination(string action)
    {
        ResetDestinationVariables();
        if (action == "GetTarget")
        {
            //Debug.Assert(device.targetForEffects == null);

            device.SetTarget(hazardManager.GetValidTarget());

            if (device.targetForEffects == null)
            {
                ud.state = "Waiting new state";
                StartCoroutine(WaitBeforeStateTransition("Moving"));
            }
            else
            {
                currentDestination = device.targetForEffects.transform.position + new Vector3(0f, ud.heightAboveDestination, 0f);
                CheckDestinationForCollision();
            }
        }
        else if (action == "GetHeight")
        {
            currentDestination = new Vector3(transform.position.x, Random.Range(ud.objectSearchHeightMin, ud.objectSearchHeightMax), transform.position.z);
            ud.state = "Getting Height";
        }
        else if (action == "Descend")
        {
            currentDestination = new Vector3(transform.position.x, Random.Range(ud.objectSearchHeightMin, ud.objectSearchHeightMax), transform.position.z);
            ud.state = "Descending";
        }
        else if (action == "WanderAround")
        {
            currentDestination = new Vector3(transform.position.x + Random.Range(-25f, 25f), Random.Range(ud.objectSearchHeightMin, ud.objectSearchHeightMax), transform.position.z + Random.Range(-25f, 25f));
            ud.state = "Wander Around";
        }
    }

    private void CheckDestinationForCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, currentDestination + new Vector3(0f, -ud.heightAboveDestination, 0f) - transform.position, out hit))
        {
            if (hit.collider.transform.GetComponentInParent<EnergyDeviceSlot>() == null)
            {
                aboveDestination = new Vector3(device.targetForEffects.transform.position.x, transform.position.y, device.targetForEffects.transform.position.z);
                ud.state = "Move Above";
            }
            else
            {
                ud.state = "Moving";
            }
        }
    }

    private void DescendFromAboveTarget()
    {
        lastSqrMag = Mathf.Infinity;
        ud.state = "Descend From Above Target";
    }

    private void DestinationReached()
    {
        ResetDestinationVariables();
        StartHovering();
    }

    private void StartHovering()
    {
        hoveringStartPoint = currentDestination;
        transform.position = hoveringStartPoint;
        hoveringMaxHeight = currentDestination + new Vector3(0f, ud.hoverMaxHeight, 0f);
        hoverUp = true;
        ud.state = "Hovering";
        ufoDrainParticles.Play();
        device.forceDeactive = false;
        //Debug.Log("Start hovering:" + device.targetForEffects);
    }

    private void EndHovering()
    {
        hoverUp = false;
        ufoDrainParticles.FastStop(true, 1, 1);
        rigid.velocity = Vector3.zero;
        device.forceDeactive = true;
        device.SetTarget(null);
        //Debug.Log("End hovering:" + device.targetForEffects);
    }

    private IEnumerator WaitBeforeStateTransition(string nextState)
    {
        ud.state = "Waiting new state";
        yield return new WaitForSeconds(ud.stateTransitionDelay);

        if (nextState == "Moving")
        {
            GetNewDestination("GetTarget");
        }
        else if (nextState == "GetHeight")
        {
            GetNewDestination("GetHeight");
        }
        else if (nextState == "Hovering")
        {
            StartHovering();
        }
        else if (nextState == "WanderAround")
        {
            if (timesWanderedAtStart < randomedWanderingAmount)
            {
                timesWanderedAtStart++;
                GetNewDestination("WanderAround");
            }
            else if (Random.Range(1, 5) == 4)
            {
                GetNewDestination("WanderAround");
            }
            else
            {
                GetNewDestination("GetTarget");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Water Trigger")
        {
            if (!destroyed)
                Destroy();
            ufoFireParticles.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!destroyed)
            Destroy();
    }

    private void Destroy()
    {
        EndHovering();
        ufoFireParticles.Play();
        rigid.useGravity = true;
        rigid.angularDrag += 0.5f;
        destroyed = true;
        StopAllCoroutines();
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(10f);
        ufoFireParticles.transform.parent = null;
        ufoFireParticles.Stop();
        ufoFireParticles.gameObject.AddComponent<DestroyParticleSystem>();
        Destroy(gameObject);
    }
}
