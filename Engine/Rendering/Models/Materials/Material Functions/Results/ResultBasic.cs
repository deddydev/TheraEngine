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
        public MatFuncValueInput Color { get; set; }
        public MatFuncValueInput Opacity { get; set; }
        public MatFuncValueInput WorldPositionOffset { get; set; }
        
        public ResultBasicFunc() : base(false) { }

        protected override List<MatFuncValueInput> GetValueInputs()
        {
            Color = new MatFuncValueInput("Color", ShaderVarType._vec3);
            Opacity = new MatFuncValueInput("Opacity", ShaderVarType._float);
            WorldPositionOffset = new MatFuncValueInput("WorldPositionOffset", ShaderVarType._vec3);
            return new List<MatFuncValueInput>() { Color, Opacity, WorldPositionOffset };
        }
    }
}