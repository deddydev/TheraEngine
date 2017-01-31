using CustomEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MaterialInstance
    {
        private Shader _vertexShader, _fragmentShader, _geometryShader, _tControlShader, _tEvalShader;
        private GLVar[] _parameters;
        private Texture[] _textures;

        public Texture[] Textures { get { return _textures; } }
        public Shader VertexShader { get { return _vertexShader; } }
        public Shader FragmentShader { get { return _fragmentShader; } }
        public Shader GeometryShader { get { return _geometryShader; } }
        public Shader TessellationControlShader { get { return _tControlShader; } }
        public Shader TessellationEvaluationShader { get { return _tEvalShader; } }

        public MaterialInstance(Material material, PrimitiveBufferInfo info)
        {
            if (material == null)
                return;
            
            _parameters = material.Parameters.ToArray();
            _textures = material.Textures.Select(x => x.GetTexture()).ToArray();
            _vertexShader = VertexShaderGenerator.Generate(info, false, false, false);
            _fragmentShader = material._fragmentShader;
            _geometryShader = material._geometryShader;
            _tControlShader = material._tessellationControlShader;
            _tEvalShader = material._tessellationEvaluationShader;
        }
        public void SetUniforms()
        {
            foreach (GLVar v in _parameters)
                v.SetUniform();
        }
    }
}
