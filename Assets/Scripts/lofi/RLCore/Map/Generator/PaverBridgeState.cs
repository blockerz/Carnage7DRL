namespace lofi.RLCore
{
    public class PaverBridgeState : PaverState
    {
        public PaverBridgeState(Paver paver, int minHeight, int maxHeight, float probablity) : base(paver, minHeight, maxHeight, probablity)
        {

        }

        public override void PlaceNextRow()
        {
            for (int b = 0; b < paver.Level.Width; b++)
            {
                paver.Level.SetAreaDataAtXY(b, paver.currentRow, paver.areaData[AreaID.WATER]);
            }

            paver.PaveRoad(paver.levelGeneratorData.primaryRoad, AreaID.GUARDRAIL);

        }
    }
}