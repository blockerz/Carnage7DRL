
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "SO/Units/UnitData")]
    public class UnitData : ScriptableObject
    {
        [SerializeField]
        public UnitType Type;
        
        [SerializeField]
        public EnemyType enemyType;

        [SerializeField]
        public string Name;
        
        [SerializeField]
        public string Description;

        [SerializeField]
        public Sprite sprite;

        [SerializeField]
        public Color color;

        [SerializeField]
        public GameState gameState;

        [SerializeField]
        public int tileWidth;

        [SerializeField]
        public int tileHeight;

        [SerializeField]
        public int maxHealth;

        [SerializeField]
        public Direction defaultDirection;

        [SerializeField]
        public int maxSpeed;

        [SerializeField]
        public int crashDamage;

        [SerializeField]
        public int shotDamage;

        [SerializeField]
        public int score;

        [SerializeField]
        public bool flammable;

        [SerializeField]
        public bool bulletproof;

        [SerializeField]
        public bool bombable;

        [SerializeField]
        public bool entersTop;

        [SerializeField]
        public bool entersBottom;
    }
}