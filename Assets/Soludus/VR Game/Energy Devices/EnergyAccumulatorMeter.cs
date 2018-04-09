using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class EnergyAccumulatorMeter : MonoBehaviour
{
    public EnergyDeviceSlot slot = null;
    public EnergyAccumulator accumulator = null;

    public Transform meter = null;
    public Transform container = null;
    public Transform shell = null;
    public Transform cubes = null;

    public int size = 10;

    public enum Side
    {
        XPositive,
        XNegative,
        ZPositive,
        ZNegative,
    }

    public Side side = Side.XNegative;

    public Animation filledAnim = null;

    public Material shellMaterial = null;

    private bool animPlayed = false;

    private void Update()
    {
        int aCount = 0;

        if (Application.isPlaying)
        {
            if (accumulator.currentValue >= accumulator.realCapacity - accumulator.capacityThreshold)
            {
                if (!animPlayed && filledAnim != null)
                    filledAnim.Play();
                animPlayed = true;
            }
            else
                animPlayed = false;

            aCount = (int)(accumulator.currentValue / (accumulator.realCapacity - accumulator.capacityThreshold) * size);
        }
        else
        {
            if (slot == null || meter == null || container == null || shell == null || cubes == null)
                return;
        }

        var scale = shell.localScale;
        scale.z = size + 0.1f;
        shell.localScale = scale;
        if (shellMaterial != null)
            shell.GetComponentInChildren<Renderer>().sharedMaterial = shellMaterial;

        var cubeP = cubes.GetChild(0);

        int i;
        for (i = 0; i < size; ++i)
        {
            Transform c;
            if (cubes.childCount <= i)
            {
                c = Instantiate(cubeP, cubes);
                c.gameObject.hideFlags = HideFlags.DontSave;
            }
            else
                c = cubes.GetChild(i);
            c.localPosition = new Vector3(0, 0, i);
            c.gameObject.SetActive(i < aCount);
            //if (material != null)
            //    c.GetComponent<Renderer>().sharedMaterial = material;
        }
        for (; i < cubes.childCount; ++i)
        {
            cubes.GetChild(i).gameObject.SetActive(false);
        }

        container.localPosition = new Vector3(0, 0, -scale.z / 2);

        var pos = meter.localPosition;
        var gsize = slot.grid.unitSize;
        gsize.x *= (slot.grid.width + 1) / 2f;
        gsize.z *= (slot.grid.height + 1) / 2f;

        switch (side)
        {
            case Side.XPositive:
                pos.x = gsize.x;
                pos.z = 0;
                meter.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Side.XNegative:
                pos.x = -gsize.x;
                pos.z = 0;
                meter.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Side.ZPositive:
                pos.z = gsize.z;
                pos.x = 0;
                meter.localEulerAngles = new Vector3(0, 90, 0);
                break;
            case Side.ZNegative:
                pos.z = -gsize.z;
                pos.x = 0;
                meter.localEulerAngles = new Vector3(0, 90, 0);
                break;
            default:
                break;
        }

        meter.localPosition = pos;
    }
}
