using System;

namespace TheraEngine.Core.Maths.MachineLearning
{
    /// <summary>
    /// [0, 1]
    /// </summary>
    public class AF_Sigmoid : ActivationFunction
    {
        public override double GetOutputValue(double x)
        {
            return TMath.Sigmoid(x);
        }
        public override double GetOutputDerivative(double x)
        {
            double y = GetOutputValue(x);
            return y * (1.0 - y);
        }
    }
}
