using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    public abstract class ComparableFunc : MaterialFunction
    {
        MatFuncValueInput A, B;
        MatFuncValueOutput Result;

        public ComparableFunc() : base(true) { }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            A = new MatFuncValueInput("A", NumericTypes);
            B = new MatFuncValueInput("B", A);
            return new List<MatFuncValueInput>() { A, B };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            Result = new MatFuncValueOutput("Result", ShaderVarType._bool);
            return new List<MatFuncValueOutput>() { Result };
        }
        protected override string GetOperation()
        {
            return "{0} " + GetOperator() + " {1}";
        }
        protected abstract string GetOperator();
    }
}
