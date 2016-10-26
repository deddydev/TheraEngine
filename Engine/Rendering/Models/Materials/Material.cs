using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material
    {
        private List<MaterialProperty> _properties = new List<MaterialProperty>();
        private bool _cullFront = false, _cullBack = true;
        public Shader _vertexShader, _fragmentShader, _geometryShader;
        
        public bool CullFront { get { return _cullFront; } set { _cullFront = true; } }
        public bool CullBack { get { return _cullBack; } set { _cullBack = true; } }
        public Shader FragmentShader { get { return _fragmentShader; } set { _fragmentShader = value; } }
        public Shader GeometryShader { get { return _geometryShader; } set { _geometryShader = value; } }
        public Shader VertexShader { get { return _vertexShader; } set { _vertexShader = value; } }
        
        public void Compile(ResultBasicFunc resultFunction)
        {
            _vertexShader.Compile(resultFunction);
            _fragmentShader.Compile(resultFunction);
        }
    }
}
