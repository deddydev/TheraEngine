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

        public int BindingId => _bindingId;
        public List<GLVar> Parameters
        {
            get => _parameters;
            set => _parameters = value;
        }
        public List<TextureReference> Textures
        {
            get => _textures;
            set => _textures = value;
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

        public static Material GetBasicTextureMaterial(TextureReference texture) => GetBasicTextureMaterial(texture, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetBasicTextureMaterial(TextureReference texture, bool deferred)
        {
            List<TextureReference> refs = new List<TextureReference>() { texture };
            Shader frag = deferred ? Shader.UnlitTextureFragDeferred() : Shader.UnlitTextureFragForward();
            List<GLVar> parameters = new List<GLVar>();
            return new Material("UnlitTextureMaterial", parameters, refs, frag);
        }

        public static Material GetUnlitColorMaterial() => GetUnlitColorMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitColorMaterial(bool deferred)
        {
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = deferred ? Shader.UnlitColorFragDeferred() : Shader.UnlitColorFragForward();
            List<GLVar> parameters = new List<GLVar>()
            {
                new GLVec4((ColorF4)Color.Green, "MatColor"),
            };
            return new Material("UnlitColorMaterial", parameters, refs, frag);
        }

        public static Material GetDefaultMaterial() => GetDefaultMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetDefaultMaterial(bool deferred)
        {
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = deferred ? Shader.TestFragDeferred() : Shader.TestFragForward();
            List<GLVar> parameters = new List<GLVar>()
            {
                new GLVec4((ColorF4)Color.DarkTurquoise, "MatColor"),
                new GLFloat(20.0f, "MatSpecularIntensity"),
                new GLFloat(128.0f, "MatShininess"),
            };
            return new Material("TestMaterial", parameters, refs, frag);
        }

        internal static Material GetGBufferMaterial(int width, int height)
        {
            //These are listed in order of appearance in the shader
            List<TextureReference> refs = new List<TextureReference>()
            {
                new TextureReference("DepthStencil", width, height,
                    EPixelInternalFormat.Depth24Stencil8, EPixelFormat.DepthStencil, EPixelType.UnsignedInt248),
                new TextureReference("AlbedoSpec", width, height, 
                    EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte),
                new TextureReference("Position", width, height,
                    EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float),
                new TextureReference("Normal", width, height, 
                    EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float),
                new TextureReference("Text", width, height, 
                    EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte, System.Drawing.Imaging.PixelFormat.Format32bppArgb),
            };
            Shader frag = Shader.GBufferShader();
            List<GLVar> parameters = new List<GLVar>()
            {
                //TODO: post process parameters here
                //PostProcessSettings
                new GLVec4((ColorF4)Color.Transparent, "Vignette.Color"),
                new GLFloat(0.0f, "DOF.NearDistance"),
                new GLFloat(0.0f, "DOF.FarDistance"),
            };
            return new Material("GBufferMaterial", parameters, refs, frag);
        }
        protected override int OnCalculateSize(StringTable table)
        {
            return 0;
        }
        public override void Write(VoidPtr address, StringTable table)
        {

        }
        public override void Read(VoidPtr address, VoidPtr strings)
        {

        }
        public override void Write(XmlWriter writer)
        {

        }
        public override void Read(XMLReader reader)
        {

        }
    }
}
