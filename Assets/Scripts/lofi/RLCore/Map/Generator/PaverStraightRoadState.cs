namespace lofi.RLCore
{
    public class PaverStraightRoadState : PaverState
    {
        public PaverStraightRoadState(Paver paver, int minHeight, int maxHeight, float probablity) : base(paver, minHeight, maxHeight, probablity)
        {

        }

        public override void PlaceNextRow()
        {
            paver.PaveRoad();
        }
    }
}