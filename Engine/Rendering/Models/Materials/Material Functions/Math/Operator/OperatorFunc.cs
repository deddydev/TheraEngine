using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class OperatorFunc : MaterialFunction
    {
        FuncValueInput A, B;
        FuncValueOutput Result;

        public OperatorFunc() : base(true) { }
        protected override List<FuncValueInput> GetInputs()
        {
            A = new FuncValueInput("A", NumericTypes);
            B = new FuncValueInput("B", A);
            return new List<FuncValueInput>() { A, B };
        }
        protected override List<FuncValueOutput> GetOutputs()
        {
            Result = new FuncValueOutput("Result", A);
            return new List<FuncValueOutput>() { Result };
        }
        protected override string GetOperation()
            => "{0} " + GetOperator() + " {1}";
        
        protected abstract string GetOperator();
    }
}
