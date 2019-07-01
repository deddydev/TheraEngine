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
        public Network() { }
        public Network(int inputCount, CostFunction costFunc, params NeuronLayer[] layers)
        {
            CostFunction = costFunc;
            NeuronLayer first = new NeuronLayer(null, inputCount, false);
            if (layers == null || layers.Length == 0)
                return;
            for (int i = 1; i < layers.Length; ++i)
                layers[i].Previous = layers[i - 1];
            first.Next = layers[0];
            Input = first;
        }
        
        public delegate void DelForwardPropagated(double[] output);
        public delegate void DelCostChanged(double oldCost, double newCost);
        
        public event DelForwardPropagated ForwardPropagated;
        public event DelCostChanged CostChanged;

        public double ConfidencePercentage => (1.0f - _currentCost) * 100.0f;

        private double _currentCost;
        private NeuronLayer _input;

        /// <summary>
        /// The method to use to calculate the cost of an output neuron.
        /// </summary>
        public CostFunction CostFunction { get; set; } = new CF_DiffSquared();
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
                    Random random = new Random();
                    NeuronLayer layer = _input;
                    layer.Initialize(this, random, false);
                    while (layer.Next != null)
                    {
                        layer = layer.Next;
                        layer.Initialize(this, random, false);
                    }
                    Output = layer;
                    LayersChanged();
                }
            }
        }
        public NeuronLayer Output { get; private set; }

        internal void LayersChanged()
        {
            LayerCount = Input?.GetLayerCount() ?? 0;
        }

        /// <summary>
        /// Recursively counts the attached layers.
        /// </summary>
        public int LayerCount { get; private set; }

        public double PreviousCost { get; private set; }
        public double CurrentCost
        {
            get => _currentCost;
            private set
            {
                PreviousCost = _currentCost;
                _currentCost = value;
                CostChanged?.Invoke(PreviousCost, _currentCost);
            }
        }

        public double TotalIterationsTrained { get; private set; } = 0;

        /// <summary>
        /// Propagates inputs through the network and returns the corresponding output values.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Calculate(params double[] input)
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

            NeuronLayer output = Input.FeedForward();

            return output.NeuronOutValues;
        }
        /// <summary>
        /// Propagates inputs through the network and returns the corresponding output values.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] CalculateError(double[] input, double[] expectedOutput, out double totalError)
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

            NeuronLayer output = Input.FeedForward();

            totalError = 0.0;
            double[] error = new double[output.NeuronOutValues.Length];
            for (int i = 0; i < output.NeuronOutValues.Length; ++i)
                totalError += error[i] = expectedOutput[i] - output.NeuronOutValues[i];

            return error;
        }
        public void Train(
            int iterations,
            double learningRate,
            params Tuple<double[], double[]>[] trainingSets)
        {
            //if (expectedOutput == null)
            //    throw new InvalidOperationException($"Network needs {nameof(expectedOutput)} to be set to train against.");
            //if (Input == null)
            //    throw new InvalidOperationException($"Network needs {nameof(Input)} to be set to forward propagate through layers.");
            //if (Input.Next == null)
            //    throw new InvalidOperationException($"Network needs at least two layers (set {nameof(Input)}.{nameof(NeuronLayer.Next)}).");
            //if (learningRate <= 0.0)
            //    throw new InvalidOperationException($"{nameof(learningRate)} must be greater than zero.");

            //if (Output.NeuronOutValues.Length != expectedOutput.Length)
            //    throw new InvalidOperationException($"{nameof(ExpectedOutput)} does not have the same number of neurons as the last layer in the network.");

            double[] expectedOutput;
            int setIndex;
            double totalCost, targetValue, actualValue, neuronCost, neuronCostDeriv;
            for (int i = 0; i < iterations; ++i, ++TotalIterationsTrained)
            {
                setIndex = i % trainingSets.Length;
                expectedOutput = trainingSets[setIndex].Item2;
                Input.NeuronOutValues = trainingSets[setIndex].Item1;
                Input.FeedForward();

                totalCost = 0.0;
                for (int outputNeuronIndex = 0; outputNeuronIndex < Output.NeuronOutValues.Length; ++outputNeuronIndex)
                {
                    targetValue = expectedOutput[outputNeuronIndex];
                    actualValue = Output.NeuronOutValues[outputNeuronIndex];
                    neuronCost = CostFunction.Evaluate(targetValue, actualValue);
                    neuronCostDeriv = CostFunction.Derivative(targetValue, actualValue);
                    Output.SetNeuronDelta(outputNeuronIndex, neuronCostDeriv);
                    totalCost += neuronCost;
                }

                CurrentCost = totalCost;

                //Update output display here
                ForwardPropagated?.Invoke(Output.NeuronOutValues);

                Output.BackPropagate(learningRate);
            }
        }

        public void ToCSV(string path)
        {
            using (CsvWriter writer = new CsvWriter(new StreamWriter(path, false)))
            {
                writer.WriteField(Input.NeuronOutValues.Length);
                writer.WriteField(CostFunction.GetType().AssemblyQualifiedName);
                writer.WriteField(CurrentCost);
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
                net.CostFunction = costType.CreateInstance<CostFunction>();

                NeuronLayer first = new NeuronLayer(null, inputCount, false);
                NeuronLayer prev = first;
                double cost = reader.GetField<double>(2);

                net.Input = first;
                net.CurrentCost = cost;

                while (reader.Read())
                {
                    string activTypeStr = reader.GetField(0);
                    Type activType = Type.GetType(activTypeStr);
                    ActivationFunction activFunc = activType.CreateInstance<ActivationFunction>();
                    int weightCount = reader.GetField<int>(1);
                    int biasCount = reader.GetField<int>(2);
                    NeuronLayer layer = new NeuronLayer(activFunc, biasCount, true);
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
