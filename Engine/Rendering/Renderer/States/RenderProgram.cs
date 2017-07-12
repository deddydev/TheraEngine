using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TheraEngine.Rendering
{
    public class RenderProgram : BaseRenderState
    {
        //    private List<Shader> _vertexShaders = new List<Shader>();
        //    private List<Shader> _geometryShaders = new List<Shader>();
        //    private List<Shader> _tessEvalShaders = new List<Shader>();
        //    private List<Shader> _tessCtrlShaders = new List<Shader>();
        //    private List<Shader> _fragmentShaders = new List<Shader>();
        private Shader[] _shaders;
        private VertexShaderDesc _desc;

        public RenderProgram(VertexShaderDesc desc, params Shader[] shaders) : base(EObjectType.Program)
        {
            _desc = desc;
            _shaders = shaders;
        }
        protected override int CreateObject()
        {
            int[] ids = _shaders.Where(x => x != null).Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids, _desc, Engine.Settings.AllowShaderPipelines);
        }

        public void Use()
        {
            Engine.Renderer.UseProgram(BindingId);
        }
    }
}
