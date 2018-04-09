using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDeviceCountProvider : MonoBehaviour, IValueProvider<int>, IValueProvider<float>
{
    public List<EnergyDeviceType> types = new List<EnergyDeviceType>();

    public bool countAttached = true;
    public bool countUnattached = true;

    public void InitValue() { }

    public int GetValue()
    {
        int c = 0;

        if (!countAttached && !countUnattached)
            return 0;
        if (countAttached && countUnattached)
        {
            for (int i = 0; i < types.Count; ++i)
            {
                c += types[i].allDevices.Count;
            }
        }
        else
        {
            for (int i = 0; i < types.Count; ++i)
            {
                var devs = types[i].allDevices;
                for (int j = 0; j < devs.Count; ++j)
                {
                    if (devs[j].slot != null)
                    {
                        if (countAttached)
                            ++c;
                    }
                    else
                    {
                        if (countUnattached)
                            ++c;
                    }
                }
            }
        }

        return c;
    }

    float IValueProvider<float>.GetValue()
    {
        return GetValue();
    }
}
