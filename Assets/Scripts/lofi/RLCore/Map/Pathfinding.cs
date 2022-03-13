using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class Pathfinding
    {
        private const float MOVE_STRAIGHT_COST = 1f; // 1.0
        private const float MOVE_DIAGONAL_COST = 1.4f; // 1.4

        public Region Region { get; }

        private List<PathNode> openList;
        private List<PathNode> closedList;

        int activeZoneBottom;
        int activeZoneTop;

        public Pathfinding(Region region)
        {
            this.Region = region;
            activeZoneTop = Region.Height;
            activeZoneBottom = 0;
        }

        public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            Vector2Int startPos = Region.GetXYFromWorldPosition(startWorldPosition);
            Vector2Int endPos = Region.GetXYFromWorldPosition(endWorldPosition);

            List<PathNode> path = FindPath(startPos.x, startPos.y, endPos.x, endPos.y);

            if (path != null)
            {
                List<Vector3> vectorPath = new List<Vector3>();

                foreach (PathNode pathNode in path)
                {
                    vectorPath.Add(new Vector3(pathNode.X * Region.AreaWorldSizeX, pathNode.Y * Region.AreaWorldSizeY)
                        + new Vector3(0.5f * Region.AreaWorldSizeX, 0.5f * Region.AreaWorldSizeY));
                }
                return vectorPath;
            }
            
            return null;
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY, Unit unit = null)
        {
            activeZoneBottom = Math.Min(startY, endY);
            activeZoneTop = Math.Max(startY, endY);

            for (int x = 0; x < Region.Width; x++)
            {
                for (int y = activeZoneBottom; y <= activeZoneTop; y++)
                {
                    Area area = Region.GetAreaAtXY(x, y);
                    PathNode pathNode = area?.PathNode;

                    if (pathNode == null)
                    {
                        pathNode = Region.GetAreaAtXY(x, y).PathNode = new PathNode(x, y, area.Data.cost);
                    }

                    if (area.Data.isPassable && (area.UnitPresent == null || area.UnitPresent == unit))
                        pathNode.IsPassable = true;
                    else
                        pathNode.IsPassable = false;

                    pathNode.GCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.ParentNode = null;
                }
            }

            PathNode startNode = Region.GetAreaAtXY(startX, startY)?.PathNode;
            PathNode endNode = Region.GetAreaAtXY(endX, endY)?.PathNode;
            endNode.IsPassable = true; // hack in case endnode has a unit

            if (startNode == null || endNode == null)
            {
                Debug.Log("No path found: (" + startX + "," + startY + ") (" + endX + "," + endY + ")");
                return null;
            }

            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);

                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighborNode in GetNeighborList(currentNode))
                {
                    if (closedList.Contains(neighborNode) || !neighborNode.IsPassable)
                        continue;

                    float tempGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighborNode);

                    if (tempGCost < neighborNode.GCost)
                    {
                        neighborNode.ParentNode = currentNode;
                        neighborNode.GCost = tempGCost + neighborNode.MovementCost;
                        neighborNode.HCost = CalculateDistanceCost(neighborNode, endNode);
                        neighborNode.CalculateFCost();

                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }

            }

            Debug.Log("No path found: (" + startX + "," + startY + ") (" + endX + "," + endY + ")");
            // no path found
            return null;
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;

            while(currentNode.ParentNode != null)
            {
                path.Add(currentNode.ParentNode);
                currentNode = currentNode.ParentNode;
            }

            path.Reverse();

            return path;
        }

        private List<PathNode> GetNeighborList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            if (currentNode.X - 1 >= 0)
            {
                // Left
                neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y));

                // Left Down
                if (currentNode.Y - 1 >= activeZoneBottom) 
                    neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y - 1));

                // Left Up
                if (currentNode.Y + 1 <= activeZoneTop) 
                    neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y + 1));
            }

            if (currentNode.X + 1 < Region.Width)
            {
                // Right
                neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y));

                // Right Down
                if (currentNode.Y - 1 >= activeZoneBottom) 
                    neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y - 1));

                // Right Up
                if (currentNode.Y + 1 <= activeZoneTop) 
                    neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y + 1));
            }

            // Down
            if (currentNode.Y - 1 >= activeZoneBottom) 
                neighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));

            // Up
            if (currentNode.Y + 1 <= activeZoneTop) 
                neighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));

            return neighbourList;
        }

        public PathNode GetNode(int x, int y)
        {
            return Region.GetAreaAtXY(x, y).PathNode;
        }

        private float CalculateDistanceCost(PathNode a, PathNode b)
        {
            int xDist = Mathf.Abs(a.X - b.X);
            int yDist = Mathf.Abs(a.Y - b.Y);
            int remaining = Mathf.Abs(xDist - yDist);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDist, yDist) + MOVE_STRAIGHT_COST * remaining;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];

            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }

            return lowestFCostNode;
        }
    }
}
