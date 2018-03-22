using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        "Textures",
        "Texture 2D Sample",
        "Gets an RGBA color from the selected texture.",
        "texture 2D sample rgba")]
    public class Texture2DSample : ShaderMethod
    {
        public Texture2DSample() : base(ShaderVarType._vec4) { HasGlobalVarDec = true; }
        protected override string GetOperation() => "texture({0}, {1})";
        public override string GetGlobalVarDec() => "uniform sampler2D Texture";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput uv = new MatFuncValueInput("UVs", ShaderVarType._vec2);
            return new MatFuncValueInput[] { uv };
        }
    }
}
