
using UnityEngine;

namespace lofi.RLCore
{
    public class Region
    {
        public int Width { get; }
        public int Height { get; }
        public float AreaWorldSizeX { get; }
        public float AreaWorldSizeY { get; }
        public Vector3 Origin { get; }

        private Area[,] areas;

        public LevelGeneratorData generatorData;

        // A rectangular region of cells/areas 
        // A.K.A. Grid, Map
        public Region(int width, int height, Vector3 origin, float areaWorldSizeX = 10.0f, float areaWorldSizeY = 10.0f)
        {
            this.Width = width;
            this.Height = height;
            this.AreaWorldSizeX = areaWorldSizeX;
            this.AreaWorldSizeY = areaWorldSizeY;

            if (origin == null)
                this.Origin = Vector3.zero;
            else
                this.Origin = new Vector3(origin.x, origin.y, origin.z);

            areas = new Area[width, height];

            //for (int x = 0; x < areas.GetLength(0); x++)
            //{
            //    for (int y = 0; y < areas.GetLength(1); y++)
            //    {
            //        areas[x, y] = new Area(GetIDFromXY(x, y));
            //        AreaData data;
            //        if (areaData.TryGetValue(AreaID.ROAD, out data))
            //        {
            //            areas[x, y].Data = data;
            //        }
            //    }
            //}

            //DrawDebugView(width, height, areaWorldSizeX, areaWorldSizeY);

            //OnRegionChanged += (object sender, OnRegionChangedEventArgs eventArgs) => {
            //    debugText[eventArgs.x, eventArgs.y].text = areas[eventArgs.x, eventArgs.y]?.ID.ToString();
            //};

        }

        //public long ConvertXYToID(int x, int y)
        //{
        //    return (Mathf.FloorToInt(Origin.x / AreaWorldSizeX) + x + Mathf.FloorToInt(Origin.x)) 
        //        + (y * (Mathf.FloorToInt(Origin.x / AreaWorldSizeX) + Width) + Mathf.FloorToInt(Origin.y));
        //}

        //public Vector2Int ConvertIDToXY(long id)
        //{
        //    Vector2Int ret = Vector2Int.zero;

        //    ret.x = (int)(id % (Mathf.FloorToInt(Origin.x / AreaWorldSizeX) + Width));
        //    ret.y = (int)(id / (Mathf.FloorToInt(Origin.x / AreaWorldSizeX) + Width));

        //    return ret;

        //}

        //public long ConvertXYToID(int x, int y)
        //{
        //    int worldX = Mathf.FloorToInt(x + Origin.x);
        //    int worldY = Mathf.FloorToInt(y + Origin.y);

        //    return (worldX)
        //        + ((worldY * Width) + Mathf.FloorToInt(Origin.x));
        //}

        //public Vector2Int ConvertIDToXY(long id)
        //{
        //    Vector2Int ret = Vector2Int.zero;

        //    ret.x = (int)(id % (Mathf.FloorToInt(Origin.x / AreaWorldSizeX) + Width));
        //    ret.y = (int)(id / (Mathf.FloorToInt(Origin.x / AreaWorldSizeX) + Width));

        //    return ret;

        //}

        public long GetIDFromXY(int x, int y)
        {

            return x + (y * Width);
        }

        public Vector2Int GetXYFromID(long id)
        {
            Vector2Int ret = Vector2Int.zero;

            ret.x = (int)(id % Width);
            ret.y = (int)(id / Width);

            return ret;

        }

        public Vector3 GetAreaWorldPosition(int x, int y)
        {
            return new Vector3(x * AreaWorldSizeX, y * AreaWorldSizeY) + Origin;
        }

        public Vector2Int GetXYFromWorldPosition(Vector3 position)
        {
            Vector2Int ret = Vector2Int.zero;
            ret.x = Mathf.FloorToInt((position - Origin).x / AreaWorldSizeX);
            ret.y = Mathf.FloorToInt((position - Origin).y / AreaWorldSizeY);
            return ret;
        }

        public bool SetIDForArea(int x, int y, long id)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                areas[x, y].ID = id;
                //TriggerRegionChanged(x, y);
                //DrawDebugView(width, height, areaWorldSizeX, areaWorldSizeY);
                return true;
            }

            return false;
        }

        public bool SetIDAtWorldPosition(Vector3 position, long id)
        {
            Vector2Int pos = GetXYFromWorldPosition(position);
            return SetIDForArea(pos.x, pos.y, id);
        }

        public long GetIDForArea(int x, int y)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                return areas[x, y].ID;
            }

            return -1;
        }
        
        public Area GetAreaAtXY(int x, int y)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                return areas[x, y];
            }

            return null;
        }
        
        public Item GetItemAtXY(int x, int y)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                return areas[x, y].ItemPresent;
            }

            return null;
        }
        
        public bool AddItemAtArea(Item item, int x, int y)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                areas[x, y].ItemPresent = item;
                return true;
            }

            return false;
        }

        public Vector2Int GetOpenAreaAtY(int y)
        {
            int FirstRoad = -1; 
            int LastRoad = -1; 

            if (y >= 0 && y < areas.GetLength(1))
            {
                for (int x = 0; x < areas.GetLength(0); x++)
                {
                    if (FirstRoad < 0 && areas[x, y].Data.isPassable && areas[x, y].Data.isHazard == false)
                    {
                        FirstRoad = x;
                    }
                    else if (FirstRoad >= 0 && LastRoad < 0 && areas[x, y].Data.isHazard == false)
                    {
                        LastRoad = x - 1;
                        break;
                    }
                }                 
            }

            int selectedRoadXPosition = Random.Range(FirstRoad, LastRoad + 1);

            return new Vector2Int(selectedRoadXPosition, y);
        }

        public Unit GetUnitAtXY(int x, int y)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                return areas[x, y].UnitPresent;
            }

            return null;
        }

        public bool SetAreaAtXY(int x, int y, Area area)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                areas[x, y] = area;
                return true;
            }

            return false;
        }

        public bool SetAreaDataAtXY(int x, int y, AreaData areaData)
        {
            if (x >= 0 && x < areas.GetLength(0) && y >= 0 && y < areas.GetLength(1))
            {
                areas[x, y].Data = areaData;
                return true;
            }

            return false;
        }

        public long GetIDAtWorldPosition(Vector3 position)
        {
            Vector2Int pos = GetXYFromWorldPosition(position);
            return GetIDForArea(pos.x, pos.y);
        }


    }
}