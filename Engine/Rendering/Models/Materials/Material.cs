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
        public int _programId = 0;
        public List<Shader> _shaders = new List<Shader>();

        public bool CullFront { get { return _cullFront; } set { _cullFront = true; } }
        public bool CullBack { get { return _cullBack; } set { _cullBack = true; } }

        public Material() { }
        public Material(params Shader[] shaders) { AddShaders(shaders); }

        public void AddShaders(params Shader[] shaders) { _shaders.AddRange(shaders); }
        public void Compile()
        {
            if (_programId > 0)
                Engine.Renderer.DeleteProgram(_programId);
            int[] ids = _shaders.Select(x => x.Compile()).ToArray();
            _programId = Engine.Renderer.GenerateProgram(ids);
        }
        public void Generate(ResultBasicFunc resultFunction)
        {
            _shaders.Clear();
            //TODO: determine shader types needed
            foreach (Shader s in _shaders)
                s.Generate(resultFunction);
        }
    }
}
