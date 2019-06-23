using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.IO;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public class Network
    {
        public delegate void DelNetworkForwardPropagated(NeuronLayer output, double networkCost);
        public event DelNetworkForwardPropagated NetworkForwardPropagated;

        //TODO: reset to 0 any time the network setup is modified
        private double _totalIterationsTrained = 0;
        private double _currentCost;
        private double _costDelta;

        /// <summary>
        /// The method to use to calculate the cost of an output neuron.
        /// </summary>
        public CostFunction Cost { get; set; } = new CF_DiffSquared();
        /// <summary>
        /// The ground truth output layer used to compare the network generated outputs against.
        /// The number of neurons in this layer must match the count in the last layer in the network.
        /// </summary>
        public NeuronLayer ExpectedOutput { get; set; }
        /// <summary>
        /// This is the first layer in the network, used solely for input.
        /// All layers after this one are either hidden layers or the output layer.
        /// Layers attached before this one are not used.
        /// </summary>
        public NeuronLayer First { get; set; }
        /// <summary>
        /// Recursively counts the attached layers.
        /// </summary>
        public int GetLayerCount => First?.GetLayerCount() ?? 0;

        /// <summary>
        /// Propagates inputs through the network and returns the corresponding output values.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Execute(params double[] input)
        {
            if (First == null)
                throw new InvalidOperationException($"{nameof(First)} cannot be null.");
            if (First.NeuronOutValues == null)
                throw new InvalidOperationException($"{nameof(First)}.{nameof(NeuronLayer.NeuronOutValues)} cannot be null.");
            if (input == null)
                throw new InvalidOperationException($"{nameof(input)} cannot be null.");
            if (First.NeuronOutValues.Length != input.Length)
                throw new InvalidOperationException($"{nameof(input)}.{nameof(NeuronLayer.NeuronOutValues)} count does not match {nameof(First)} count.");

            First.NeuronOutValues = input;

            NeuronLayer output = First.ForwardPropagate();

            return output.NeuronOutValues;
        }
        public void Train(int iterations, double learningRate)
        {
            if (ExpectedOutput == null)
                throw new InvalidOperationException($"Network needs {nameof(ExpectedOutput)} to be set to train against.");
            if (First == null)
                throw new InvalidOperationException($"Network needs {nameof(First)} to be set to forward propagate through layers.");
            if (First.Next == null)
                throw new InvalidOperationException($"Network needs at least two layers (set {nameof(First)}.{nameof(NeuronLayer.Next)}).");
            if (learningRate <= 0.0)
                throw new InvalidOperationException($"{nameof(learningRate)} must be greater than zero.");

            NeuronLayer last = First.Initialize(new Random());
            if (last.NeuronOutValues.Length != ExpectedOutput.NeuronOutValues.Length)
                throw new InvalidOperationException($"{nameof(ExpectedOutput)} does not have the same number of neurons as the last layer in the network.");
            
            int ouputNeuronCount = last.NeuronOutValues.Length;
            double totalCost, targetValue, actualValue, neuronCost;
            for (int i = 0; i < iterations; ++i, ++_totalIterationsTrained)
            {
                last = First.ForwardPropagate();

                totalCost = 0.0;
                for (int outputNeuronIndex = 0; outputNeuronIndex < ouputNeuronCount; ++outputNeuronIndex)
                {
                    targetValue = ExpectedOutput.NeuronOutValues[outputNeuronIndex];
                    actualValue = last.NeuronOutValues[outputNeuronIndex];
                    neuronCost = Cost.Evaluate(targetValue, actualValue);
                    last.SetNeuronCost(outputNeuronIndex, neuronCost);
                    totalCost += neuronCost;
                }

                //Update output display here
                NetworkForwardPropagated?.Invoke(last, totalCost);

                _costDelta = totalCost - _currentCost;
                _currentCost = totalCost;

                last.BackwardPropagate(learningRate);
            }
        }

        public void ExportCSV(string path)
        {
            using (CsvWriter writer = new CsvWriter(new StreamWriter(path, false)))
            {
                writer.WriteField(First.NeuronOutValues.Length);
                writer.WriteField(_currentCost);
                writer.NextRecord();

                NeuronLayer next = First;
                while (next != null)
                {
                    writer.WriteField(next.Weights.Length);
                    writer.WriteField(next.Biases.Length);
                    for (int i = 0; i < next.Weights.Length; ++i)
                        writer.WriteField(next.Weights[i]);
                    for (int i = 0; i < next.Biases.Length; ++i)
                        writer.WriteField(next.Biases[i]);
                    writer.NextRecord();
                }
            }
        }
        public static Network FromCSV(string path)
        {
            Network net = new Network();
            Configuration config = new Configuration();
            using (CsvReader reader = new CsvReader(new StreamReader(path), config))
            {
                int inputCount = reader.GetField<int>(0);
                NeuronLayer first = new NeuronLayer(null, inputCount);
                NeuronLayer prev = first;
                double cost = reader.GetField<double>(1);

                net.First = first;
                net._currentCost = cost;

                while (reader.Read())
                {
                    int weightCount = reader.GetField<int>(0);
                    int biasCount = reader.GetField<int>(1);
                    NeuronLayer layer = new NeuronLayer(null, biasCount);
                    for (int i = 0; i < weightCount; ++i)
                        layer.Weights[i] = reader.GetField<double>(i + 2);
                    for (int i = 0; i < biasCount; ++i)
                        layer.Biases[i] = reader.GetField<double>(i + 2 + weightCount);
                    prev.Next = layer;
                    prev = layer;
                }
            }
            return net;
        }
    }
}
