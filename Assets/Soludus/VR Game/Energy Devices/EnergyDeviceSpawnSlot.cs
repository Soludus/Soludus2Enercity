using Soludus;
using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class EnergyDeviceSpawnSlot : MonoBehaviour
{
    public VRTK_SnapDropZone snapDropZone = null;

    public int maxHeight = 2;
    public int maxWidth = 2;

    public EnergyValue requireCredits = null;

    public float spawnInterval = 2.0f;

    public bool snapDropZoneActive = false;

    [Layer]
    public int layerInSlot = 0;

    public EnergyDevice devicePrefab = null;

    private bool pushing = false;
    private int normalLayer;

    private void OnEnable()
    {
        RegisterCallbacks();
    }

    private void OnDisable()
    {
        DeregisterCallbacks();
    }

    private void Update()
    {
        if (snapDropZone.GetCurrentSnappedObject() == null && !pushing)
        {
            if (requireCredits == null || requireCredits.value >= 1)
            {
                StartCoroutine(PushNextAfterDelay(spawnInterval));
            }
        }
    }

    private void RegisterCallbacks()
    {
        snapDropZone.ObjectUnsnappedFromDropZone += OnUnsnap;
    }

    private void DeregisterCallbacks()
    {
        snapDropZone.ObjectUnsnappedFromDropZone -= OnUnsnap;
    }


    private void OnUnsnap(object sender, SnapDropZoneEventArgs e)
    {
        if (requireCredits != null)
            requireCredits.Increment(-1);
        SetSnapDropZoneActive(false);
        e.snappedObject.layer = normalLayer;
    }

    public void PushNextToSnapZone()
    {
        StartCoroutine(PushNextOnEndOfFrame());
    }

    private IEnumerator PushNextAfterDelay(float delay)
    {
        pushing = true;
        yield return new WaitForSeconds(delay);
        DoPushNext();
        pushing = false;
    }

    private IEnumerator PushNextOnEndOfFrame()
    {
        pushing = true;
        yield return new WaitForEndOfFrame();
        DoPushNext();
        pushing = false;
    }

    private void DoPushNext()
    {
        normalLayer = devicePrefab.gameObject.layer;
        devicePrefab.gameObject.layer = layerInSlot;
        var nextDevice = Instantiate(devicePrefab, snapDropZone.transform.position + snapDropZone.transform.up * 0.05f, snapDropZone.transform.rotation);

        devicePrefab.gameObject.layer = normalLayer;

        //nextDevice.transform.localScale = 0.02f * Vector3.one;
        nextDevice.grid.width = Random.Range(1, maxWidth + 1);
        nextDevice.grid.height = Random.Range(1, maxHeight + 1);
        SetSnapDropZoneActive(true);
        snapDropZone.ForceSnap(nextDevice.gameObject);
    }

    private void SetSnapDropZoneActive(bool active)
    {
        //snapDropZone.gameObject.SetActive(active);
        snapDropZone.GetComponent<Collider>().enabled = snapDropZoneActive & active;
    }
}
