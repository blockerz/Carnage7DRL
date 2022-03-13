namespace lofi.RLCore
{
    public class PaverCurvyRoadState : PaverState
    {
        private bool curvingLeft;
        private float curveRatio;
        private int startCurve;

        public PaverCurvyRoadState(Paver paver, int minHeight, int maxHeight, float probablity) : base(paver, minHeight, maxHeight, probablity)
        {
            Reset();
            curveRatio = paver.levelGeneratorData.curveRatio;
        }

        public override void Exit()
        {
            base.Exit();

            Reset();
        }

        private void Reset()
        {
            curvingLeft = true;
            startCurve = 1;
        }

        public override void PlaceNextRow()
        {
            if (startCurve > 0)
            {
                paver.PaveRoad();
                startCurve--;
            }
            else if (paver.currentRow % curveRatio == 0)
            {
                if (curvingLeft && paver.offsetLeft > 1)
                {
                    paver.offsetLeft--;
                }
                else
                {
                    curvingLeft = false;
                }

                if (!curvingLeft && paver.offsetLeft < paver.Level.Width - paver.width - 1)
                {
                    paver.offsetLeft++;
                }
                else
                {
                    curvingLeft = true;
                }
            }

            paver.PaveRoad();
        }
    }
}