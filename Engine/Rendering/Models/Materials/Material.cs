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
        private Shader
            _vertexShader,
            _fragmentShader,
            _geometryShader,
            _tessellationControlShader, 
            _tessellationEvaluationShader;

        private List<RenderableObject> _renderingReferences = new List<RenderableObject>();
        private MaterialSettings _settings;
        private int _bindingId = -1;
        private string _name;
        
        public int BindingId
        {
            get { return _bindingId; }
        }
        public MaterialSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        internal void AddReference(RenderableObject user)
        {
            if (_renderingReferences.Count == 0)
                _bindingId = Engine.Renderer.AddActiveMaterial(this);
            _renderingReferences.Add(user);
        }
        internal void RemoveReference(RenderableObject user)
        {
            _renderingReferences.Add(user);
            if (_renderingReferences.Count == 0)
            {
                Engine.Renderer.RemoveActiveMaterial(this);
                _bindingId = -1;
            }
        }
        public Material(string name, MaterialSettings settings, params Shader[] shaders)
        {
            _name = name;
            _settings = settings;
        }
    }
}
