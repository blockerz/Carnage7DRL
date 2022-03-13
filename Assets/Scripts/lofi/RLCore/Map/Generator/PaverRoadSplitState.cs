using System;

namespace lofi.RLCore
{
    public class PaverRoadSplitState : PaverState
    {
        // length the road is separated
        public int splitDistance { get; set; }

        // distance between two roads
        public int splitSteps { get; set; }

        // normal road prior to split
        public int splitStart { get; set; }

        // normal road after split
        public int splitEnd { get; set; }

        // proportion between left and right road (0.5 is equally split)
        public float splitPercent { get; set; }


        private int splitWidthLeft;
        private int splitWidth;
        private int splitRatio;
        private int mergeSteps;


        public PaverRoadSplitState(Paver paver, int minHeight, int maxHeight, float probablity) : base(paver, minHeight, maxHeight, probablity)
        {
            Reset();
        }


        // Rest on Exit so they can be manually set when making new 
        public override void Exit()
        {
            base.Exit();

            Reset();
        }

        private void Reset()
        {
            splitDistance = paver.levelGeneratorData.splitDistance;
            splitStart = paver.levelGeneratorData.splitStart;
            splitEnd = paver.levelGeneratorData.splitEnd;
            splitSteps = paver.levelGeneratorData.splitSteps;
            mergeSteps = splitSteps-1;
            splitPercent = paver.levelGeneratorData.splitPercent;

            splitWidth = 0;
            splitRatio = 2;
            splitWidthLeft = (int)(splitPercent * (paver.width + splitRatio));
            //splitWidthLeft = Math.Min((int)splitPercent * paver.width,paver.levelGeneratorData.minRoadWidth);
        }

        public override void PlaceNextRow()
        {            
            if (splitStart > 0)
            {
                paver.PaveRoad();
                splitStart--;

                if (splitStart == 0)
                {
                    splitWidthLeft = (int)(splitPercent * (paver.width + splitRatio));

                    if (splitWidthLeft <= 4)
                        splitWidthLeft++;
                }
            }
            else if (splitSteps > 0)
            {
                paver.width += splitRatio;
                paver.offsetLeft -= (int)(splitRatio * splitPercent);

                //if (splitWidth == 0 && paver.width % 2 == 1)
                //    splitWidth = 1;

                paver.PaveRoadSplit(splitWidthLeft,splitWidth);

                splitSteps--;
                if (splitSteps > 0)
                    splitWidth += splitRatio;
            }            
            else if (splitDistance > 0)
            {
                paver.PaveRoadSplit(splitWidthLeft, splitWidth);
                splitDistance--;
            }
            else if (mergeSteps > 0)
            {
                paver.width -= splitRatio;
                paver.offsetLeft += (int)(splitRatio * splitPercent);
                splitWidth -= splitRatio;

                paver.PaveRoadSplit(splitWidthLeft, splitWidth);

                mergeSteps--;
            }
            else 
            {
                paver.PaveRoad();
                splitEnd--;
            }
        }
    }
}