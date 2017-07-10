using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using System.Linq;

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

        private RenderingParameters _renderParams;
        protected GLVar[] _parameters;
        protected Texture2D[] _textures;
        protected Shader[] _shaders;
        protected VertexShaderDesc _info;

        public GLVar[] Parameters => _parameters;
        public Texture2D[] Textures => _textures;

        public RenderingParameters RenderParams
        {
            get => _renderParams;
            set => _renderParams = value;
        }

        public MeshProgram(Material material, VertexShaderDesc info) : base(EObjectType.Program)
        {
            if (material == null)
                return;

            _renderParams = material._renderParams;
            MakeVertexShader(info);
            SetMaterial(material);
        }

        public void MakeVertexShader(VertexShaderDesc info)
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
        public virtual void SetUniforms()
        {
            foreach (GLVar v in _parameters)
                v.SetUniform();

            AbstractRenderer.CurrentCamera.SetUniforms();
            if (Engine.Settings.ShadingStyle == ShadingStyle.Forward)
                Engine.Scene.Lights.SetUniforms();

            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.RenderDelta), Engine.RenderDelta);
        }

        public virtual void SetMaterial(Material material)
        {
            _renderParams = material._renderParams;
            _parameters = material.Parameters.ToArray();
            _textures = new Texture2D[material.Textures.Count];
            for (int i = 0; i < material.Textures.Count; ++i)
            {
                Texture2D t = material.Textures[i].GetTexture();
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
