using OpenTK.Graphics.OpenGL;
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
        public int MaterialId
        {
            get
            {
                if (BindingId <= 0)
                    Generate();
                return BindingId;
            }
        }
        public override int GetHashCode() { return MaterialId; }

        public Shader[] _shaders;
        private MaterialSettings _settings;

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
            SetShaders(shaders);
        }

        public void SetShaders(params Shader[] shaders) { _shaders = shaders; }
        public void Compile() { Generate(); }
        protected override int CreateObject()
        {
            int[] ids = _shaders.Select(x => x.Compile()).ToArray();
            int id = Engine.Renderer.GenerateProgram(ids);
            return id;
        }

        public void SetUniforms()
        {

        }
    }
}
