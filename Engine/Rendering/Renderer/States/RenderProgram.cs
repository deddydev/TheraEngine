using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using System.Linq;
using System.Collections.Generic;

namespace TheraEngine.Rendering
{
    public class RenderProgram : BaseRenderState
    {
        private List<Shader> _vertexShaders = new List<Shader>();
        private List<Shader> _geometryShaders = new List<Shader>();
        private List<Shader> _tessEvalShaders = new List<Shader>();
        private List<Shader> _tessCtrlShaders = new List<Shader>();
        private List<Shader> _fragmentShaders = new List<Shader>();
        private Shader[] _shaders;
        private VertexShaderDesc _vtxDesc = null;

        public RenderProgram(VertexShaderDesc d, params Shader[] shaders) : base(EObjectType.Program)
        {
            _vtxDesc = d;
            _shaders = shaders;
        }
        protected override int CreateObject()
        {
            int[] ids = _shaders.Where(x => x != null).Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids, _vtxDesc, true);
        }
    }
}
