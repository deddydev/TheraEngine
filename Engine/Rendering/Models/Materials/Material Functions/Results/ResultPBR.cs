using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Physically-based rendering result.
    /// </summary>
    [FunctionDefinition(
        "Output",
        "Output PBR",
        "Combines the given inputs using a physically-based shading pipeline.",
        "result output final return physically based rendering PBR albedo roughness shininess specularity metallic refraction")]
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
