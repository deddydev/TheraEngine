using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System.Linq;

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

        public GLVar[] Parameters => _parameters;
        public Texture[] Textures => _textures;
        public Shader VertexShader => _vertexShader;
        public Shader FragmentShader => _fragmentShader;
        public Shader GeometryShader => _geometryShader;
        public Shader TessellationControlShader => _tControlShader;
        public Shader TessellationEvaluationShader => _tEvalShader;

        public MeshProgram(Material material, PrimitiveBufferInfo info) : base(GenType.Program)
        {
            if (material == null)
                return;

            MakeVertexShader(info);
            SetMaterial(material);
        }

        public void MakeVertexShader(PrimitiveBufferInfo info)
        {
            _info = info;
            _vertexShader = VertexShaderGenerator.Generate(_info, false, false, false);
            RemakeShaderArray();
            if (IsActive)
            {
                Destroy();
                Generate();
            }
        }

        protected override int CreateObject()
        {
            int[] ids = _shaders.Where(x => x != null).Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids, _info);
        }
        protected override void OnGenerated()
        {

        }
        protected override void PostDeleted()
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
            RemakeShaderArray();
        }

        private void RemakeShaderArray()
        {
            _shaders = new Shader[]
            {
                _vertexShader,
                _fragmentShader,
                _geometryShader,
                _tControlShader,
                _tEvalShader
            };
        }
    }
}
