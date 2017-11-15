using TheraEngine.Rendering.Models.Materials;
using System.Linq;

namespace TheraEngine.Rendering
{
    public class RenderProgram : BaseRenderState
    {
        private Shader[] _shaders;

        public RenderProgram(params Shader[] shaders) : base(EObjectType.Program)
        {
            _shaders = shaders;
        }
        protected override int CreateObject()
        {
            int[] ids = _shaders.Where(x => x != null).Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids, Engine.Settings.AllowShaderPipelines);
        }

        public void Use()
        {
            Engine.Renderer.UseProgram(BindingId);
        }
    }
}
