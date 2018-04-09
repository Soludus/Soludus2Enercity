using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_DragCamera : MonoBehaviour
{
    [Header("Grabbing")]
    [SerializeField]
    private VRTK_ControllerEvents m_controllerEvents = null;
    [SerializeField]
    private LayerMask m_layersToIgnore = 0;
    [SerializeField]
    private VRTK_CustomRaycast m_customRaycast = null;
    [SerializeField]
    private float m_maxDistance = 2000;

    [SerializeField]
    private bool m_noGrabIfTouchingObject = true;
    [SerializeField]
    private bool m_noGrabIfTouchingGrabbableObject = true;

    private bool m_grabbed = false;
    private Vector3 m_grabPoint = Vector3.zero;
    private Plane m_plane = new Plane(Vector3.up, Vector3.zero);

    private Transform m_playArea = null;
    private Transform m_headset = null;

    [Header("Dragging")]
    [SerializeField]
    private bool m_coneDrag = true;
    [SerializeField]
    private float m_coneNormalMinAngle = 100;

    // set when dragging starts to prevent angles smaller than this
    // if this is below m_coneNormalMinAngle, every movement that increases the angle will cause this to be set to current min angle until m_coneNormalMinAngle is reached
    private float m_initialConeNormalMinAngle = 0;

    // TODO: add a temporary snap to the filtered controller to prevent immediately teleporting when drag starts to match m_coneNormalMinAngle

    [SerializeField]
    [Range(1, 10)]
    private float m_inertiaDrag = 5;

    private Vector3 m_velocity = Vector3.zero;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    private void OnEnable()
    {
        m_controllerEvents = m_controllerEvents ? m_controllerEvents : GetComponent<VRTK_ControllerEvents>();
        m_playArea = VRTK_DeviceFinder.PlayAreaTransform();
        m_headset = VRTK_DeviceFinder.HeadsetTransform();

        m_controllerEvents.TriggerPressed += OnTriggerClicked;
        m_controllerEvents.TriggerReleased += OnTriggerUnclicked;
    }

    private void OnDisable()
    {
        m_controllerEvents.TriggerPressed -= OnTriggerClicked;
        m_controllerEvents.TriggerReleased -= OnTriggerUnclicked;
    }

    private bool IsGrabValid()
    {
        var uiPointer = m_controllerEvents.GetComponent<VRTK_UIPointer>();
        if (uiPointer != null && uiPointer.hoveringElement != null)
            return false;

        var touch = m_controllerEvents.GetComponent<VRTK_InteractTouch>();
        //Debug.LogError("VRTK_InteractTouch: " + touch);
        if (touch == null) return true;
        var obj = touch.GetTouchedObject();
        //Debug.LogError("GetTouchedObject(): " + obj);
        if (obj == null) return true;
        if (m_noGrabIfTouchingObject) return false;
        var iObj = obj.GetComponent<VRTK_InteractableObject>();
        //Debug.LogError("VRTK_InteractableObject: " + iObj);
        if (iObj == null) return true;
        //Debug.LogError("isGrabbable: " + iObj.isGrabbable);
        if (m_noGrabIfTouchingGrabbableObject)
            return !iObj.isGrabbable; // FIXME: iObj.isGrabbable returns incorrect value in build (not in editor)

        return false;
    }

    private void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        m_grabbed = IsGrabValid() && ControllerRaycast(out m_grabPoint);
        if (m_grabbed)
        {
            GetComponent<VRTK_TransformFollow>().smoothsRotation = true;
            GetComponent<VR_TransformClampAngle>().fromDirection = m_plane.normal;

            m_initialConeNormalMinAngle = Vector3.Angle(m_plane.normal, transform.forward);
            m_initialConeNormalMinAngle = Mathf.Min(m_initialConeNormalMinAngle, m_coneNormalMinAngle);
            GetComponent<VR_TransformClampAngle>().minAngle = m_initialConeNormalMinAngle;

            GetComponent<VR_TransformClampAngle>().enabled = true;
        }
    }

    private void OnTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        GetComponent<VRTK_TransformFollow>().smoothsRotation = false;
        GetComponent<VR_TransformClampAngle>().enabled = false;
        m_grabbed = false;
    }

    private bool ControllerRaycast(out Vector3 hitPoint)
    {
        Ray ray = new Ray(m_controllerEvents.transform.position, transform.forward);
        RaycastHit hit;

        if (VRTK_CustomRaycast.Raycast(m_customRaycast, ray, out hit, m_layersToIgnore, m_maxDistance))
        {
            hitPoint = hit.point;

            if (m_coneDrag)
            {
                Vector3 groundPos = m_playArea.position;

                // find ground below play area
                RaycastHit groundHit;
                Ray groundRay = new Ray(new Vector3(groundPos.x, m_headset.position.y, groundPos.z), Vector3.down);
                if (VRTK_CustomRaycast.Raycast(m_customRaycast, groundRay, out groundHit, m_layersToIgnore))
                {
                    groundPos = groundHit.point;
                }

                // a "ramp"
                m_plane = new Plane(groundPos, hitPoint, groundPos + Vector3.Cross(Vector3.up, hitPoint - groundPos));
            }
            else
            {
                m_plane = new Plane(Vector3.up, hitPoint);
            }
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }

    private bool ControllerPlaneRaycast(out Vector3 hitPoint)
    {
        // flip the plane to ensure controller is on the positive side
        //Debug.DrawRay(grabPoint, (m_controllerEvents.transform.position - grabPoint), Color.red);
        //Debug.DrawRay(grabPoint, plane.normal * 10, Color.blue);
        float dot = Vector3.Dot((m_controllerEvents.transform.position - m_grabPoint).normalized, m_plane.normal);
        if (dot < 0)
        {
            m_plane = m_plane.flipped;
            GetComponent<VR_TransformClampAngle>().fromDirection = m_plane.normal;
        }

        float hitDist;
        Ray ray = new Ray(m_controllerEvents.transform.position, transform.forward);
        if (m_plane.Raycast(ray, out hitDist))
        {
            hitPoint = ray.origin + ray.direction * hitDist;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

    private void Update()
    {
        if (m_playArea == null)
            return;

        if (m_grabbed)
        {
            Vector3 lastPlayAreaPos = m_playArea.position;

            Vector3 hitPoint;
            if (ControllerPlaneRaycast(out hitPoint))
            {
                m_playArea.position += m_grabPoint - hitPoint;
            }

            if (m_initialConeNormalMinAngle < m_coneNormalMinAngle)
            {
                m_initialConeNormalMinAngle = Mathf.Max(m_initialConeNormalMinAngle, Vector3.Angle(m_plane.normal, transform.forward));
                GetComponent<VR_TransformClampAngle>().minAngle = m_initialConeNormalMinAngle;
            }

            // update velocity for inertia
            m_velocity = (m_playArea.position - lastPlayAreaPos) / Time.deltaTime;
        }
        else // apply inertia only when not grabbed
        {
            m_velocity *= 1 - m_inertiaDrag * Time.deltaTime;
            m_playArea.position += m_velocity * Time.deltaTime;
        }

        //Debug.Log("Grab playarea pos update " + Time.frameCount);
    }

}
