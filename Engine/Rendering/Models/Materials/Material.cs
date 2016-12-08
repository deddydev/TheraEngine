using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material
    {
        private Shader _fragmentShader, _tessellationControlShader, _tessellationEvaluationShader;
        private List<RenderableObject> _renderingReferences = new List<RenderableObject>();
        private MaterialSettings _settings;

        [Category("Material")]
        public MaterialSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        internal void AddReference(RenderableObject user) { _renderingReferences.Add(user); }
        internal void RemoveReference(RenderableObject user) { _renderingReferences.Add(user); }

        public Material(string name, MaterialSettings settings)
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
    }
}
