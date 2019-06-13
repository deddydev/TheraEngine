using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Core.Maths.MachineLearning
{
    public abstract class ActivationFunction
    {
        public abstract double GetOutputValue(double x);
        public abstract double GetOutputDerivative(double x);
    }
    public class Neuron
    {
        public double Weight { get; set; }
        public double Value { get; set; }

        public List<Neuron> Inputs { get; set; } = new List<Neuron>();
        public ActivationFunction Activation { get; set; } = new AF_ReLU();
        
        public static double ImportanceSum(List<Neuron> neurons, double bias)
            => TMath.Sigmoid(neurons.Sum(x => x.Value * x.Weight) + bias);

        public static double Cost(NeuronSet expected, NeuronSet actual)
        {
            if (expected.Neurons.Count != actual.Neurons.Count)
                return 0.0;

            double sum = 0.0;
            double diff;
            for (int i = 0; i < expected.Neurons.Count; ++i)
            {
                diff = actual.Neurons[i].Value - expected.Neurons[i].Value;
                sum += diff * diff;
            }
            return sum;
        }

        public void Initialize()
        {

        }
    }
}
