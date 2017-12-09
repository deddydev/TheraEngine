using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Basic rendering result.
    /// </summary>
    [FunctionDefinition(
                "Output",
                "Basic Output",
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return")]
    public class ResultBasicFunc : MaterialFunction
    {
        MatFuncValueInput Color;
        MatFuncValueInput Opacity;
        MatFuncValueInput WorldPositionOffset;

        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;
            }
        }

        public ResultBasicFunc() : base(false) { }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            Color = new MatFuncValueInput("Color", ShaderVarType._vec3);
            Opacity = new MatFuncValueInput("Opacity", ShaderVarType._float);
            WorldPositionOffset = new MatFuncValueInput("WorldPositionOffset", ShaderVarType._vec3);
            return new List<MatFuncValueInput>() { Color, Opacity, WorldPositionOffset };
        }
        protected override string GetOperation()
        {
            return FragmentShaderGenerator.OutputColorName + " = {0}";
        }

        public string CompileWorldPositionOffsetCode()
        {
            string code = "";
            return code;
        }

        public string CompileFinalColorCode()
        {
            string code = "";
            return code;
        }
    }
}