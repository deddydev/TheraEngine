using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Maths.MachineLearning
{
    /// <summary>
    /// [0, infinity)
    /// </summary>
    public class AF_ReLU : ActivationFunction
    {
        public override double GetOutputValue(double x)
        {
            if (x < 0.0)
                return 0.0;
            return x;
        }
        public override double GetOutputDerivative(double x)
        {
            if (x < 0.0)
                return 0.0;
            return 1.0;
        }
    }
}
