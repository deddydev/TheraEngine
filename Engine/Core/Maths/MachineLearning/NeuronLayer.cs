using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public class NeuronLayer
    {
        public NeuronLayer(ActivationFunction activation, int neuronCount)
        {
            Activation = activation;

            _neuronOutValues = new double[neuronCount];
            _neuronBiases = new double[neuronCount];
            _neuronOutDerivatives = new double[neuronCount];

            _weights = new double[0];
            _prevWeights = new double[0];
            _neuronCosts = new double[0];
        }
        public NeuronLayer(params double[] values) : this(null, values?.Length ?? 0)
        {
            if (values == null)
                return;
            for (int i = 0; i < values.Length; ++i)
                _neuronOutValues[i] = values[i];
        }

        public int GetLayerCount() => 1 + (_next?.GetLayerCount() ?? 0);

        private ActivationFunction _activation = new AF_ReLU();

        private NeuronLayer _previous;
        private NeuronLayer _next;
        
        private double[] _weights;
        private double[] _prevWeights;
        private double[] _neuronCosts;

        private double[] _neuronOutValues;
        private double[] _neuronBiases;
        private double[] _neuronOutDerivatives;

        public double[] Weights => _weights;
        public double[] Biases => _neuronBiases;

        public Network OwningNetwork { get; private set; }
        public Dictionary<string, int> NeuronNameIndexAssocations { get; set; }
        public ActivationFunction Activation
        {
            get => _activation;
            set => _activation = value ?? new AF_ReLU();
        }

        public double[] NeuronOutValues
        {
            get => _neuronOutValues;
            set
            {
                int prevCount = _neuronOutValues.Length;
                _neuronOutValues = value ?? new double[0];
                int nCount = _neuronOutValues.Length;
                if (nCount != prevCount)
                {
                    _neuronBiases.Resize(nCount);
                    _neuronOutDerivatives.Resize(nCount);
                    RemakeWeights();
                    _next?.RemakeWeights();
                    OwningNetwork.LayersChanged();
                }
            }
        }
        
        public NeuronLayer Previous
        {
            get => _previous;
            set
            {
                if (_previous != null)
                    _previous._next = null;
                _previous = value;
                if (_previous != null)
                    _previous._next = this;
                RemakeWeights();
            }
        }
        public NeuronLayer Next
        {
            get => _next;
            set
            {
                if (_next != null)
                {
                    _next._previous = null;
                    _next.RemakeWeights();
                }
                _next = value;
                if (_next != null)
                {
                    _next._previous = this;
                    _next.RemakeWeights();
                }
            }
        }

        public double? GetNeuronValue(string name)
        {
            if (NeuronNameIndexAssocations == null || 
                !NeuronNameIndexAssocations.ContainsKey(name))
                return null;

            int index = NeuronNameIndexAssocations[name];
            if (_neuronOutValues.IndexInArrayRange(index))
                return _neuronOutValues[index];

            return null;
        }

        private void RemakeWeights()
        {
            int count = _previous._neuronOutValues.Length * _neuronOutValues.Length;
            _weights.Resize(count);
            _prevWeights.Resize(count);
            _neuronCosts.Resize(count);
        }

        public NeuronLayer Initialize(Network owner, Random rand, bool recursive)
        {
            OwningNetwork = owner;

            int prevNeuronCount = _previous?._neuronOutValues?.Length ?? 0;
            for (int neuronIndex = 0; neuronIndex < _neuronOutValues.Length; ++neuronIndex)
            {
                _neuronBiases[neuronIndex] = rand.NextDouble();
                for (int prevNeuronIndex = 0; prevNeuronIndex < prevNeuronCount; ++prevNeuronIndex)
                    _weights[neuronIndex * prevNeuronCount + prevNeuronIndex] = rand.NextDouble();
            }

            if (recursive)
                return Next?.Initialize(owner, rand, true) ?? this;

            return this;
        }

        public NeuronLayer GetLast()
        {
            NeuronLayer layer = this;
            while (layer.Next != null)
                layer = layer.Next;
            return layer;
        }

        public NeuronLayer ForwardPropagate()
        {
            if (_previous != null)
            {
                int prevNeuronCount = _previous._neuronOutValues.Length;
                //for (int neuronIndex = 0; neuronIndex < _neuronActivationValues.Length; ++neuronIndex)
                Parallel.For(0, _neuronOutValues.Length, neuronIndex =>
                {
                    double sum = _neuronBiases[neuronIndex];
                    for (int prevNeuronIndex = 0; prevNeuronIndex < prevNeuronCount; ++prevNeuronIndex)
                    {
                        double weight = GetWeight(neuronIndex, prevNeuronIndex, false);
                        sum += _previous._neuronOutValues[prevNeuronIndex] * weight;
                    }
                    _neuronOutDerivatives[neuronIndex] = Activation.Derivative(sum);
                    _neuronOutValues[neuronIndex] = Activation.Value(sum);
                }
                );
            }
            return _next?.ForwardPropagate() ?? this;
        }
        public void BackwardPropagate(double learningRate)
        {
            if (_previous == null)
                return;
            
            int totalConnections = 
                _neuronOutValues.Length *
                _previous._neuronOutValues.Length;

            Parallel.For(0, totalConnections, conn =>
            //for (int conn = 0; conn < totalConnections; ++conn)
            {
                int neuronIndex = conn / _previous._neuronOutValues.Length;
                int prevNeuronIndex = conn % _previous._neuronOutValues.Length;

                double dTotROut;
                double dOutRNet = _neuronOutDerivatives[neuronIndex];
                double dNetRWt = _previous._neuronOutValues[prevNeuronIndex];
                if (_next == null)
                {
                    dTotROut = GetNeuronCost(neuronIndex);
                }
                else
                {
                    dTotROut = 0.0;
                    for (int i = 0; i < _next._neuronOutValues.Length; ++i)
                    {
                        double dNextTotROut = _next.GetNeuronCost(neuronIndex);
                        double dNextOutRNet = _next._neuronOutDerivatives[neuronIndex];
                        double dNextTotRNet = dNextTotROut * dNextOutRNet;
                        double dNextNetROut = _next.GetWeight(i, neuronIndex, true);
                        dTotROut += dNextTotRNet * dNextNetROut;
                    }
                }

                double dTotRNet = dTotROut * dOutRNet;
                SetNeuronCost(neuronIndex, dTotRNet);
                double dTotRWt = dTotRNet * dNetRWt;

                double weight = GetWeight(neuronIndex, prevNeuronIndex, false);
                weight -= learningRate * dTotRWt;
                SetWeight(neuronIndex, prevNeuronIndex, weight);

                if (prevNeuronIndex == 0)
                {
                    //Set bias for this neuron
                    double dTotRBias = dTotRNet * 1.0;
                    double bias = _neuronBiases[neuronIndex];
                    bias -= learningRate * dTotRBias;
                    _neuronBiases[neuronIndex] = bias;
                }
            }
            );

            _previous.BackwardPropagate(learningRate);
        }

        /// <summary>
        /// Gets or sets a weight between this and the previous layer.
        /// </summary>
        /// <param name="neuronIndex">The neuron in this layer the weight affects.</param>
        /// <param name="inputNeuronIndex">The neuron in the previous layer this weight affects.</param>
        /// <returns>The weight value.</returns>
        public double GetWeight(int neuronIndex, int inputNeuronIndex, bool previous)
        {
            //if (_weights == null ||
            //    _previous?._weights == null ||
            //    neuronIndex >= _weights.Length ||
            //    inputNeuronIndex >= _previous._weights.Length)
            //    return 0.0f;

            int index = _previous._neuronOutValues.Length * neuronIndex + inputNeuronIndex;
            return previous ? _prevWeights[index] : _weights[index];
        }
        /// <summary>
        /// Gets or sets a weight between this and the previous layer.
        /// </summary>
        /// <param name="neuronIndex">The neuron in this layer the weight affects.</param>
        /// <param name="inputNeuronIndex">The neuron in the previous layer this weight affects.</param>
        /// <returns>The weight value.</returns>
        public void SetWeight(int neuronIndex, int inputNeuronIndex, double weight)
        {
            //if (_weights == null ||
            //    _previous?._weights == null ||
            //    neuronIndex >= _weights.Length ||
            //    inputNeuronIndex >= _previous._weights.Length)
            //    return;

            int index = _previous._neuronOutValues.Length * neuronIndex + inputNeuronIndex;
            _prevWeights[index] = _weights[index];
            _weights[index] = weight;
        }
        /// <summary>
        /// Gets or sets a weight between this and the previous layer.
        /// </summary>
        /// <param name="neuronIndex">The neuron in this layer the weight affects.</param>
        /// <param name="inputNeuronIndex">The neuron in the previous layer this weight affects.</param>
        /// <returns>The weight value.</returns>
        public double GetNeuronCost(int neuronIndex)
        {
            //if (_weightDeltas == null ||
            //    _previous?._weightDeltas == null ||
            //    neuronIndex >= _weightDeltas.Length ||
            //    inputNeuronIndex >= _previous._weightDeltas.Length)
            //    return 0.0f;

            return _neuronCosts[neuronIndex];
        }
        /// <summary>
        /// Gets or sets a weight between this and the previous layer.
        /// </summary>
        /// <param name="neuronIndex">The neuron in this layer the weight affects.</param>
        /// <param name="inputNeuronIndex">The neuron in the previous layer this weight affects.</param>
        /// <returns>The weight value.</returns>
        public void SetNeuronCost(int neuronIndex, double cost)
        {
            //if (_weightDeltas == null ||
            //    _previous?._weightDeltas == null ||
            //    neuronIndex >= _weightDeltas.Length ||
            //    inputNeuronIndex >= _previous._weightDeltas.Length)
            //    return;

            _neuronCosts[neuronIndex] = cost;
        }
    }
}
