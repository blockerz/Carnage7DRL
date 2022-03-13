
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "GameState", menuName = "SO/GameState/GameState")]
    public class GameState : ScriptableObject
    {
        [SerializeField]
        public GameStateType currentState;

        //[SerializeField]
        //public GameStateType lastState;

        [SerializeField]
        public int gameStage;

        [SerializeField]
        public int gameScore;

        [SerializeField]
        public int highScore;

        [SerializeField]
        public int stageTurns;

        [SerializeField]
        public int playerHealth;

        [SerializeField]
        public int shotgunAmmo;

        [SerializeField]
        public int gameTurns;

        [SerializeField]
        public Player player;

        [SerializeField]
        public Region Level;
        
        [SerializeField]
        public bool gameOver;
        
        [SerializeField]
        public bool gameWon;


    }
}
