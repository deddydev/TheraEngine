using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Physically-based rendering result.
    /// </summary>
    [FunctionDefinition(
        "Output",
        "PBR Output [Deferred]",
        "Combines the given inputs using a deferred physically-based shading pipeline.",
        "result output final return physically based rendering PBR albedo roughness shininess specularity metallic refraction")]
    public class ResultPBRFunc : ResultFunc
    {
        public MatFuncValueInput Albedo { get; set; }
        public MatFuncValueInput Roughness { get; set; }
        public MatFuncValueInput Shininess { get; set; }
        public MatFuncValueInput Specularity { get; set; }
        public MatFuncValueInput Metallic { get; set; }
        public MatFuncValueInput Refraction { get; set; }

        public ResultPBRFunc() : base(false) { }

        protected override List<MatFuncValueInput> GetValueInputs()
        {
            Albedo = new MatFuncValueInput("Albedo", ShaderVarType._vec4);
            Roughness = new MatFuncValueInput("Roughness", ShaderVarType._float);
            Shininess = new MatFuncValueInput("Shininess", ShaderVarType._float);
            Specularity = new MatFuncValueInput("Specularity", ShaderVarType._float);
            Metallic = new MatFuncValueInput("Metallic", ShaderVarType._float);
            Refraction = new MatFuncValueInput("Refraction", ShaderVarType._float);
            return new List<MatFuncValueInput>() { Albedo, Roughness, Shininess, Specularity, Metallic, Refraction };
        }
    }
}
