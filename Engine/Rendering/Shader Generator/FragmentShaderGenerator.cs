using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEngine.Rendering
{
    public static class FragmentShaderGenerator
    {
        public static readonly string OutputColorName = "OutColor";

        private static ShaderGenerator _generator;
        
        public static Shader Generate(ResultBasicFunc end)
        {
            _generator.Reset();
            _generator.WriteVersion();
            _generator.wl("{0} = ", OutputColorName);
            _generator.Begin();
            return new Shader(ShaderMode.Fragment, _generator.Finish());
        }
    }
}
