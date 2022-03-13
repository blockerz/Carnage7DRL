using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "AreaType", menuName = "SO/Area/Type/AreaType")]
    public class AreaType : ScriptableObject
    {
        [SerializeField]
        string Type;
    }
}
