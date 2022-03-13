using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class Area
    {
        public long ID { get; set; }
        public PathNode PathNode { get; set; }
        public Unit UnitPresent { get; set; }
        public Item ItemPresent { get; set; }
        public AreaData Data { get; set; }

        public Area(long id)
        {
            ID = id;
        }
    }
}