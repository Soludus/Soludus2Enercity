using Soludus.SharedObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    public class EnergyDeviceSlotSharedSetItem : SharedSetItem<EnergyDeviceSlot>
    {
        public EnergyDeviceSlotSharedSet m_sharedSet = null;
        public EnergyDeviceSlot m_item = null;

        public override SharedSet<EnergyDeviceSlot> sharedSet
        {
            get { return m_sharedSet; }
        }

        public override EnergyDeviceSlot item
        {
            get
            {
                if (m_item == null)
                    m_item = GetComponent<EnergyDeviceSlot>();
                return m_item;
            }
            set { m_item = value; }
        }
    }

}