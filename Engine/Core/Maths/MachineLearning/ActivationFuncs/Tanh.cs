using System;

namespace TheraEngine.Core.Maths.MachineLearning
{
    /// <summary>
    /// [-1, 1]
    /// </summary>
    public class AF_TanH : ActivationFunction
    {
        public override double GetOutputValue(double x)
        {
            return Math.Tanh(x);
        }
        public override double GetOutputDerivative(double x)
        {
            double y = GetOutputValue(x);
            return 1.0 - y * y;
        }
    }
}
