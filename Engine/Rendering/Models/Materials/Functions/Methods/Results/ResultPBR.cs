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
        public ResultPBRFunc() : base() { NecessaryMeshParams.Add(new MeshParam(EMeshValue.FragNorm, 0)); }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput Albedo = new MatFuncValueInput("Albedo", ShaderVarType._vec3);
            MatFuncValueInput Opacity = new MatFuncValueInput("Opacity", ShaderVarType._float);
            MatFuncValueInput Roughness = new MatFuncValueInput("Roughness", ShaderVarType._float);
            MatFuncValueInput Metallic = new MatFuncValueInput("Metallic", ShaderVarType._float);
            MatFuncValueInput Specularity = new MatFuncValueInput("Specularity", ShaderVarType._float);
            MatFuncValueInput Refraction = new MatFuncValueInput("Refraction", ShaderVarType._float);
            return new MatFuncValueInput[] { Albedo, Opacity, Roughness, Metallic, Specularity, Refraction };
        }
        public override string GetGlobalVarDec()
        {
            return @"layout(location = 0) out vec4 AlbedoOpacity;
layout(location = 1) out vec3 Normal;
layout(location = 2) out vec4 RMSI;";
        }

        protected override string GetOperation()
        {
            return @"AlbedoOpacity = vec4({0}, {1});
Normal = FragNorm;
RMSI = vec4({2}, {3}, {4}, {5});";
        }
    }
}
