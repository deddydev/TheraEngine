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
using System.IO;
using System.Xml;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Material : FileObject
    {
        public override ResourceType ResourceType => ResourceType.Material;

        internal Shader
            _vertexShader,
            _fragmentShader, 
            _geometryShader,
            _tessellationControlShader, 
            _tessellationEvaluationShader;

        private List<TextureReference> _textures = new List<TextureReference>();
        private List<GLVar> _parameters = new List<GLVar>();
        private List<IRenderable> _renderingReferences = new List<IRenderable>();
        private int _bindingId = -1;

        public int BindingId
        {
            get { return _bindingId; }
        }
        public List<GLVar> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
        public List<TextureReference> Textures
        {
            get { return _textures; }
            set { _textures = value; }
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
        public Material(string name, List<GLVar> parameters, List<TextureReference> textures, params Shader[] shaders)
        {
            _name = name;
            _parameters = parameters;
            _textures = textures;
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
        public static Material GetBasicTextureMaterial(TextureReference texture)
        {
            List<TextureReference> refs = new List<TextureReference>() { texture };
            Shader frag = Shader.UnlitTextureFrag();
            List<GLVar> p = new List<GLVar>();
            return new Material("UnlitTextureMaterial", p, refs, frag);
        }
        public static Material GetUnlitColorMaterial()
        {
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = Shader.UnlitColorFrag();
            List<GLVar> p = new List<GLVar>()
            {
                new GLVec4((ColorF4)Color.Green, "MatColor"),
            };
            return new Material("UnlitColorMaterial", p, refs, frag);
        }

        public static Material GetDefaultMaterial()
        {
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = Shader.TestFrag();
            List<GLVar> p = new List<GLVar>()
            {
                new GLVec4((ColorF4)Color.DarkTurquoise, "MatColor"),
                new GLFloat(20.0f, "MatSpecularIntensity"),
                new GLFloat(128.0f, "MatShininess"),
            };
            return new Material("TestMaterial", p, refs, frag);
        }
        internal static Material GetGBufferMaterial()
        {
            List<TextureReference> refs = new List<TextureReference>()
            {
                new TextureReference("Position"),
                new TextureReference("Normal"),
                new TextureReference("AlbedoSpec"),
                new TextureReference("TexCoord"),
            };
            Shader frag = Shader.UnlitColorFrag();
            List<GLVar> p = new List<GLVar>()
            {
                //TODO: post process parameters here
                //PostProcessSettings
                new GLVec4((ColorF4)Color.Transparent, "Vignette.Color"),
                new GLFloat(0.0f, "DOF.NearDistance"),
                new GLFloat(0.0f, "DOF.FarDistance"),
            };
            return new Material("GBufferMaterial", p, refs, frag);
        }
        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
