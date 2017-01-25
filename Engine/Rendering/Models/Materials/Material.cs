using CustomEngine.Files;
using CustomEngine.Rendering.Models.Materials;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.Material; } }

        internal Shader
            _vertexShader,
            _fragmentShader, 
            _geometryShader,
            _tessellationControlShader, 
            _tessellationEvaluationShader;
        
        private List<IRenderable> _renderingReferences = new List<IRenderable>();
        private MaterialSettings _settings;
        private int _bindingId = -1;
        
        public int BindingId
        {
            get { return _bindingId; }
        }
        public MaterialSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        internal void AddReference(IRenderable user)
        {
            if (_renderingReferences.Count == 0)
                _bindingId = Engine.Renderer.AddActiveMaterial(this);
            _renderingReferences.Add(user);
        }
        internal void RemoveReference(IRenderable user)
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
            foreach (Shader s in shaders)
            {
                switch (s.ShaderType)
                {
                    case ShaderMode.Vertex:
                        _vertexShader = s;
                        break;
                    case ShaderMode.Fragment:
                        _fragmentShader = s;
                        break;
                    case ShaderMode.Geometry:
                        _geometryShader = s;
                        break;
                    case ShaderMode.TessControl:
                        _tessellationControlShader = s;
                        break;
                    case ShaderMode.TessEvaluation:
                        _tessellationEvaluationShader = s;
                        break;
                }
            }
        }

        public static Material GetDefaultMaterial()
        {
            Shader vert = Shader.TestVertexShader();
            Shader frag = Shader.TestFragmentShader();
            MaterialSettings settings = new MaterialSettings();
            settings.Parameters.Add(new GLVec4((ColorF4)Color.MediumVioletRed, "MatColor", null));
            settings.Parameters.Add(new GLFloat(1.0f, "MatSpecularIntensity", null));
            return new Material("TestMaterial", settings, vert, frag);
        }
    }
}
