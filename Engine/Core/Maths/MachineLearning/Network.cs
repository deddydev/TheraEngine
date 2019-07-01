﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public enum EErrorTrainingType
    {
        Individual,
        IndividualWeighted,
        Total,
    }
    public delegate void DelForwardPropagated(double[] output);
    public delegate void DelCostChanged(double oldCost, double newCost, int iteration);
    public class Network : IEnumerable<NeuronLayer>
    {
        public Network() { }
        public Network(int inputCount, CostFunction costFunc, params NeuronLayer[] layers)
        {
            CostFunction = costFunc;
            NeuronLayer first = NeuronLayer.Dense(null, inputCount, false);
            if (layers == null || layers.Length == 0)
                return;
            for (int i = 1; i < layers.Length; ++i)
                layers[i].Previous = layers[i - 1];
            first.Next = layers[0];
            Input = first;
        }
                
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
                CostChanged?.Invoke(PreviousCost, _currentCost, TotalIterationsTrained);
            }
        }

        public int TotalIterationsTrained { get; private set; } = 0;

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
            for (int iteration = 0; iteration < iterations; ++iteration, ++TotalIterationsTrained)
            {
                setIndex = iteration % trainingSets.Length;
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
        public void Train(
            double maxError,
            EErrorTrainingType errorType,
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

            Random r = new Random();
            double[] expectedOutput;
            int setIndex;
            double totalCost, targetValue, actualValue, neuronCost, neuronCostDeriv;
            int iteration = 0;
            switch (errorType)
            {
                case EErrorTrainingType.Individual:
                {
                    int successful = 0;
                    do
                    {
                        setIndex = iteration++ % trainingSets.Length; //r.Next(0, trainingSets.Length - 1);
                        if (setIndex == 0)
                            successful = 0;

                        expectedOutput = trainingSets[setIndex].Item2;
                        Input.NeuronOutValues = trainingSets[setIndex].Item1;
                        Input.FeedForward();

                        totalCost = 0.0;
                        for (int outputNeuronIndex = 0; outputNeuronIndex < Output.NeuronOutValues.Length; ++outputNeuronIndex)
                        {
                            targetValue = expectedOutput[outputNeuronIndex];
                            actualValue = Output.NeuronOutValues[outputNeuronIndex];

                            if (Math.Abs(targetValue - actualValue) < maxError)
                                ++successful;

                            neuronCost = CostFunction.Evaluate(targetValue, actualValue);
                            neuronCostDeriv = CostFunction.Derivative(targetValue, actualValue);
                            Output.SetNeuronDelta(outputNeuronIndex, neuronCostDeriv);
                            totalCost += neuronCost;
                        }

                        CurrentCost = totalCost;

                        //Update output display here
                        ForwardPropagated?.Invoke(Output.NeuronOutValues);

                        if (successful == trainingSets.Length * Output.NeuronOutValues.Length)
                            break;

                        Output.BackPropagate(learningRate);
                        ++TotalIterationsTrained;
                    }
                    while (true);
                }
                break;
                case EErrorTrainingType.IndividualWeighted:
                {
                    int successful = 0;
                    do
                    {
                        setIndex = iteration++ % trainingSets.Length;
                        if (setIndex == 0)
                            successful = 0;

                        expectedOutput = trainingSets[setIndex].Item2;
                        Input.NeuronOutValues = trainingSets[setIndex].Item1;
                        Input.FeedForward();

                        totalCost = 0.0;
                        for (int outputNeuronIndex = 0; outputNeuronIndex < Output.NeuronOutValues.Length; ++outputNeuronIndex)
                        {
                            targetValue = expectedOutput[outputNeuronIndex];
                            actualValue = Output.NeuronOutValues[outputNeuronIndex];

                            neuronCost = CostFunction.Evaluate(targetValue, actualValue);
                            if (Math.Abs(neuronCost) < maxError)
                                ++successful;

                            neuronCostDeriv = CostFunction.Derivative(targetValue, actualValue);
                            Output.SetNeuronDelta(outputNeuronIndex, neuronCostDeriv);
                            totalCost += neuronCost;
                        }

                        CurrentCost = totalCost;

                        //Update output display here
                        ForwardPropagated?.Invoke(Output.NeuronOutValues);

                        if (successful == trainingSets.Length * Output.NeuronOutValues.Length)
                            break;

                        Output.BackPropagate(learningRate);
                        ++TotalIterationsTrained;
                    }
                    while (true);
                }
                break;
                case EErrorTrainingType.Total:
                {
                    do
                    {
                        setIndex = iteration++ % trainingSets.Length;
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

                        if (Math.Abs(CurrentCost) < maxError)
                            break;

                        Output.BackPropagate(learningRate);
                        ++TotalIterationsTrained;
                    }
                    while (true);
                }
                break;
            }
        }
        public void ToCSV(string path)
        {
            using (CsvWriter writer = new CsvWriter(new StreamWriter(path, false)))
            {
                writer.WriteField(Input.NeuronOutValues.Length.ToString(CultureInfo.InvariantCulture));
                writer.WriteField(CostFunction.GetType().AssemblyQualifiedName.Replace(",", "|"));
                writer.WriteField(CurrentCost.ToString(CultureInfo.InvariantCulture));
                writer.NextRecord();

                NeuronLayer layer = Input;
                while (layer != null)
                {
                    writer.WriteField(layer.Activation.GetType().AssemblyQualifiedName.Replace(",", "|"));
                    writer.WriteField(layer.Weights.Length.ToString(CultureInfo.InvariantCulture));
                    writer.WriteField(layer.Biases.Length.ToString(CultureInfo.InvariantCulture));
                    writer.WriteField(layer.UseBias);

                    for (int i = 0; i < layer.Weights.Length; ++i)
                        writer.WriteField(layer.Weights[i].ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < layer.Biases.Length; ++i)
                        writer.WriteField(layer.Biases[i].ToString(CultureInfo.InvariantCulture));

                    writer.NextRecord();
                    layer = layer.Next;
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
                string costTypeStr = reader.GetField(1).Replace("|", ",");
                double cost = reader.GetField<double>(2);

                Type costType = Type.GetType(costTypeStr);
                net.CostFunction = costType.CreateInstance<CostFunction>();

                NeuronLayer prev = NeuronLayer.Dense(null, inputCount, false);

                net.Input = prev;
                net.CurrentCost = cost;

                while (reader.Read())
                {
                    string activTypeStr = reader.GetField(0).Replace("|", ",");
                    int weightCount = reader.GetField<int>(1);
                    int biasCount = reader.GetField<int>(2);
                    bool useBias = reader.GetField<bool>(3);
                    
                    Type activType = Type.GetType(activTypeStr);
                    ActivationFunction activFunc = activType.CreateInstance<ActivationFunction>();

                    NeuronLayer layer = NeuronLayer.Dense(activFunc, biasCount, useBias);

                    for (int i = 0; i < weightCount; ++i)
                        layer.Weights[i] = reader.GetField<double>(i + 4);

                    for (int i = 0; i < biasCount; ++i)
                        layer.Biases[i] = reader.GetField<double>(i + 4 + weightCount);

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
