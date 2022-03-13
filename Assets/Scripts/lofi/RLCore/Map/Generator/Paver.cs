using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class Paver
    {
        public int currentRow;
        public int width;
        public int offsetLeft;

        public Dictionary<AreaID, AreaData> areaData;
        public Region Level;        

        public LevelGeneratorData levelGeneratorData;

        public Paver(Dictionary<AreaID, AreaData> areaData, Region level, LevelGeneratorData levelGeneratorData)
        {
            Level = level;
            this.areaData = areaData;
            this.levelGeneratorData = levelGeneratorData;
            currentRow = 0;
            width = levelGeneratorData.startRoadWidth;
            offsetLeft = levelGeneratorData.startRoadLeftOffset;
                        
        }


        public void SetCurrentRow(int y)
        {
            //if (distance != y)
            //    Debug.Log("distance out of sync.");
            
            currentRow = y;
        }

        public void PaveRoad()
        {
            PaveRoad(levelGeneratorData.primaryRoad, levelGeneratorData.primaryRoadEdge);           
        }
        
        public void PaveRoad(AreaID road, AreaID roadEdge)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || x == width-1)
                    Level.SetAreaDataAtXY(x + offsetLeft, currentRow, areaData[roadEdge]);
                else
                    Level.SetAreaDataAtXY(x + offsetLeft, currentRow, areaData[road]);
            }
        }

        public void PaveRoadSplit(int splitStart, int splitLength)
        {
            PaveRoadSplit(levelGeneratorData.primaryRoad, levelGeneratorData.primaryRoadEdge, splitStart, splitLength);
        }

        public void PaveRoadSplit(AreaID road, AreaID roadEdge, int splitStart, int splitLength)
        {
            for (int x = offsetLeft; x < width + offsetLeft; x++)
            {
                if (x >= splitStart + offsetLeft && x < splitStart + offsetLeft + splitLength)
                    continue;

                if (x == offsetLeft || x == offsetLeft+splitStart - 1 || x == offsetLeft + splitStart + splitLength || x == offsetLeft+width - 1)
                    Level.SetAreaDataAtXY(x, currentRow, areaData[roadEdge]);
                else
                    Level.SetAreaDataAtXY(x, currentRow, areaData[road]);
            }
        }
    }
}