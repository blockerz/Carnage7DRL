using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "UnitType", menuName = "SO/Units/Type/UnitType")]
    public class UnitType : ScriptableObject
    {
        [SerializeField]
        string Type;
    }
}
