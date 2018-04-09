using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.SharedObjects
{

    public class GameObjectSharedSetItem : SharedSetItem<GameObject>
    {
        public GameObjectSharedSet m_sharedSet = null;
        public GameObject m_item = null;

        public override SharedSet<GameObject> sharedSet
        {
            get { return m_sharedSet; }
        }

        public override GameObject item
        {
            get
            {
                if (m_item == null)
                    m_item = gameObject;
                return m_item;
            }
            set { m_item = value; }
        }
    }

}