using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Event Payload
/// </summary>
/// <param name="target">The target the event is dealing with.</param>
/// <param name="collider">An optional collider that the body physics is colliding with.</param>
public struct BodyPhysicsEventArgs
{
    public GameObject target;
    public Collider collider;
}

/// <summary>
/// Event Payload
/// </summary>
/// <param name="sender">this object</param>
/// <param name="e"><see cref="BodyPhysicsEventArgs"/></param>
public delegate void BodyPhysicsEventHandler(object sender, BodyPhysicsEventArgs e);

/// <summary>
/// The body physics script deals with how a user's body in the scene reacts to world physics and how to handle drops.
/// </summary>
/// <remarks>
/// The body physics creates a rigidbody and collider for where the user is standing to allow physics interactions and prevent going through walls.
/// </remarks>
[AddComponentMenu("VR/Presence/VR_PlayAreaBoxPhysics")]
public class VR_PlayAreaBoxPhysics : MonoBehaviour
{
    [Header("Body Collision Settings")]

    [Tooltip("If checked then the body collider and rigidbody will be used to check for rigidbody collisions.")]
    public bool enableBodyCollisions = true;
    [Tooltip("If this is checked then any items that are grabbed with the controller will not collide with the body collider. This is very useful if the user is required to grab and wield objects because if the collider was active they would bounce off the collider.")]
    public bool ignoreGrabbedCollisions = true;
    [Tooltip("An array of GameObjects that will not collide with the body collider.")]
    public GameObject[] ignoreCollisionsWith;

    [Header("Custom Settings")]

    [Tooltip("A GameObject to represent a custom body collider container. It should contain a collider component that will be used for detecting body collisions. If one isn't provided then it will be auto generated.")]
    public GameObject customBodyColliderContainer;

    /// <summary>
    /// Emitted when the body collider starts colliding with another game object.
    /// </summary>
    public event BodyPhysicsEventHandler StartColliding;
    /// <summary>
    /// Emitted when the body collider stops colliding with another game object.
    /// </summary>
    public event BodyPhysicsEventHandler StopColliding;

    protected Transform playArea;
    protected Transform headset;
    protected Rigidbody bodyRigidbody;
    protected GameObject bodyColliderContainer;

    protected CapsuleCollider bodyCollider;

    protected VRTK_CollisionTracker collisionTracker;
    protected bool currentBodyCollisionsSetting;
    protected GameObject currentCollidingObject = null;

    protected Vector3 lastPlayAreaPosition = Vector3.zero;
    protected float bodyRadius = 0.15f;

    protected bool generateRigidbody = false;
    protected Vector3 playAreaVelocity = Vector3.zero;
    protected const string BODY_COLLIDER_CONTAINER_NAME = "BodyColliderContainer";
    protected List<GameObject> ignoreCollisionsOnGameObjects = new List<GameObject>();
    protected Transform cachedGrabbedObjectTransform = null;
    protected VRTK_InteractableObject cachedGrabbedObject;

    // Draws a sphere for current standing position and a sphere for current headset position.
    // Set to `true` to view the debug spheres.
    protected bool drawDebugGizmo = false;

    /// <summary>
    /// The ArePhysicsEnabled method determines whether the body physics are set to interact with other scene physics objects.
    /// </summary>
    /// <returns>Returns true if the body physics will interact with other scene physics objects and false if the body physics will ignore other scene physics objects.</returns>
    public virtual bool ArePhysicsEnabled()
    {
        return (bodyRigidbody != null ? !bodyRigidbody.isKinematic : false);
    }

    /// <summary>
    /// The GetVelocity method returns the velocity of the body physics rigidbody.
    /// </summary>
    /// <returns>The velocity of the body physics rigidbody.</returns>
    public virtual Vector3 GetVelocity()
    {
        return (bodyRigidbody != null ? bodyRigidbody.velocity : Vector3.zero);
    }

    /// <summary>
    /// The GetAngularVelocity method returns the angular velocity of the body physics rigidbody.
    /// </summary>
    /// <returns>The angular velocity of the body physics rigidbody.</returns>
    public virtual Vector3 GetAngularVelocity()
    {
        return (bodyRigidbody != null ? bodyRigidbody.angularVelocity : Vector3.zero);
    }

    /// <summary>
    /// The ResetVelocities method sets the rigidbody velocity and angular velocity to zero to stop the Play Area rigidbody from continuing to move if it has a velocity already.
    /// </summary>
    public virtual void ResetVelocities()
    {
        bodyRigidbody.velocity = Vector3.zero;
        bodyRigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// The GetBodyColliderContainer method returns the auto generated GameObject that contains the body colliders.
    /// </summary>
    /// <returns>The auto generated body collider GameObject.</returns>
    /// <returns></returns>
    public virtual GameObject GetBodyColliderContainer()
    {
        return bodyColliderContainer;
    }

    /// <summary>
    /// The GetCurrentCollidingObject method returns the object that the body physics colliders are currently colliding with.
    /// </summary>
    /// <returns>The GameObject that is colliding with the body physics colliders.</returns>
    public virtual GameObject GetCurrentCollidingObject()
    {
        return currentCollidingObject;
    }

    /// <summary>
    /// The ResetIgnoredCollisions method is used to clear any stored ignored colliders in case the `Ignore Collisions On` array parameter is changed at runtime. This needs to be called manually if changes are made at runtime.
    /// </summary>
    public virtual void ResetIgnoredCollisions()
    {
        //Go through all the existing set up ignored colliders and reset their collision state
        for (int i = 0; i < ignoreCollisionsOnGameObjects.Count; i++)
        {
            if (ignoreCollisionsOnGameObjects[i] != null)
            {
                Collider[] objectColliders = ignoreCollisionsOnGameObjects[i].GetComponentsInChildren<Collider>();
                for (int j = 0; j < objectColliders.Length; j++)
                {
                    ManagePhysicsCollider(objectColliders[j], false);
                }
            }
        }

        ignoreCollisionsOnGameObjects.Clear();
    }

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnEnable()
    {
        SetupPlayArea();
        SetupHeadset();
        EnableBodyPhysics();
        SetupIgnoredCollisions();
    }

    protected virtual void OnDisable()
    {
        DisableBodyPhysics();
        ManageCollisionListeners(false);
        ResetIgnoredCollisions();
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void FixedUpdate()
    {
        CheckBodyCollisionsSetting();
        CalculateVelocity();
        UpdateCollider();

        lastPlayAreaPosition = (playArea != null ? playArea.position : Vector3.zero);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (CheckValidCollision(collision.gameObject))
        {
            currentCollidingObject = collision.gameObject;
            OnStartColliding(SetBodyPhysicsEvent(currentCollidingObject, collision.collider));
        }
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        if (CheckValidCollision(collider.gameObject))
        {
            currentCollidingObject = collider.gameObject;
            OnStartColliding(SetBodyPhysicsEvent(currentCollidingObject, collider));
        }

    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (CheckExistingCollision(collision.gameObject))
        {
            OnStopColliding(SetBodyPhysicsEvent(currentCollidingObject, collision.collider));
            currentCollidingObject = null;
        }
    }

    protected virtual void OnTriggerExit(Collider collider)
    {
        if (CheckExistingCollision(collider.gameObject))
        {
            OnStopColliding(SetBodyPhysicsEvent(currentCollidingObject, collider));
            currentCollidingObject = null;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (drawDebugGizmo && headset != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(headset.position.x, headset.position.y - 0.3f, headset.position.z), 0.075f);
        }
    }

    protected virtual bool CheckValidCollision(GameObject checkObject)
    {
        return (!VRTK_PlayerObject.IsPlayerObject(checkObject));
    }

    protected virtual bool CheckExistingCollision(GameObject checkObject)
    {
        return (currentCollidingObject != null && currentCollidingObject.Equals(checkObject));
    }

    protected virtual void SetupPlayArea()
    {
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
        if (playArea != null)
        {
            lastPlayAreaPosition = playArea.position;
            collisionTracker = playArea.GetComponent<VRTK_CollisionTracker>();
            if (collisionTracker == null)
            {
                collisionTracker = playArea.gameObject.AddComponent<VRTK_CollisionTracker>();
            }
            ManageCollisionListeners(true);
        }
    }

    protected virtual void SetupHeadset()
    {
        headset = VRTK_DeviceFinder.HeadsetTransform();
        if (headset != null)
        {
        }
    }

    protected virtual void ManageCollisionListeners(bool state)
    {
        if (collisionTracker != null)
        {
            if (state)
            {
                collisionTracker.CollisionEnter += CollisionTracker_CollisionEnter;
                collisionTracker.CollisionExit += CollisionTracker_CollisionExit;
                collisionTracker.TriggerEnter += CollisionTracker_TriggerEnter;
                collisionTracker.TriggerExit += CollisionTracker_TriggerExit;
            }
            else
            {
                collisionTracker.CollisionEnter -= CollisionTracker_CollisionEnter;
                collisionTracker.CollisionExit -= CollisionTracker_CollisionExit;
                collisionTracker.TriggerEnter -= CollisionTracker_TriggerEnter;
                collisionTracker.TriggerExit -= CollisionTracker_TriggerExit;
            }
        }
    }

    protected virtual void CollisionTracker_TriggerExit(object sender, CollisionTrackerEventArgs e)
    {
        OnTriggerExit(e.collider);
    }

    protected virtual void CollisionTracker_TriggerEnter(object sender, CollisionTrackerEventArgs e)
    {
        OnTriggerEnter(e.collider);
    }

    protected virtual void CollisionTracker_CollisionExit(object sender, CollisionTrackerEventArgs e)
    {
        OnCollisionExit(e.collision);
    }

    protected virtual void CollisionTracker_CollisionEnter(object sender, CollisionTrackerEventArgs e)
    {
        OnCollisionEnter(e.collision);
    }

    protected virtual void OnStartColliding(BodyPhysicsEventArgs e)
    {
        if (StartColliding != null)
        {
            StartColliding(this, e);
        }
    }

    protected virtual void OnStopColliding(BodyPhysicsEventArgs e)
    {
        if (StopColliding != null)
        {
            StopColliding(this, e);
        }
    }

    protected virtual BodyPhysicsEventArgs SetBodyPhysicsEvent(GameObject target, Collider collider)
    {
        BodyPhysicsEventArgs e;
        e.target = target;
        e.collider = collider;
        return e;
    }

    protected virtual void CalculateVelocity()
    {
        playAreaVelocity = (playArea != null ? (playArea.position - lastPlayAreaPosition) / Time.fixedDeltaTime : Vector3.zero);
    }

    protected virtual void TogglePhysics(bool state)
    {
        if (bodyRigidbody != null)
        {
            bodyRigidbody.isKinematic = !state;
        }
        if (bodyCollider != null)
        {
            bodyCollider.isTrigger = !state;
        }

        currentBodyCollisionsSetting = state;
    }

    protected virtual void CheckBodyCollisionsSetting()
    {
        if (enableBodyCollisions != currentBodyCollisionsSetting)
        {
            TogglePhysics(enableBodyCollisions);
        }
    }

    protected virtual void EnableBodyPhysics()
    {
        currentBodyCollisionsSetting = enableBodyCollisions;

        CreateCollider();
        InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), true);
        InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), true);
    }

    protected virtual void DisableBodyPhysics()
    {
        DestroyCollider();
        InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), false);
        InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), false);
    }

    protected virtual void SetupIgnoredCollisions()
    {
        ResetIgnoredCollisions();
        if (ignoreCollisionsWith == null)
        {
            return;
        }

        for (int i = 0; i < ignoreCollisionsWith.Length; i++)
        {
            Collider[] objectColliders = ignoreCollisionsWith[i].GetComponentsInChildren<Collider>();
            for (int j = 0; j < objectColliders.Length; j++)
            {
                ManagePhysicsCollider(objectColliders[j], true);
            }

            if (objectColliders.Length > 0)
            {
                ignoreCollisionsOnGameObjects.Add(ignoreCollisionsWith[i]);
            }
        }
    }

    protected virtual void ManagePhysicsCollider(Collider collider, bool state)
    {
        Physics.IgnoreCollision(bodyCollider, collider, state);
    }

    protected virtual GameObject CreateColliderContainer(string name, Transform parent)
    {
        GameObject generatedContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, name));
        generatedContainer.transform.SetParent(parent);
        generatedContainer.transform.localPosition = Vector3.zero;
        generatedContainer.transform.localRotation = Quaternion.identity;
        generatedContainer.transform.localScale = Vector3.one;

        generatedContainer.layer = LayerMask.NameToLayer("Ignore Raycast");
        VRTK_PlayerObject.SetPlayerObject(generatedContainer, VRTK_PlayerObject.ObjectTypes.Collider);

        return generatedContainer;
    }

    protected virtual GameObject InstantiateColliderContainer(GameObject objectToClone, string name, Transform parent)
    {
        GameObject generatedContainer = Instantiate(objectToClone, parent);
        generatedContainer.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, name);
        VRTK_PlayerObject.SetPlayerObject(generatedContainer, VRTK_PlayerObject.ObjectTypes.Collider);

        return generatedContainer;
    }

    protected virtual void GenerateRigidbody()
    {
        bodyRigidbody = playArea.GetComponent<Rigidbody>();
        if (bodyRigidbody == null)
        {
            generateRigidbody = true;
            bodyRigidbody = playArea.gameObject.AddComponent<Rigidbody>();
            bodyRigidbody.freezeRotation = true;
        }
    }

    protected virtual CapsuleCollider GenerateCapsuleCollider(GameObject parent, float setRadius)
    {
        CapsuleCollider foundCollider = parent.GetComponent<CapsuleCollider>();
        if (foundCollider == null)
        {
            foundCollider = parent.AddComponent<CapsuleCollider>();
            foundCollider.radius = setRadius;
        }

        return foundCollider;
    }

    protected virtual void GenerateBodyCollider()
    {
        if (bodyColliderContainer == null)
        {
            if (customBodyColliderContainer != null)
            {
                bodyColliderContainer = InstantiateColliderContainer(customBodyColliderContainer, BODY_COLLIDER_CONTAINER_NAME, playArea);
                bodyCollider = bodyColliderContainer.GetComponent<CapsuleCollider>();
            }
            else
            {
                bodyColliderContainer = CreateColliderContainer(BODY_COLLIDER_CONTAINER_NAME, playArea);
                bodyColliderContainer.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            bodyCollider = GenerateCapsuleCollider(bodyColliderContainer, bodyRadius);
        }
    }

    protected virtual void CreateCollider()
    {
        generateRigidbody = false;

        if (playArea == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
            return;
        }

        VRTK_PlayerObject.SetPlayerObject(playArea.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);

        GenerateRigidbody();
        GenerateBodyCollider();

        if (playArea.gameObject.layer == 0)
        {
            playArea.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        TogglePhysics(enableBodyCollisions);
    }

    protected virtual void DestroyCollider()
    {
        if (generateRigidbody && bodyRigidbody != null)
        {
            Destroy(bodyRigidbody);
        }

        if (bodyColliderContainer != null)
        {
            Destroy(bodyColliderContainer);
        }
    }

    protected virtual void UpdateCollider()
    {
        if (bodyCollider != null)
        {

        }
    }

    protected virtual void InitControllerListeners(GameObject mappedController, bool state)
    {
        if (mappedController != null)
        {
            IgnoreCollisions(mappedController.GetComponentsInChildren<Collider>(), true);

            VRTK_InteractGrab grabbingController = mappedController.GetComponent<VRTK_InteractGrab>();
            if (grabbingController != null && ignoreGrabbedCollisions)
            {
                if (state)
                {
                    grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                    grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                }
                else
                {
                    grabbingController.ControllerGrabInteractableObject -= new ObjectInteractEventHandler(OnGrabObject);
                    grabbingController.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(OnUngrabObject);
                }
            }
        }
    }

    protected virtual IEnumerator RestoreCollisions(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        if (obj != null)
        {
            VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
            if (objScript != null && !objScript.IsGrabbed())
            {
                IgnoreCollisions(obj.GetComponentsInChildren<Collider>(), false);
            }
        }
    }

    protected virtual void IgnoreCollisions(Collider[] colliders, bool state)
    {
        if (bodyColliderContainer != null)
        {
            Collider[] playareaColliders = bodyColliderContainer.GetComponentsInChildren<Collider>();
            for (int i = 0; i < playareaColliders.Length; i++)
            {
                Collider collider = playareaColliders[i];
                if (collider.gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        Collider controllerCollider = colliders[j];
                        if (controllerCollider.gameObject.activeInHierarchy)
                        {
                            Physics.IgnoreCollision(collider, controllerCollider, state);
                        }
                    }
                }
            }
        }
    }

    protected virtual void OnGrabObject(object sender, ObjectInteractEventArgs e)
    {
        if (e.target != null)
        {
            StopCoroutine("RestoreCollisions");
            IgnoreCollisions(e.target.GetComponentsInChildren<Collider>(), true);
        }
    }

    protected virtual void OnUngrabObject(object sender, ObjectInteractEventArgs e)
    {
        if (gameObject.activeInHierarchy && playArea.gameObject.activeInHierarchy)
        {
            StartCoroutine(RestoreCollisions(e.target));
        }
    }

    protected virtual bool FloorIsGrabbedObject(RaycastHit collidedObj)
    {
        if (cachedGrabbedObjectTransform != collidedObj.transform)
        {
            cachedGrabbedObjectTransform = collidedObj.transform;
            cachedGrabbedObject = collidedObj.transform.GetComponent<VRTK_InteractableObject>();
        }
        return (cachedGrabbedObject != null && cachedGrabbedObject.IsGrabbed());
    }

    protected virtual void ApplyBodyMomentum(bool applyMomentum = false)
    {
        if (applyMomentum)
        {
            float rigidBodyMagnitude = bodyRigidbody.velocity.magnitude;
            Vector3 appliedMomentum = playAreaVelocity / (rigidBodyMagnitude < 1f ? 1f : rigidBodyMagnitude);
            bodyRigidbody.AddRelativeForce(appliedMomentum, ForceMode.VelocityChange);
        }
    }
}
