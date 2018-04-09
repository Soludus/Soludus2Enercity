using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachDeviceObjective : Objective
{
    public List<EnergyDeviceType> types = new List<EnergyDeviceType>();
    public int requiredCount = 1;
    public bool useTotalCount = false;

    private int attachedCount = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        attachedCount = useTotalCount ? 0 : CountAttachedPanels();
    }

    private int CountAttachedPanels()
    {
        int count = 0;
        for (int i = 0; i < types.Count; ++i)
        {
            var type = types[i];
            for (int j = 0; j < type.allSlots.Count; ++j)
            {
                count += type.allSlots[j].attachedDevices.Count;
            }
        }
        return count;
    }

    public override bool IsComplete()
    {
        return CountAttachedPanels() - attachedCount >= requiredCount;
    }
}
