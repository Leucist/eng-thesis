namespace Application.AI
{
    public class AIDistributionManager
    {
        private DistributionNN _neuralNetwork;
        private string _weightsFile = Path.Combine(Pathfinder.GetSolutionDirectory(), "Application", "AI", "weights.json");
        // Game results being:
        // - Percent of enemies spawned CLOSE
        // - Percent of enemies spawned at MID
        // - Percent of enemies spawned FAR
        // - "Did AI win?" [0/1]
        private float[] _previousRound;

        private readonly float[] _startDistribution = [0.33f, 0.33f, 0.34f];    // todo: randomly generate

        private float[] _distribution;
        public float[] Distribution => _distribution;

        private int _logRoundCounter = 0;

        public AIDistributionManager() {
            _previousRound  = new float[4];
            _distribution   = new float[3];

            // Fill data about the previous round for the game start (copy default distribution)
            Array.Copy(_startDistribution, _previousRound, 3);
            Array.Copy(_startDistribution, _distribution, 3);

            // Init neural network
            _neuralNetwork = new();
            if (File.Exists(_weightsFile))
            {
                _neuralNetwork.LoadWeights(_weightsFile);
                Console.WriteLine("Loaded trained neural network.");
            }
            else
            {
                Console.WriteLine("Started training new neural network.");
                TrainNetwork(_weightsFile);
            }
        }

        private void TrainNetwork(string weightsFile)
        {
            var trainingData = TrainingDataGenerator.GenerateTrainingData();
            Console.WriteLine($"Generated {trainingData.Length} training samples");

            _neuralNetwork.Train(trainingData, epochs: 2000, learningRate: 0.1f);
            _neuralNetwork.SaveWeights(weightsFile);

            Console.WriteLine($"Training complete! Saved to {weightsFile}");
        }

        private void HandleDecision(int decisionNum) {
            // Decision being the flags describing how much is NN inclined towards each option:
            // - Reinforce CLOSE position
            // - Reinforce MID position
            // - Reinforce FAR position
            // - Change nothing

            // Copy distribution from the previous round
            Array.Copy(_previousRound, _distribution, _distribution.Length);

            // Quit method if the decision is to change nothing 
            // [when desicion number is higher than distribution length (basically amount of the spawn points) minus one (index)]
            if (decisionNum > _distribution.Length - 1) return;

            // Gather data: which spawnpoints aren't empty yet and find the minimal percent each of them can spare 
            var optionsLeftNotEmpty = new List<int>(_distribution.Length - 1);
            float minGiveaway = 1;
            for (int i = 0; i < _distribution.Length; i++) {
                if (i == decisionNum) continue; // skip the chosen spawnpoint
                var enemiesOnPoint = _distribution[i];
                if (enemiesOnPoint > 0) optionsLeftNotEmpty.Add(i);                     // store id if point not empty
                else if (minGiveaway > enemiesOnPoint) minGiveaway = enemiesOnPoint;    // establish min not zero
            }
            
            // if there's any percentage left to transfer among spawnpoints
            if (minGiveaway > 0) {
                float decrease = Math.Min(0.1f / optionsLeftNotEmpty.Count, minGiveaway); // either 1 or less
                float increase = decrease * optionsLeftNotEmpty.Count;  // to take exactly as much as we add
                _distribution[decisionNum] += increase;
                foreach (var point in optionsLeftNotEmpty) {
                    _distribution[point] -= decrease;
                }
            }
        }

        private void ConcludeRoundResults(int didAIWin) {
            _previousRound[3] = didAIWin;

            // * Generate result *;
            float[] decision = _neuralNetwork.Forward(_previousRound);

            // Find the final decision (max inclined to)
            int maxIndex = 0;
            for (int i = 1; i < decision.Length; i++) {
                maxIndex = decision[i] > decision[maxIndex] ? i : maxIndex;
            }

            // todo: - NN Logs -
            _logRoundCounter++;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n\tROUND #{_logRoundCounter}");
            Console.ResetColor();
            Console.WriteLine("Data from the previous round >");
            Console.WriteLine($"Close percent:\t{_previousRound[0]}");
            Console.WriteLine($"Mid percent:\t{_previousRound[1]}");
            Console.WriteLine($"Far percent:\t{_previousRound[2]}");
            Console.WriteLine($"Did AI win?:\t{_previousRound[3]}");
            Console.WriteLine($"\nNN chose option number: {maxIndex}");
            // todo: - end logs -

            HandleDecision(maxIndex);
            // Store the distribution
            Array.Copy(_distribution, _previousRound, _distribution.Length);

            // todo: - NN Logs -
            Console.WriteLine($"NN proposes the following distribution >");
            Console.WriteLine($"Close percent:\t{_distribution[0]}");
            Console.WriteLine($"Mid percent:\t{_distribution[1]}");
            Console.WriteLine($"Far percent:\t{_distribution[2]}\n");
            // todo: - end logs -
        }

        public void RecordAIWin()       => ConcludeRoundResults(1);
        public void RecordPlayerWin()   => ConcludeRoundResults(0);
    }
}