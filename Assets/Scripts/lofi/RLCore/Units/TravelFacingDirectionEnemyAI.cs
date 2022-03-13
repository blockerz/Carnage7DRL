using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class TravelFacingDirectionEnemyAI : EnemyAI
    {
         

        public TravelFacingDirectionEnemyAI(Enemy enemy) : base(enemy)
        {
            
        }

        public override bool TakeTurn(GameState gameState, GameplayManager manager)
        {
            //if(GetApproxDIstanceToPlayer(gameState) < 2 && EnemyUnit.CurrentSpeed > 1)
            if(IsAheadOfPlayer(gameState))
            {
                EnemyUnit.ChangeSpeed(1);
            }
            else
            {
                EnemyUnit.ChangeSpeed(EnemyUnit.CurrentSpeed + 1);

                if(EnemyUnit.unitData.shotDamage > 0)
                {
                    if (gameState.player.xPos == EnemyUnit.xPos)
                    {
                        EnemyUnit.Shoot();
                        manager.ShootProjectile(new Vector3(0, 1, 0), EnemyUnit.xPos, EnemyUnit.yPos);
                        SoundManager.PlaySound(SoundManager.Sound.SHOOT);
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

        //public override bool TakeTurn(GameState gameState)
        //{
        //    bool moved = false;

        //    for (int s = 0; s < EnemyUnit.CurrentSpeed; s++)
        //    {
        //        if (path == null || path.Count <= 2)
        //        {
        //            if (EnemyUnit.Direction == Direction.North)
        //            {
        //                path = FindPathNorth(gameState);
        //            }
        //            else if (EnemyUnit.Direction == Direction.South)
        //            {
        //                path = FindPathSouth(gameState);
        //            }
        //        }

        //        if (path != null && path.Count > 1)
        //        {
        //            path.RemoveAt(0);

        //            moved = EnemyUnit.MoveTo(path[0].X, path[0].Y);

        //            if (!moved)
        //                path = null;

        //        }
        //        else
        //        {
        //            moved = MoveFacingDirection();
        //        }
        //    }

        //    return moved;
        //}
    }
}