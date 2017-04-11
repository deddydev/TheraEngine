using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class OperatorFunc : MaterialFunction
    {
        MatFuncValueInput A, B;
        MatFuncValueOutput Result;

        public OperatorFunc() : base(true) { }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            A = new MatFuncValueInput("A", NumericTypes);
            B = new MatFuncValueInput("B", A);
            return new List<MatFuncValueInput>() { A, B };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            Result = new MatFuncValueOutput("Result", A);
            return new List<MatFuncValueOutput>() { Result };
        }
        protected override string GetOperation()
            => "{0} " + GetOperator() + " {1}";
        
        protected abstract string GetOperator();
    }
}
