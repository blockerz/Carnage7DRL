
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "AreaData", menuName = "SO/Area/AreaData")]
    public class AreaData : ScriptableObject
    {

        [SerializeField]
        public AreaID AreaID;

        //[SerializeField]
        //public AreaType Type;

        [SerializeField]
        public string Name;

        [SerializeField]
        public string Description;

        [SerializeField]
        public int spriteXIndex;

        [SerializeField]
        public int spriteYIndex;

        [SerializeField]
        public bool isPassable;

        [SerializeField]
        public bool isVisible;

        [SerializeField]
        public bool isHazard;

        [SerializeField]
        public int damageAmount;

        [SerializeField]
        public int cost;
    }
}