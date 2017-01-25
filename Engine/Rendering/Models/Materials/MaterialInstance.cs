using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        private Shader _modifiedVertexShader;
        private Material _material;
        private List<GLVar> _parameters = new List<GLVar>();
        
        public Shader VertexShader { get { return _modifiedVertexShader; } }
        public Shader FragmentShader { get { return _material._fragmentShader; } }
        public Shader GeometryShader { get { return _material._geometryShader; } }
        public Shader TessellationControlShader { get { return _material._tessellationControlShader; } }
        public Shader TessellationEvaluationShader { get { return _material._tessellationEvaluationShader; } }

        public MaterialInstance(Material material, int boneCount)
        {
            _material = material;
            if (_material != null)
            {
                _parameters.AddRange(_material.Settings.Parameters);
                if (boneCount > 0)
                {
                    //TODO: incorporate skinning and morphing into material's vertex shader
                    string s = material._vertexShader._source;
                    string[] parts = new string[2];
                    int first = s.IndexOf('{');

                    parts[0] = s.Substring(0, first);
                    parts[1] = s.Substring(first);
                    _modifiedVertexShader = new Shader(ShaderMode.Vertex, s);
                }
                else
                    _modifiedVertexShader = _material._vertexShader;
            }
        }
        public void SetUniforms()
        {
            _parameters.ForEach(x => x.SetUniform());
        }
    }
}
