using CustomEngine.Files;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material : FileObject
    {
        private Shader
            _vertexShader,
            _fragmentShader, 
            _geometryShader,
            _tessellationControlShader, 
            _tessellationEvaluationShader;

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
    }
}
