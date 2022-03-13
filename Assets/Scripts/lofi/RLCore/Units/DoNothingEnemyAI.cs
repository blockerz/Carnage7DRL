using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class DoNothingEnemyAI : EnemyAI
    {


        public DoNothingEnemyAI(Enemy enemy) : base(enemy)
        {

        }

        public override bool TakeTurn(GameState gameState, GameplayManager manager)
        {
            return true;
        }

    }
}