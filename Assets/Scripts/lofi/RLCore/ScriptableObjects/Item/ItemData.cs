
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "SO/Items/ItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField]
        public ItemType Type;

        [SerializeField]
        public string Name;

        [SerializeField]
        public int amount;

        [SerializeField]
        public Sprite sprite;

    }
}
