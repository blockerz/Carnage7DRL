
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "LevelGeneratorData", menuName = "SO/Map/LevelGeneratorData")]
    public class LevelGeneratorData : ScriptableObject
    {

        [SerializeField]
        public int levelHeight;

        [SerializeField]
        public int startRoadHeight;
        
        [SerializeField]
        public int endRoadHeight;

        [SerializeField]
        public int startRoadWidth;
        
        [SerializeField]
        public int startRoadLeftOffset;

        [SerializeField]
        public int startPlayerX;
        
        [SerializeField]
        public int startPlayerY;

        [SerializeField]
        public int minRoadWidth;

        [SerializeField]
        public int maxRoadWidth;

        [SerializeField]
        public float narrowRatio;

        [SerializeField]
        public float widenRatio;

        [SerializeField]
        public float curveRatio;

        [SerializeField]
        public int splitDistance;

        [SerializeField]
        public int splitSteps;

        [SerializeField]
        public int splitStart;

        [SerializeField]
        public int splitEnd;

        [SerializeField]
        public float splitPercent;

        [SerializeField]
        public AreaID primaryBackground;

        [SerializeField]
        public AreaID primaryRoadEdge;

        [SerializeField]
        public AreaID primaryRoad;

        [SerializeField]
        public float straightRoadChance;

        [SerializeField]
        public float curvyRoadChance;
        
        [SerializeField]
        public float narrowRoadChance;
        
        [SerializeField]
        public float narrowLeftRoadChance;
        
        [SerializeField]
        public float narrowRightRoadChance;
        
        [SerializeField]
        public float widenRoadChance;
        
        [SerializeField]
        public float splitRoadChance;
        
        [SerializeField]
        public float bridgeRoadChance;

        [SerializeField]
        public int enemyProbabilityPerRow;

        [SerializeField]
        public int enemyProbabilityBehindPerTurn;

        [SerializeField]
        public int enemyProbabilityAheadPerTurn;
    }
}