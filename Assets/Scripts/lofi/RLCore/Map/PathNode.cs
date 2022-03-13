using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class PathNode
    {
        //private Region region;

        public int X { get; set; }
        public int Y { get; set; }
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost { get; set; }
        public float MovementCost { get; set; }
        public bool IsPassable { get; set; }
        public PathNode ParentNode { get; set; }

        public PathNode (int x, int y, int moveCost, bool passable = true)
        {
            this.X = x;
            this.Y = y;
            //this.region = region;
            this.MovementCost = moveCost;
            this.IsPassable = passable;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }

        internal void CalculateFCost()
        {
            FCost = GCost + HCost;
        }
    }
}
