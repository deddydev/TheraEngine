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
        public ResultPBRFunc() : base()
        {
            NecessaryMeshParams.Add(new MeshParam(EMeshValue.FragNorm, 0));
        }
        public override string GetGlobalVarDec()
        {
            return @"
layout(location = 0) out vec4 AlbedoOpacity;
layout(location = 1) out vec3 Normal;
layout(location = 2) out vec4 RMSI;";
        }

        protected override string GetOperation()
        {
            return @"
AlbedoOpacity = vec4({0}, {1});
Normal = {2};
RMSI = vec4({3}, {4}, {5}, {6});";
        }

        public override void GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads)
        {
            inputNames = new string[] { "Albedo", "Opacity", "World Normal", "Roughness", "Metallic", "Specularity", "Refraction", "World Position Offset" };
            outputNames = new string[] { };
            overloads = new MatFuncOverload[]
            {
                new MatFuncOverload(EGLSLVersion.Ver_110, new EGenShaderVarType[]
                {
                    EGenShaderVarType.Vec3,
                    EGenShaderVarType.Float,
                    EGenShaderVarType.Vec3,
                    EGenShaderVarType.Float,
                    EGenShaderVarType.Float,
                    EGenShaderVarType.Float,
                    EGenShaderVarType.Float,
                    EGenShaderVarType.Vec3,
                }, true)
            };
        }
    }
}
