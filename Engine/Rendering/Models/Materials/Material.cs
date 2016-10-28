using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material : BaseRenderState
    {
        private List<MaterialProperty> _properties = new List<MaterialProperty>();
        private bool _cullFront = false, _cullBack = true;
        public List<Shader> _shaders = new List<Shader>();
        private string _name;

        public bool CullFront { get { return _cullFront; } set { _cullFront = true; } }
        public bool CullBack { get { return _cullBack; } set { _cullBack = true; } }

        public Material(string name) : base(GenType.Program) { _name = name; }
        public Material(string name, params Shader[] shaders) : base(GenType.Program)
        {
            _name = name;
            AddShaders(shaders);
        }

        public void AddShaders(params Shader[] shaders) { _shaders.AddRange(shaders); }
        public void Compile() { Generate(); }
        protected override int CreateObject()
        {
            int[] ids = _shaders.Select(x => x.Compile()).ToArray();
            return Engine.Renderer.GenerateProgram(ids);
        }
        public void Generate(ResultBasicFunc resultFunction)
        {
            _shaders.Clear();
            if (resultFunction == null)
                return;

            //TODO: determine shader types needed
            foreach (GLVar arg in resultFunction.InputArguments)
            {

            }
            foreach (Shader s in _shaders)
                s.Generate(resultFunction);
        }
    }
}
