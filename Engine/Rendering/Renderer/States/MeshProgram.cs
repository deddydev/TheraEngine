using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class MeshProgram : BaseRenderState
    {
        private Shader 
            _vertexShader, 
            _fragmentShader, 
            _geometryShader,
            _tControlShader,
            _tEvalShader;

        private GLVar[] _parameters;
        private Texture[] _textures;
        private Shader[] _shaders;
        private PrimitiveBufferInfo _info;

        public Texture[] Textures { get { return _textures; } }
        public Shader VertexShader { get { return _vertexShader; } }
        public Shader FragmentShader { get { return _fragmentShader; } }
        public Shader GeometryShader { get { return _geometryShader; } }
        public Shader TessellationControlShader { get { return _tControlShader; } }
        public Shader TessellationEvaluationShader { get { return _tEvalShader; } }

        public MeshProgram(Material material, PrimitiveBufferInfo info) : base(GenType.Program)
        {
            if (material == null)
                return;

            _info = info;
            _vertexShader = VertexShaderGenerator.Generate(info, false, false, false);
            SetMaterial(material);
        }

        protected override int CreateObject()
        {
            int[] ids = _shaders.Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids, _info);
        }
        protected override void OnGenerated()
        {

        }
        protected override void OnDeleted()
        {

        }
        public void SetUniforms()
        {
            foreach (GLVar v in _parameters)
                v.SetUniform();
        }

        public void SetMaterial(Material material)
        {
            _parameters = material.Parameters.ToArray();
            _textures = material.Textures.Select(x => x.GetTexture()).ToArray();
            _fragmentShader = material._fragmentShader;
            _geometryShader = material._geometryShader;
            _tControlShader = material._tessellationControlShader;
            _tEvalShader = material._tessellationEvaluationShader;
            _shaders = new Shader[]
            {
                _vertexShader,
                _fragmentShader,
                _geometryShader,
                _tControlShader,
                _tEvalShader
            }.Where(x => x != null).ToArray();
        }
    }
}
