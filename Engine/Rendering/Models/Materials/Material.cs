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
        private List<RenderableObject> _renderingReferences = new List<RenderableObject>();

        internal void AddReference(RenderableObject user) { _renderingReferences.Add(user); }
        internal void RemoveReference(RenderableObject user) { _renderingReferences.Add(user); }
        
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
            int id = Engine.Renderer.GenerateMaterial(ids);
            return id;
        }
        protected override void OnGenerated()
        {
            Engine.Renderer.AddActiveMaterial(this);
        }
        protected override void OnDeleted()
        {
            Engine.Renderer.RemoveActiveMaterial(this);
        }

        public void SetUniforms()
        {

        }
    }
}
