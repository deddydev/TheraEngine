using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class OperatorFunc : MaterialFunction
    {
        GLInput A, B;
        GLOutput Result;

        public OperatorFunc() : base(true) { }
        protected override List<GLInput> GetInputs()
        {
            A = new GLInput("A", NumericTypes);
            B = new GLInput("B", A);
            return new List<GLInput>() { A, B };
        }
        protected override List<GLOutput> GetOutputs()
        {
            Result = new GLOutput("Result", A);
            return new List<GLOutput>() { Result };
        }
        protected override string GetOperation()
        {
            return "{0} " + GetOperator() + " {1}";
        }
        protected abstract string GetOperator();
    }
}
