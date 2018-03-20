using System;
using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Returns 1 / input
    /// </summary>
    [FunctionDefinition(
        "Helpers",
        "Inverse",
        "Returns 1 / value.",
        "inverse divided divison one 1 over value")]
    public class InverseFunc : ShaderMethod
    {
        public InverseFunc() : base(ShaderVar.FloatingPointTypes) { }
        protected override string GetOperation()
            => One(InputArguments[0].ArgumentType) + " / {0}";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput InputValue = new MatFuncValueInput("Value", OutputArguments[0]);
            return new MatFuncValueInput[] { InputValue };
        }
    }
}
