using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class OperatorFunc : ShaderMethod
    {
        public const string CategoryName = "Basic Operators";
        
        public OperatorFunc() : base(NumericTypes) { }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput A = new MatFuncValueInput("A", OutputArguments[0]);
            MatFuncValueInput B = new MatFuncValueInput("B", A);
            return new MatFuncValueInput[] { A, B };
        }
        protected override string GetOperation()
            => "{0} " + GetOperator() + " {1}";
        protected abstract string GetOperator();
    }
}
