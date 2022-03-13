
using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField]
        public GameState gameState;

        [SerializeField]
        public Enemy enemyPrefab;

        Region Level;

        public Dictionary<EnemyType, UnitData> enemyDatabase;


        public void Initialize()
        {
            Level = gameState.Level;

            UnitData[] allEnemyData;
            Resources.LoadAll<UnitData>("ScriptableObjects/Units");
            allEnemyData = Resources.FindObjectsOfTypeAll<UnitData>();

            enemyDatabase = new Dictionary<EnemyType, UnitData>();

            foreach (var data in allEnemyData)
            {
                enemyDatabase.Add(data.enemyType, data);
            }

            //enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");
        }

        public List<Enemy> PopulateInitialEnemies()
        {
            Debug.Log("Populating Initial Enemies");
            List<Enemy> enemies = new List<Enemy>();

            if (Level == null)
                Debug.Log("Level is null");
            if (Level.generatorData == null)
                Debug.Log("Level generator is null");
            if (Level.generatorData.enemyProbabilityPerRow == 0)
                Debug.Log("Level prob is zero");
            //Debug.Log("Level: " + Level + " GeneratorData: " + Level.generatorData + " Prob: " + Level.generatorData.enemyProbabilityPerRow);
            int enemyProbabilityPerRow = Level.generatorData.enemyProbabilityPerRow;

            for (int y = Level.generatorData.startRoadHeight; y < Level.Height; y++)
            {
                if (Random.Range(0,100) < enemyProbabilityPerRow)
                {
                    Enemy enemy = CreateRandomEnemy(y);
                    if (enemy != null)
                        enemies.Add(enemy);
                }
            }
            return enemies;
        }

        public Enemy CreateEnemyAheadOfPlayer(UnitData enemyData)
        {
            int y = Mathf.Max(0, gameState.player.yPos + Constants.SPAWN_AHEAD_RANGE);
            Enemy enemy = SpawnEnemyOnLevel(enemyData, y, Direction.North);
            if (enemy.unitData.enemyType == EnemyType.CYCLE)
            {
                enemy.RotateTo(Direction.South);
                enemy.SetEnemyAI(new EnemyAI(enemy));
            }
            return enemy;
        }
        
        public Enemy CreateEnemyBehindPlayer(UnitData enemyData)
        {
            int y = Mathf.Max(0, gameState.player.yPos - Constants.SPAWN_BEHIND_RANGE);
            return SpawnEnemyOnLevel(enemyData, y, Direction.North);
        }

        public Enemy CreateRandomEnemy(int y)
        {
            return SpawnEnemyOnLevel(GetRandomEnemyData(), y);
        }

        public UnitData GetRandomEnemyData()
        {
            int index = Random.Range(1, (int)EnemyType.PICKUP_TRUCK);
            return enemyDatabase[(EnemyType)index];
        }

        public Enemy SpawnEnemyOnLevel(UnitData enemyData, int y, Direction direction)
        {
            Enemy enemy = SpawnEnemyOnLevel(enemyData, y);
            enemy?.RotateTo(direction);
            return enemy;
        }

        public Enemy SpawnEnemyOnLevel(UnitData enemyData, int y)
        {
            Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
            enemy.unitData = enemyData;
            var area = Level.GetOpenAreaAtY(y);
            enemy.Initialize(area.x, area.y);
            //if (!enemy.Initialize(area.x, area.y))
            //{
            //Debug.Log(enemy.name + " failed to initialize at y: " + y);
            //enemy.Die(true);
            //return null;
            //}
            enemy.SetEnemyAI(new TravelFacingDirectionEnemyAI(enemy));

            return enemy;
        }
        
        public Enemy SpawnEnemyOnLevel(UnitData enemyData, int x, int y)
        {
            Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
            enemy.unitData = enemyData;
            enemy.Initialize(x, y);

            // enemy.SetEnemyAI(new TravelFacingDirectionEnemyAI(enemy));

            return enemy;
        }

        public Enemy CreateBombAtXY(int x, int y)
        {
            Enemy enemy = SpawnEnemyOnLevel(enemyDatabase[EnemyType.BOMB], x, y);
            enemy.SetEnemyAI(new DoNothingEnemyAI(enemy));
            return enemy;
        }
        
        public Enemy CreateSemiAtXY(int x, int y)
        {
            Enemy enemy = SpawnEnemyOnLevel(enemyDatabase[EnemyType.SEMI_TRUCK], x, y);
            enemy.SetEnemyAI(new SemiTruckAI(enemy));
            return enemy;
        }
    }
}