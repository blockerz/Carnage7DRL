
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName ="StringVariable",menuName ="SO/Variables/String")]
    public class StringVariable : ScriptableObject
    {
        [SerializeField]
        private string value = "";

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
