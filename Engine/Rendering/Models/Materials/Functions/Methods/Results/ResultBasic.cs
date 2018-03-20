using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Basic rendering result.
    /// </summary>
    [FunctionDefinition(
        "Output",
        "Basic Output [Forward]",
        "Outputs the given vec4 color as the color for this fragment in a forward shading pipeline.",
        "result output final return")]
    public class ResultBasicFunc : ResultFunc
    {
        public ResultBasicFunc() : base() { }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput Color = new MatFuncValueInput("Color", ShaderVarType._vec3);
            MatFuncValueInput Opacity = new MatFuncValueInput("Opacity", ShaderVarType._float);
            MatFuncValueInput  WorldPositionOffset = new MatFuncValueInput("WorldPositionOffset", ShaderVarType._vec3);
            return new MatFuncValueInput[] { Color, Opacity, WorldPositionOffset };
        }
        protected override string GetOperation()
            => "OutColor = vec4({0}, {1})";
        protected override string GetGlobalVarDec()
            => "layout(location = 0) out vec4 OutColor;";
    }
}