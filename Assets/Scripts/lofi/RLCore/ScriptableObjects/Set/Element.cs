using UnityEngine;

namespace lofi.RLCore
{
    public class Element : MonoBehaviour
    {
        public Set set;

        protected virtual void OnEnable()
        {
            set.Add(this);
        }

        protected virtual void OnDisable()
        {
            set.Remove(this);
        }
    }
}