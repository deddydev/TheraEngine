using System.Collections.Generic;

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
                new MatFuncValueInput("Diffuse", ShaderType._vec4),
                new MatFuncValueInput("Roughness", ShaderType._float),
                new MatFuncValueInput("Shininess", ShaderType._float),
                new MatFuncValueInput("Specularity", ShaderType._float),
                new MatFuncValueInput("Metallic", ShaderType._float),
                new MatFuncValueInput("Refraction", ShaderType._float),
            };
        }
    }
}
