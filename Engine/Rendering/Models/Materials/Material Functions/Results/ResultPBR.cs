using System.Collections.Generic;
using TheraEngine.Rendering.HUD.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    /// <summary>
    /// Physically-based rendering result.
    /// </summary>
    [FunctionDefinition(
                "Output",
                "Output PBR",
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return physically based rendering PBR")]
    public class ResultPBRFunc : ResultBasicFunc
    {
        protected override string GetOperation()
        {
            return base.GetOperation();
        }

        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("Albedo", ShaderVarType._vec4),
                new MatFuncValueInput("Roughness", ShaderVarType._float),
                new MatFuncValueInput("Shininess", ShaderVarType._float),
                new MatFuncValueInput("Specularity", ShaderVarType._float),
                new MatFuncValueInput("Metallic", ShaderVarType._float),
                new MatFuncValueInput("Refraction", ShaderVarType._float),
            };
        }
    }
}
