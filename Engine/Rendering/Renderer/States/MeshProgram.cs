using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using System.Linq;
using System;

namespace TheraEngine.Rendering
{
    public class MeshProgram : BaseRenderState
    {
        protected Shader 
            _vertexShader, 
            _fragmentShader, 
            _geometryShader,
            _tControlShader,
            _tEvalShader;

        protected GLVar[] _parameters;
        protected Texture[] _textures;
        protected Shader[] _shaders;
        protected PrimitiveBufferInfo _info;

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

        public virtual void SetMaterial(Material material)
        {
            _parameters = material.Parameters.ToArray();
            _textures = new Texture[material.Textures.Count];
            for (int i = 0; i < material.Textures.Count; ++i)
            {
                Texture t = material.Textures[i].GetTexture();
                t.Index = i;
                _textures[i] = t;
            }
            _fragmentShader = material._fragmentShader;
            _geometryShader = material._geometryShader;
            _tControlShader = material._tessellationControlShader;
            _tEvalShader = material._tessellationEvaluationShader;
            RemakeShaderArray();
        }
        protected void RemakeShaderArray()
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
        protected internal virtual void BindTextures()
        {
            for (int i = 0; i < Textures.Length; ++i)
            {
                Engine.Renderer.SetActiveTexture(i);
                Engine.Renderer.Uniform("Texture" + i, i);
                Textures[i].Bind();
            }
        }
    }
}
