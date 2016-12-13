using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class MeshProgram : BaseRenderState
    {
        public Shader[] _shaders;
        public MaterialInstance _material;

        public MeshProgram(Material material) : base(GenType.Program)
        {
            SetMaterial(material);
        }

        protected override int CreateObject()
        {
            int[] ids = _shaders.Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids);
        }
        protected override void OnGenerated()
        {

        }
        protected override void OnDeleted()
        {

        }
        public void SetMaterial(Material material)
        {
            _material = new MaterialInstance(material);
            SetShaders(
                _material.VertexShader,
                _material.FragmentShader,
                _material.GeometryShader,
                _material.TessellationControlShader,
                _material.TessellationEvaluationShader);
        }
        public void SetShaders(params Shader[] shaders) { _shaders = shaders.Where(x => x != null).ToArray(); }
    }
}
