using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace lofi.RLCore
{
    public class SemiTruckAI : EnemyAI
    {

        int turnsUntilBomb;

        public SemiTruckAI(Enemy enemy) : base(enemy)
        {
            turnsUntilBomb = 0;
        }

        public override bool TakeTurn(GameState gameState, GameplayManager manager)
        {
            turnsUntilBomb--;

            //if(GetApproxDIstanceToPlayer(gameState) < 2 && EnemyUnit.CurrentSpeed > 1)
            if ((EnemyUnit.yPos - gameState.player.yPos) > 9)
            {
                EnemyUnit.ChangeSpeed(0);
                return true;
                //DropBomb(gameState, manager);
            }
            else if ((EnemyUnit.yPos - gameState.player.yPos) > 4)
            {
                EnemyUnit.ChangeSpeed(1);
                DropBomb(gameState, manager);
            }
            else
            {
                EnemyUnit.ChangeSpeed(EnemyUnit.CurrentSpeed + 1);

                if (EnemyUnit.unitData.shotDamage > 0)
                {
                    if (!IsAheadOfPlayer(gameState) && gameState.player.xPos == EnemyUnit.xPos)
                    {
                        EnemyUnit.Shoot();
                        manager.ShootProjectile(new Vector3(0, 1, 0), EnemyUnit.xPos, EnemyUnit.yPos);
                        SoundManager.PlaySound(SoundManager.Sound.SHOOT);
                    }
                    else if (Math.Abs(EnemyUnit.yPos - gameState.player.yPos) <= 2 &&
                                Math.Abs(EnemyUnit.xPos - gameState.player.xPos) <= 2)
                    {
                        Vector2Int loc = EnemyUnit.Shotgun();
                        manager.CreateSmokeParticle(loc.x, loc.y);
                        SoundManager.PlaySound(SoundManager.Sound.SHOTGUN);
                    }
                }
            }



            List<PathNode> path = null;
            if (EnemyUnit.Direction == Direction.North)
            {
                path = FindPathNorth(gameState);
            }
            else if (EnemyUnit.Direction == Direction.South)
            {
                path = FindPathSouth(gameState);
            }


            if (path != null && path.Count > 2)
            {
                //path.RemoveAt(0);

                return EnemyUnit.MoveTo(path[1].X, path[1].Y);

            }
            else
            {
                return MoveFacingDirection();
            }

        }

        private void DropBomb(GameState gameState, GameplayManager manager)
        {
            if (gameState.player.xPos == EnemyUnit.xPos && turnsUntilBomb <= 0)
            {
                EnemyUnit.Shoot();
                manager.SpawnBombUnit(EnemyUnit.xPos, EnemyUnit.yPos-1);
                turnsUntilBomb = Random.Range(2, 5);
                //SoundManager.PlaySound(SoundManager.Sound.SHOOT);
            }
        }
    }
}