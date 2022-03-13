namespace lofi.RLCore
{
    public abstract class PaverState : IState
    {
        protected Paver paver;

        public float Probability { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int CurrentHeight { get; set; }

        public PaverState(Paver paver, int minHeight, int maxHeight, float probability)
        {
            this.paver = paver;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            Probability = probability;
        }

        public virtual void Enter()
        {
            CurrentHeight = 0;
        }

        public virtual void Execute()
        {
            CurrentHeight++;
            PlaceNextRow();
        }

        public virtual void Exit()
        {
            
        }

        public abstract void PlaceNextRow();
    }
}