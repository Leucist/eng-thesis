namespace Application.AI
{
    public class AIDistributionManager
    {
        // Game results being:
        // - Percent of enemies spawned CLOSE
        // - Percent of enemies spawned at MID
        // - Percent of enemies spawned FAR
        // - "Did AI win?" [0/1]
        private float[] _previousRound;

        // Decision being the flags describing how much is NN inclined towards each option:
        // - Reinforce CLOSE position
        // - Reinforce MID position
        // - Reinforce FAR position
        // - Change nothing
        private float[] _decision;

        private readonly float[] _startDistribution = [0.4f, 0.3f, 0.3f];    // todo: randomly generate

        public float[] Distribution => 

        public AIDistributionManager() {
            _previousRound = new float[4];
            _decision = new float[4];

            // Fill data about the previous round for the game start (copy default distribution)
            Array.Copy(_startDistribution, _previousRound, 3);
        }

        public void ConcludeRoundResults(int didAIWin) {
            _previousRound[4] = didAIWin;
        }

        public void RecordAIWin()       => ConcludeRoundResults(1);
        public void RecordPlayerWin()   => ConcludeRoundResults(0);
    }
}