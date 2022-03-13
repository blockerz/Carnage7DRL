namespace lofi.RLCore
{
    public class PaverRoadNarrowsState : PaverState
    {
        private int minRoadWidth;
        private float narrowRatio;

        public PaverRoadNarrowsState(Paver paver, int minHeight, int maxHeight, float probablity) : base(paver, minHeight, maxHeight, probablity)
        {
            narrowRatio = paver.levelGeneratorData.narrowRatio;
            minRoadWidth = paver.levelGeneratorData.minRoadWidth;
        }

        public override void PlaceNextRow()
        {
            if (paver.currentRow % narrowRatio == 0 && paver.width > minRoadWidth)
            {
                paver.offsetLeft++;
                paver.width-=2;
            }

            paver.PaveRoad();
        }
    }
}