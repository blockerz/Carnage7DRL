using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        public List<T> Items = new List<T>();

        public void Add(T element)
        {
            if (!Items.Contains(element))
                Items.Add(element);
        }

        public void Remove(T element)
        {
            if (Items.Contains(element))
                Items.Remove(element);
        }
    }
}