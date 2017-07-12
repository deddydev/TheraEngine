using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
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
        MatFuncValueInput FinalColor;
        MatFuncValueInput WorldPositionOffset;

        public ResultBasicFunc() : base(true) { }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            FinalColor = new MatFuncValueInput("FinalColor", ShaderVarType._vec4);
            WorldPositionOffset = new MatFuncValueInput("WorldPositionOffset", ShaderVarType._vec3);
            return new List<MatFuncValueInput>() { FinalColor };
        }
        protected override string GetOperation()
        {
            return FragmentShaderGenerator.OutputColorName + " = {0}";
        }
    }
}