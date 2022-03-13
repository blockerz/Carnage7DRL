using UnityEngine;

namespace lofi.RLCore
{
    public class Constants
    {
        public static readonly int MAP_LAYER_ID = 6;

        public static readonly bool DEBUG_ENABLED = false;

        public static readonly int MAP_TILE_WIDTH = 22;
        public static readonly int ACTIVE_TILE_WIDTH = 22;
        public static readonly int ACTIVE_TILE_HEIGHT = 17*3;
        public static readonly Vector3 MAP_ORIGIN = new Vector3(100, 100, 0);
        public static readonly int MAP_AREA_X = 10;
        public static readonly int MAP_AREA_Y = 10;
        public static readonly int STAGE_COMPLETE_RANGE = 3;
        public static readonly int SPAWN_BEHIND_RANGE = 7;
        public static readonly int SPAWN_AHEAD_RANGE = 10;

        public static readonly float UNIT_Z_POS = -1f;
        public static readonly float KEYBOARD_INPUT_DELAY = 0.2f;

        public static readonly int SHOTGUN_START_AMOUNT = 10;
        public static readonly int SHOTGUN_RANGE = 2;
        public static readonly int SHOTGUN_DAMAGE = 10;
        public static readonly int LOOT_CHANCE = 20;

        public static readonly string HIGH_SCORE_KEY = "HighScore";

    }
}