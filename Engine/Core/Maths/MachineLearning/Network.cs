using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public class Network : IEnumerable<NeuronLayer>
    {
        public delegate void DelNetworkForwardPropagated(NeuronLayer output, double networkCost);
        public event DelNetworkForwardPropagated NetworkForwardPropagated;

        //TODO: reset to 0 any time the network setup is modified
        private double _totalIterationsTrained = 0;
        private double _currentCost;
        private double _costDelta;
        private NeuronLayer _input;

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
        public NeuronLayer Input
        {
            get => _input;
            set
            {
                _input = value;
                if (_input != null)
                {
                    _input.Initialize(this, new Random());
                    NeuronLayer layer = _input;
                    while (layer.Next != null)
                        layer = layer.Next;
                    Output = layer;
                }
            }
        }
        public NeuronLayer Output { get; private set; }
        /// <summary>
        /// Recursively counts the attached layers.
        /// </summary>
        public int GetLayerCount => Input?.GetLayerCount() ?? 0;

        /// <summary>
        /// Propagates inputs through the network and returns the corresponding output values.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Execute(params double[] input)
        {
            if (Input == null)
                throw new InvalidOperationException($"{nameof(Input)} cannot be null.");
            if (Input.NeuronOutValues == null)
                throw new InvalidOperationException($"{nameof(Input)}.{nameof(NeuronLayer.NeuronOutValues)} cannot be null.");
            if (input == null)
                throw new InvalidOperationException($"{nameof(input)} cannot be null.");
            if (Input.NeuronOutValues.Length != input.Length)
                throw new InvalidOperationException($"{nameof(input)}.{nameof(NeuronLayer.NeuronOutValues)} count does not match {nameof(Input)} count.");

            Input.NeuronOutValues = input;

            NeuronLayer output = Input.ForwardPropagate();

            return output.NeuronOutValues;
        }
        public void Train(int iterations, double learningRate)
        {
            if (ExpectedOutput == null)
                throw new InvalidOperationException($"Network needs {nameof(ExpectedOutput)} to be set to train against.");
            if (Input == null)
                throw new InvalidOperationException($"Network needs {nameof(Input)} to be set to forward propagate through layers.");
            if (Input.Next == null)
                throw new InvalidOperationException($"Network needs at least two layers (set {nameof(Input)}.{nameof(NeuronLayer.Next)}).");
            if (learningRate <= 0.0)
                throw new InvalidOperationException($"{nameof(learningRate)} must be greater than zero.");

            if (Output.NeuronOutValues.Length != ExpectedOutput.NeuronOutValues.Length)
                throw new InvalidOperationException($"{nameof(ExpectedOutput)} does not have the same number of neurons as the last layer in the network.");
            
            double totalCost, targetValue, actualValue, neuronCost;
            for (int i = 0; i < iterations; ++i, ++_totalIterationsTrained)
            {
                Input.ForwardPropagate();

                totalCost = 0.0;
                for (int outputNeuronIndex = 0; outputNeuronIndex < Output.NeuronOutValues.Length; ++outputNeuronIndex)
                {
                    targetValue = ExpectedOutput.NeuronOutValues[outputNeuronIndex];
                    actualValue = Output.NeuronOutValues[outputNeuronIndex];
                    neuronCost = Cost.Evaluate(targetValue, actualValue);
                    Output.SetNeuronCost(outputNeuronIndex, neuronCost);
                    totalCost += neuronCost;
                }

                //Update output display here
                NetworkForwardPropagated?.Invoke(Output, totalCost);

                _costDelta = totalCost - _currentCost;
                _currentCost = totalCost;

                Output.BackwardPropagate(learningRate);
            }
        }

        public void ToCSV(string path)
        {
            using (CsvWriter writer = new CsvWriter(new StreamWriter(path, false)))
            {
                writer.WriteField(Input.NeuronOutValues.Length);
                writer.WriteField(Cost.GetType().AssemblyQualifiedName);
                writer.WriteField(_currentCost);
                writer.NextRecord();

                NeuronLayer next = Input;
                while (next != null)
                {
                    writer.WriteField(next.Activation.GetType().AssemblyQualifiedName);
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
                string costTypeStr = reader.GetField(1);
                Type costType = Type.GetType(costTypeStr);
                net.Cost = costType.CreateInstance<CostFunction>();

                NeuronLayer first = new NeuronLayer(null, inputCount);
                NeuronLayer prev = first;
                double cost = reader.GetField<double>(2);

                net.Input = first;
                net._currentCost = cost;

                while (reader.Read())
                {
                    string activTypeStr = reader.GetField(0);
                    Type activType = Type.GetType(activTypeStr);
                    ActivationFunction activFunc = activType.CreateInstance<ActivationFunction>();
                    int weightCount = reader.GetField<int>(1);
                    int biasCount = reader.GetField<int>(2);
                    NeuronLayer layer = new NeuronLayer(activFunc, biasCount);
                    for (int i = 0; i < weightCount; ++i)
                        layer.Weights[i] = reader.GetField<double>(i + 3);
                    for (int i = 0; i < biasCount; ++i)
                        layer.Biases[i] = reader.GetField<double>(i + 3 + weightCount);
                    prev.Next = layer;
                    prev = layer;
                }
            }
            return net;
        }

        public IEnumerator<NeuronLayer> GetEnumerator()
        {
            NeuronLayer layer = _input;
            while (layer != null)
            {
                yield return layer;
                layer = layer.Next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
