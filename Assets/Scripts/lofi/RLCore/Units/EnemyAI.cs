using System;
using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class EnemyAI
    {
        protected Enemy EnemyUnit;
        protected Pathfinding pathfinding;

        public EnemyAI(Enemy enemyUnit)
        {
            EnemyUnit = enemyUnit;
        }

        public virtual bool TakeTurn(GameState gameState, GameplayManager manager)
        {
            if (pathfinding == null)    
                pathfinding = new Pathfinding(gameState.Level);

            var path = FindPathToPlayer(gameState);

            if (path != null && path.Count > 1)
            {
                if (gameState.player.yPos > path[1].Y || EnemyUnit.Direction != Direction.North)
                {
                    EnemyUnit.RotateTo(Direction.North);
                }                
                else if (gameState.player.yPos < path[1].Y || EnemyUnit.Direction != Direction.South)
                {
                    EnemyUnit.RotateTo(Direction.South);
                }
                return EnemyUnit.MoveTo(path[1].X, path[1].Y);
            }
            else
            {
                MoveFacingDirection();
            }

            return false;
        }

        public float GetApproxDIstanceToPlayer(GameState gameState)
        {
            int xDist = Mathf.Abs(gameState.player.xPos - EnemyUnit.xPos);
            int yDist = Mathf.Abs(gameState.player.yPos - EnemyUnit.yPos);
            int remaining = Mathf.Abs(xDist - yDist);

            return Mathf.Min(xDist, yDist) * remaining;
        }

        public bool IsAheadOfPlayer(GameState gameState)
        {            
            //int yDist = Mathf.Abs(gameState.player.yPos - EnemyUnit.yPos);
            return (EnemyUnit.yPos > gameState.player.yPos);
        }

        //public virtual bool TakeTurn(GameState gameState)
        //{
        //    if (pathfinding == null)
        //        pathfinding = new Pathfinding(gameState.Level);
        //    bool moved = false;

        //    for (int s = 0; s < EnemyUnit.CurrentSpeed; s++)
        //    {

        //        var path = FindPathToPlayer(gameState);


        //        if (path != null && path.Count > 1)
        //        {
        //            moved = EnemyUnit.MoveTo(path[1].X, path[1].Y);
        //        }
        //        else
        //        {
        //            MoveFacingDirection();
        //        }
        //    }

        //    return moved;
        //}
        protected List<PathNode> FindPathToPlayer(GameState gameState)
        {
            if (pathfinding == null)
                pathfinding = new Pathfinding(gameState.Level);

            return pathfinding.FindPath(EnemyUnit.xPos, EnemyUnit.yPos, gameState.player.xPos, gameState.player.yPos, EnemyUnit);
        }

        protected List<PathNode> FindPathNorth(GameState gameState)
        {
            if (pathfinding == null)
                pathfinding = new Pathfinding(gameState.Level);

            Vector2Int openRoadPos = gameState.Level.GetOpenAreaAtY(Math.Min(gameState.Level.Height, EnemyUnit.yPos + (Constants.ACTIVE_TILE_HEIGHT / 2)));

            if (openRoadPos == null || openRoadPos.x == -1)
                return null;

            return pathfinding.FindPath(EnemyUnit.xPos, EnemyUnit.yPos, openRoadPos.x, openRoadPos.y, EnemyUnit);
        }
        
        protected List<PathNode> FindPathSouth(GameState gameState)
        {
            if (pathfinding == null)
                pathfinding = new Pathfinding(gameState.Level);

            Vector2Int openRoadPos = gameState.Level.GetOpenAreaAtY(Math.Max(0, EnemyUnit.yPos - (Constants.ACTIVE_TILE_HEIGHT / 2)));

            if (openRoadPos == null || openRoadPos.x == -1)
                return null;

            return pathfinding.FindPath(EnemyUnit.xPos, EnemyUnit.yPos, openRoadPos.x, openRoadPos.y, EnemyUnit);
        }

        protected bool MoveFacingDirection()
        {
            if (EnemyUnit.Direction == Direction.North)
            {
                return EnemyUnit.MoveBy(0, 1);
            }
            else if (EnemyUnit.Direction == Direction.South)
            {
                return EnemyUnit.MoveBy(0, -1);
            }
            else if (EnemyUnit.Direction == Direction.East)
            {
                return EnemyUnit.MoveBy(1, 0);
            }
            else if (EnemyUnit.Direction == Direction.West)
            {
                return EnemyUnit.MoveBy(-1, 0);
            }

            return false;
        }
    }
}