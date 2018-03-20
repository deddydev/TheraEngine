using System;
using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// returns the absolute value of the input value
    /// </summary>
    [FunctionDefinition(
        OperatorFunc.CategoryName,
        "Negate",
        "Returns 0 - value.",
        "negate negative zero - value minus")]
    public class NegateFunc : ShaderMethod
    {
        public NegateFunc() : base(SignedTypes) { }
        protected override string GetOperation() => "-{0}";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput InputValue = new MatFuncValueInput(string.Empty, OutputArguments[0]);
            return new MatFuncValueInput[] { InputValue };
        }
    }
}
