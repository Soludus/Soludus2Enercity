using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDeviceSlotMaterialSwitch : MonoBehaviour
{
    public EnergyDeviceSlot slot = null;
    public GridRenderer grid = null;
    public EnergyAccumulatorMeter meter = null;

    public Material normalMaterial = null;
    public Material reducedMaterial = null;

    private void OnEnable()
    {
        if (slot == null)
            slot = GetComponent<EnergyDeviceSlot>();
        if (grid == null)
            grid = GetComponentInChildren<GridRenderer>();
        if (meter == null)
            meter = GetComponentInChildren<EnergyAccumulatorMeter>();
    }

    private void Update()
    {
        var mat = slot.lastEnergyEffect.deviceEffectMultiplier >= 1 ? normalMaterial : reducedMaterial;
        if (grid != null)
            grid.material = mat;
        if (meter != null)
            meter.shellMaterial = mat;
    }
}
