using System.Text.Json;

namespace Application.AI
{
    public struct TrainingData
    {
        public float[] Input;  // [closePercent, midPercent, farPercent, aiWon]
        public float[] Target; // [reinforceClose, reinforceMid, reinforceFar, noChange]
    }

    public class DistributionNN
    {
        private int _inputSize = 4;
        private int _hiddenSize = 8;
        private int _outputSize = 4;

        private float[][] _weightsInputHidden;
        private float[][] _weightsHiddenOutput;
        private float[] _biasHidden;
        private float[] _biasOutput;

        private Random _rand = new Random();

        public DistributionNN()
        {
            _weightsInputHidden = InitMatrix(_inputSize, _hiddenSize);
            _weightsHiddenOutput = InitMatrix(_hiddenSize, _outputSize);
            _biasHidden = InitArray(_hiddenSize);
            _biasOutput = InitArray(_outputSize);
        }

        public void Train(TrainingData[] data, int epochs = 1000, float learningRate = 0.1f)
        {
            string logPath = "training_log.csv";
            using (StreamWriter sw = new StreamWriter(logPath))
            {
                sw.WriteLine("Epoch,Loss"); // CSV Header for Python visualization

                for (int e = 0; e < epochs; e++)
                {
                    float totalLoss = 0;
                    foreach (var sample in data)
                    {
                        totalLoss += Backpropagate(sample.Input, sample.Target, learningRate);
                    }

                    float avgLoss = totalLoss / data.Length;
                    sw.WriteLine($"{e},{avgLoss.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

                    if (e % 100 == 0)
                        Console.WriteLine($"Epoch {e}: Loss = {avgLoss:F6}");
                }
            }
            Console.WriteLine($"Training finished. Log saved to {logPath}");
        }

        public float[] Forward(float[] input)
        {
            // Hidden layer activations
            float[] hidden = new float[_hiddenSize];
            for (int j = 0; j < _hiddenSize; j++)
            {
                float sum = _biasHidden[j];
                for (int i = 0; i < _inputSize; i++) sum += input[i] * _weightsInputHidden[i][j];
                hidden[j] = Sigmoid(sum);
            }

            // Output layer activations
            float[] output = new float[_outputSize];
            for (int j = 0; j < _outputSize; j++)
            {
                float sum = _biasOutput[j];
                for (int i = 0; i < _hiddenSize; i++) sum += hidden[i] * _weightsHiddenOutput[i][j];
                output[j] = Sigmoid(sum);
            }
            return output;
        }

        public void SaveWeights(string path)
        {
            var store = new WeightStore
            {
                WIH = _weightsInputHidden,
                WHO = _weightsHiddenOutput,
                BH = _biasHidden,
                BO = _biasOutput
            };
            File.WriteAllText(path, JsonSerializer.Serialize(store));
            Console.WriteLine($"Weights saved to {path}");
        }

        public void LoadWeights(string path)
        {
            if (!File.Exists(path)) return;
            var store = JsonSerializer.Deserialize<WeightStore>(File.ReadAllText(path));
            _weightsInputHidden = store.WIH;
            _weightsHiddenOutput = store.WHO;
            _biasHidden = store.BH;
            _biasOutput = store.BO;
            Console.WriteLine("Weights loaded successfully.");
        }

        // --- Private Math & Helpers ---

        private float Backpropagate(float[] input, float[] target, float lr)
        {
            // 1. Forward Pass (storing intermediate values)
            float[] hidden = new float[_hiddenSize];
            for (int j = 0; j < _hiddenSize; j++)
            {
                float sum = _biasHidden[j];
                for (int i = 0; i < _inputSize; i++) sum += input[i] * _weightsInputHidden[i][j];
                hidden[j] = Sigmoid(sum);
            }

            float[] output = Forward(input); // Simplified for logic, in real prod reuse 'hidden'

            // 2. Output Error & Gradients
            float[] outputDeltas = new float[_outputSize];
            float mse = 0;
            for (int i = 0; i < _outputSize; i++)
            {
                float error = target[i] - output[i];
                outputDeltas[i] = error * SigmoidDerivative(output[i]);
                mse += error * error;
            }

            // 3. Hidden Layer Error & Gradients
            float[] hiddenDeltas = new float[_hiddenSize];
            for (int i = 0; i < _hiddenSize; i++)
            {
                float error = 0;
                for (int j = 0; j < _outputSize; j++) error += outputDeltas[j] * _weightsHiddenOutput[i][j];
                hiddenDeltas[i] = error * SigmoidDerivative(hidden[i]);
            }

            // 4. Weights Update
            for (int i = 0; i < _hiddenSize; i++)
            {
                for (int j = 0; j < _outputSize; j++)
                    _weightsHiddenOutput[i][j] += lr * outputDeltas[j] * hidden[i];
            }

            for (int i = 0; i < _inputSize; i++)
            {
                for (int j = 0; j < _hiddenSize; j++)
                    _weightsInputHidden[i][j] += lr * hiddenDeltas[j] * input[i];
            }

            return mse / _outputSize;
        }

        private float Sigmoid(float x) => 1f / (1f + (float)Math.Exp(-x));
        private float SigmoidDerivative(float x) => x * (1f - x);

        private float[][] InitMatrix(int r, int c) => 
            Enumerable.Range(0, r).Select(_ => Enumerable.Range(0, c).Select(__ => (float)_rand.NextDouble() * 2 - 1).ToArray()).ToArray();

        private float[] InitArray(int size) => 
            Enumerable.Range(0, size).Select(_ => (float)_rand.NextDouble() * 2 - 1).ToArray();

        private class WeightStore
        {
            public float[][] WIH { get; set; }
            public float[][] WHO { get; set; }
            public float[] BH { get; set; }
            public float[] BO { get; set; }
        }
    }
}