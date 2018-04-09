using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.SharedObjects
{

    public abstract class SharedSetItem<T> : MonoBehaviour
    {
        /// <summary>
        /// The set this panel is added to.
        /// </summary>
        public abstract SharedSet<T> sharedSet
        {
            get;
        }

        public abstract T item
        {
            get;
            set;
        }

        private void OnEnable()
        {
            if (item == null)
                item = GetComponent<T>();
            Add();
        }

        private void OnDisable()
        {
            Remove();
        }

        private void Add()
        {
            if (sharedSet != null)
                sharedSet.Add(item);
        }

        private void Remove()
        {
            if (sharedSet != null)
                sharedSet.Remove(item);
        }
    }

}