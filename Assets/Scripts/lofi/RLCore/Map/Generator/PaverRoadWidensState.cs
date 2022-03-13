namespace lofi.RLCore
{
    public class PaverRoadWidensState : PaverState
    {
        private int maxRoadWidth;
        private float widenRatio;

        public PaverRoadWidensState(Paver paver, int minHeight, int maxHeight, float probablity) : base(paver, minHeight, maxHeight, probablity)
        {
            widenRatio = paver.levelGeneratorData.widenRatio;
            maxRoadWidth = paver.levelGeneratorData.maxRoadWidth;
        }

        public override void PlaceNextRow()
        {
            if (paver.currentRow % widenRatio == 0 && paver.width <= maxRoadWidth)
            {
                if (paver.offsetLeft > (paver.Level.Width / 4))
                {
                    paver.offsetLeft--;
                    paver.width++;
                }
                else if (paver.width < (paver.Level.Width / 2) || paver.offsetLeft < (paver.Level.Width / 10))
                {
                    paver.width++;
                }
                else
                {
                    paver.offsetLeft--;
                    paver.width++;
                }
            }

            paver.PaveRoad();
        }
    }
}