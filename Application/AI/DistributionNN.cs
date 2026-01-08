namespace Application.AI
{
    public struct TrainingData
    {
        public float[] Input;  // [closePercent, midPercent, farPercent, aiWon]
        public float[] Target; // [reinforceClose, reinforceMid, reinforceFar, noChange]
    }

    public class DistributionNN
    {
        private float[,] weightsInputHidden;
        private float[] biasHidden;
        private float[,] weightsHiddenOutput;
        private float[] biasOutput;

        private int inputSize = 4;   // [closePercent, midPercent, farPercent, aiWon]
        private int hiddenSize = 8;
        private int outputSize = 4;  // [reinforceClose, reinforceMid, reinforceFar, noChange]

        private Random random = new Random();

        public DistributionNN()
        {
            InitializeWeights();
        }

        private void InitializeWeights()
        {
            weightsInputHidden = new float[inputSize, hiddenSize];
            biasHidden = new float[hiddenSize];
            weightsHiddenOutput = new float[hiddenSize, outputSize];
            biasOutput = new float[outputSize];

            // Xavier initialization
            float limitIH = MathF.Sqrt(6f / (inputSize + hiddenSize));
            float limitHO = MathF.Sqrt(6f / (hiddenSize + outputSize));

            for (int i = 0; i < inputSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    weightsInputHidden[i, j] = RandomRange(-limitIH, limitIH);

            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < outputSize; j++)
                    weightsHiddenOutput[i, j] = RandomRange(-limitHO, limitHO);

            for (int i = 0; i < hiddenSize; i++)
                biasHidden[i] = 0;

            for (int i = 0; i < outputSize; i++)
                biasOutput[i] = 0;
        }

        public float[] Forward(float[] input)
        {
            // Input -> Hidden
            float[] hidden = new float[hiddenSize];
            for (int j = 0; j < hiddenSize; j++)
            {
                float sum = biasHidden[j];
                for (int i = 0; i < inputSize; i++)
                    sum += input[i] * weightsInputHidden[i, j];
                hidden[j] = ReLU(sum);
            }

            // Hidden -> Output
            float[] output = new float[outputSize];
            for (int j = 0; j < outputSize; j++)
            {
                float sum = biasOutput[j];
                for (int i = 0; i < hiddenSize; i++)
                    sum += hidden[i] * weightsHiddenOutput[i, j];
                output[j] = sum;
            }

            // Softmax for output (gives probabilities that sum to 1)
            return Softmax(output);
        }

        public void Train(TrainingData[] dataset, int epochs = 1000, float learningRate = 0.01f)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                float totalLoss = 0;

                foreach (var data in dataset)
                {
                    // Forward pass
                    float[] hidden = new float[hiddenSize];
                    for (int j = 0; j < hiddenSize; j++)
                    {
                        float sum = biasHidden[j];
                        for (int i = 0; i < inputSize; i++)
                            sum += data.Input[i] * weightsInputHidden[i, j];
                        hidden[j] = ReLU(sum);
                    }

                    float[] output = new float[outputSize];
                    for (int j = 0; j < outputSize; j++)
                    {
                        float sum = biasOutput[j];
                        for (int i = 0; i < hiddenSize; i++)
                            sum += hidden[i] * weightsHiddenOutput[i, j];
                        output[j] = sum;
                    }

                    float[] predictions = Softmax(output);

                    // Calculate loss (cross-entropy)
                    for (int i = 0; i < outputSize; i++)
                        totalLoss -= data.Target[i] * MathF.Log(predictions[i] + 1e-7f);

                    // Backpropagation
                    float[] outputGrad = new float[outputSize];
                    for (int i = 0; i < outputSize; i++)
                        outputGrad[i] = predictions[i] - data.Target[i];

                    // Update hidden->output weights
                    for (int i = 0; i < hiddenSize; i++)
                    {
                        for (int j = 0; j < outputSize; j++)
                        {
                            weightsHiddenOutput[i, j] -= learningRate * outputGrad[j] * hidden[i];
                        }
                    }

                    for (int j = 0; j < outputSize; j++)
                        biasOutput[j] -= learningRate * outputGrad[j];

                    // Backprop to hidden layer
                    float[] hiddenGrad = new float[hiddenSize];
                    for (int i = 0; i < hiddenSize; i++)
                    {
                        float sum = 0;
                        for (int j = 0; j < outputSize; j++)
                            sum += outputGrad[j] * weightsHiddenOutput[i, j];
                        hiddenGrad[i] = sum * ReLUDerivative(hidden[i]);
                    }

                    // Update input->hidden weights
                    for (int i = 0; i < inputSize; i++)
                    {
                        for (int j = 0; j < hiddenSize; j++)
                        {
                            weightsInputHidden[i, j] -= learningRate * hiddenGrad[j] * data.Input[i];
                        }
                    }

                    for (int j = 0; j < hiddenSize; j++)
                        biasHidden[j] -= learningRate * hiddenGrad[j];
                }

                if (epoch % 100 == 0)
                    Console.WriteLine($"Epoch {epoch}, Loss: {totalLoss / dataset.Length}");
            }
        }

        private float ReLU(float x) => MathF.Max(0, x);
        private float ReLUDerivative(float x) => x > 0 ? 1 : 0;

        private float[] Softmax(float[] values)
        {
            float max = values.Max();
            float[] exp = values.Select(v => MathF.Exp(v - max)).ToArray();
            float sum = exp.Sum();
            return exp.Select(e => e / sum).ToArray();
        }

        private float RandomRange(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        // Serialization for saving/loading
        public void SaveWeights(string filename)
        {
            using var writer = new System.IO.StreamWriter(filename);

            // Save dimensions
            writer.WriteLine($"{inputSize},{hiddenSize},{outputSize}");

            // Save input->hidden weights
            for (int i = 0; i < inputSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    writer.WriteLine(weightsInputHidden[i, j]);

            // Save hidden biases
            for (int i = 0; i < hiddenSize; i++)
                writer.WriteLine(biasHidden[i]);

            // Save hidden->output weights
            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < outputSize; j++)
                    writer.WriteLine(weightsHiddenOutput[i, j]);

            // Save output biases
            for (int i = 0; i < outputSize; i++)
                writer.WriteLine(biasOutput[i]);
        }

        public void LoadWeights(string filename)
        {
            using var reader = new System.IO.StreamReader(filename);

            // Read dimensions
            var dims = reader.ReadLine().Split(',');
            inputSize = int.Parse(dims[0]);
            hiddenSize = int.Parse(dims[1]);
            outputSize = int.Parse(dims[2]);

            // Initialize arrays
            weightsInputHidden = new float[inputSize, hiddenSize];
            biasHidden = new float[hiddenSize];
            weightsHiddenOutput = new float[hiddenSize, outputSize];
            biasOutput = new float[outputSize];

            // Read input->hidden weights
            for (int i = 0; i < inputSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    weightsInputHidden[i, j] = float.Parse(reader.ReadLine());

            // Read hidden biases
            for (int i = 0; i < hiddenSize; i++)
                biasHidden[i] = float.Parse(reader.ReadLine());

            // Read hidden->output weights
            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < outputSize; j++)
                    weightsHiddenOutput[i, j] = float.Parse(reader.ReadLine());

            // Read output biases
            for (int i = 0; i < outputSize; i++)
                biasOutput[i] = float.Parse(reader.ReadLine());
        }
    }
}