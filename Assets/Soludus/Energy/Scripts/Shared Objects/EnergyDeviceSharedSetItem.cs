using Soludus.SharedObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.Energy
{

    public class EnergyDeviceSharedSetItem : SharedSetItem<EnergyDevice>
    {
        public EnergyDeviceSharedSet m_sharedSet = null;
        public EnergyDevice m_item = null;

        public override SharedSet<EnergyDevice> sharedSet
        {
            get { return m_sharedSet; }
        }

        public override EnergyDevice item
        {
            get
            {
                if (m_item == null)
                    m_item = GetComponent<EnergyDevice>();
                return m_item;
            }
            set { m_item = value; }
        }
    }

}