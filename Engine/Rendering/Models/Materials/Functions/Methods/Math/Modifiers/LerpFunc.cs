using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// a * (1 − t) + b * t. 
    /// </summary>
    [FunctionDefinition(
        "Helpers",
        "Lerp",
        "Linearly interpolates between A and B using Time: A + (B - A) * Time",
        "lerp mix linear interpolate blend")]
    public class LerpFunc : ShaderMethod
    {
        public LerpFunc() : base(ShaderVar.FloatingPointTypes) { }
        protected override string GetOperation() => "mix({0}, {1}, {2})";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput A = new MatFuncValueInput("A", OutputArguments[0]);
            MatFuncValueInput B = new MatFuncValueInput("B", A);
            MatFuncValueInput Time = new MatFuncValueInput("Time", ShaderVarType._float);
            return new MatFuncValueInput[] { A, B, Time };
        }
    }
}
