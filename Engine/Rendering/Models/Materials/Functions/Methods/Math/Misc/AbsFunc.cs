using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Returns the absolute value of the input value
    /// </summary>
    [FunctionDefinition(
        "Helpers",
        "|Value|",
        "Returns the absolute value of the given value; |value|", 
        "absolute value")]
    public class AbsFunc : ShaderMethod
    {
        public AbsFunc() : base(SignedTypes) { }
        protected override string GetOperation() => "abs({0})";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput InputValue = new MatFuncValueInput("Value", OutputArguments[0]);
            return new MatFuncValueInput[] { InputValue };
        }
    }
}
