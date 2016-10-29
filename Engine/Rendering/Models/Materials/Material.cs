using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material : BaseRenderState
    {
        public List<Shader> _shaders = new List<Shader>();
        private string _name;
        private MaterialSettings _settings;

        [Category("Material")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Category("Material")]
        public MaterialSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public Material(string name, MaterialSettings settings) : base(GenType.Program)
        {
            _name = name;
            _settings = settings;
        }
        public Material(string name, MaterialSettings settings, params Shader[] shaders) : base(GenType.Program)
        {
            _name = name;
            _settings = settings;
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
