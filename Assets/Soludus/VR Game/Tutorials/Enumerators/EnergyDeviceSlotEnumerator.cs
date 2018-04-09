using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDeviceSlotEnumerator : GameObjectEnumerator
{
    public List<EnergyDeviceType> types = new List<EnergyDeviceType>();

    public override IEnumerable<GameObject> GetObjects()
    {
        for (int i = 0; i < types.Count; ++i)
        {
            var type = types[i];
            if (type != null)
            {
                for (int j = 0; j < type.allSlots.Count; ++j)
                {
                    yield return type.allSlots[j].gameObject;
                }
            }
        }
    }
}
