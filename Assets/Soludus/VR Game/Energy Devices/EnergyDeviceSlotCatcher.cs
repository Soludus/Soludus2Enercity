using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.Events;
using Soludus.Energy;

/// <summary>
/// Catches and attaches thrown or placed devices to a holder.
/// The Collider of this slot is set to trigger and objects with EnergyDevice, VRTK_InteractableObject and Rigidbody will be attached to the slot when they touch the trigger and there is slots available.
/// </summary>
public class EnergyDeviceSlotCatcher : MonoBehaviour
{
    public EnergyDeviceSlot slot;

    [SerializeField, Tooltip("This object is scaled to fit the slot Grid Data.")]
    private Transform m_catchArea = null;

    public UnityEvent onAttached = new UnityEvent();

    private void OnEnable()
    {
        if (slot == null)
            slot = GetComponent<EnergyDeviceSlot>();
    }

    private void Update()
    {
        // update catch area width/height
        var size = slot.grid.size;
        size.y = m_catchArea.localScale.y;
        m_catchArea.localScale = size;
        //GetComponentInChildren<Collider>().isTrigger = true;
    }

    /// <summary>
    /// Try to attach the device to the holder at the specified slot.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool TryAttach(EnergyDevice device, int x, int y)
    {
        if (!slot.TryAddToPosition(device, x, y))
            return false;

        AttachDevice(device, x, y);
        return true;
    }

    /// <summary>
    /// Try to attach the device to the holder. A slot is selected automatically. x and y will contain the attached slot position.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool TryAttach(EnergyDevice device, out int x, out int y)
    {
        // try a reasonable index based on rounded position of device transform
        GetGridSpacePosition(device, out x, out y);

        if (!slot.TryAddToPosition(device, x, y))
            return false;

        // TODO: try all indexes sorted by distance

        AttachDevice(device, x, y);
        return true;
    }

    private void GetGridSpacePosition(EnergyDevice device, out int x, out int y)
    {
        Vector3 p = WorldToGridSpace(device.transform.position);
        p -= new Vector3((device.grid.width - 1) * 0.5f, 0, (device.grid.height - 1) * 0.5f);
        x = Mathf.RoundToInt(p.x);
        y = Mathf.RoundToInt(p.z);
    }

    private Vector3 WorldToGridSpace(Vector3 point)
    {
        point = slot.transform.InverseTransformPoint(point);

        var unitSize = slot.grid.unitSize;
        point.x /= unitSize.x;
        point.y /= unitSize.y;
        point.z /= unitSize.z;

        Vector3 gridHalfOffset = new Vector3((slot.grid.width - 1) * 0.5f, 0, (slot.grid.height - 1) * 0.5f);
        point += gridHalfOffset;

        return point;
    }

    private Vector3 GridToWorldSpace(Vector3 point)
    {
        Vector3 gridHalfOffset = new Vector3((slot.grid.width - 1) * 0.5f, 0, (slot.grid.height - 1) * 0.5f);
        point -= gridHalfOffset;

        var unitSize = slot.grid.unitSize;
        point.x *= unitSize.x;
        point.y *= unitSize.y;
        point.z *= unitSize.z;

        return slot.transform.TransformPoint(point);
    }

    private bool FindNearestSlot(EnergyDevice device, out int x, out int y)
    {
        GetGridSpacePosition(device, out x, out y);

        if (slot.IsValidPosition(device, x, y))
        {
            return true;
        }

        // TODO: try all indices sorted by distance to gridPos
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other);
        GameObject o = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        //Debug.Log(gameObject + " triggered by " + o);

        var sp = o.GetComponent<EnergyDevice>();
        if (sp == null || sp.slot != null || sp.type != slot.acceptedType)
            return;

        int x, y;
        //if (!FindNearestSlot(sp, out x, out y))
        //    return;

        var vrio = o.GetComponent<VRTK_InteractableObject>();
        if (vrio != null)
        {
            if (vrio.IsInSnapDropZone())
            {
                return;
            }
            if (vrio.IsGrabbed())
            {
                return;
            }
        }

        //Debug.Log(x + " " + y);

        if (slot.TryAddToFirstAvailablePosition(sp, out x, out y))
        {
            sp.forceDeactive = true;
            AttachDevice(sp, x, y);
        }
    }

    private void AttachDevice(EnergyDevice device, int x, int y)
    {
        Vector3 localPos = new Vector3(
            (x + (device.grid.width - 1f) * 0.5f) * slot.grid.unitSize.x,
            0,
            (y + (device.grid.height - 1f) * 0.5f) * slot.grid.unitSize.z
            );

        Vector3 gridHalfOffset = new Vector3((slot.grid.width - 1) * slot.grid.unitSize.x * 0.5f, 0, (slot.grid.height - 1) * slot.grid.unitSize.z * 0.5f);
        localPos -= gridHalfOffset;

        //device.gameObject.SetCollidersEnabled(false);
        device.grid.unitSize.y = 0.13f;
        //Vector3 targetScale = Vector3.one;
        Vector3 targetScale = device.transform.localScale;

        device.transform.SetParent(transform, true);

        var rb = device.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        var vrio = device.GetComponent<VRTK_InteractableObject>();
        if (vrio != null)
        {
            vrio.isGrabbable = false;
        }

        StartCoroutine(LerpDeviceIntoPlace(device, localPos, Quaternion.identity, targetScale));

        //device.transform.localRotation = Quaternion.identity;
        //device.transform.localPosition = pos;
    }

    private IEnumerator LerpDeviceIntoPlace(EnergyDevice device, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
    {
        var devTransfrom = device.transform;
        Vector3 currentPos = devTransfrom.localPosition;
        Quaternion currentRot = devTransfrom.localRotation;
        Vector3 currentScale = devTransfrom.localScale;

        while (true)
        {
            var posDist = Vector3.Distance(currentPos, targetPosition);
            var posAlpha = Time.deltaTime * 8;
            currentPos = Vector3.Lerp(currentPos, targetPosition, posAlpha);
            devTransfrom.localPosition = currentPos;

            var rotDist = Quaternion.Angle(currentRot, targetRotation);
            var rotAlpha = Time.deltaTime * 8;
            currentRot = Quaternion.Lerp(currentRot, targetRotation, rotAlpha);
            devTransfrom.localRotation = currentRot;
            
            var scaleAlpha = Time.deltaTime * 8;
            currentScale = Vector3.Lerp(currentScale, targetScale, scaleAlpha);
            devTransfrom.localScale = currentScale;

            if (posDist < 0.01f && rotDist < 0.01f)
            {
                devTransfrom.localPosition = targetPosition;
                devTransfrom.localRotation = targetRotation;
                devTransfrom.localScale = targetScale;
                break;
            }

            yield return null;
        }

        slot.SetElementsActive(device, true, 0);
        device.forceDeactive = false;
        onAttached.Invoke();
    }
}
