using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus.SharedObjects
{

    public abstract class SharedSet<T> : ScriptableObject
    {
        public List<T> items = new List<T>();

        public void Add(T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }

        public void Remove(T item)
        {
            items.Remove(item);
        }
    }

}