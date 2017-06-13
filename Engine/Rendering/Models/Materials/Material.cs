using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.Models.Materials
{
    public class AlphaTest
    {
        private bool _enableAlphaTest;
        private bool _useConstantAlpha;
        private float _constantAlphaValue;
        private bool _useAlphaToCoverage;


        public bool EnableAlphaTest { get => _enableAlphaTest; set => _enableAlphaTest = value; }
    }
    public class StencilFace
    {
        private EComparison _func;
        private int _ref;
        private int _mask;

        public EComparison Func { get => _func; set => _func = value; }
        public int Ref { get => _ref; set => _ref = value; }
        public int Mask { get => _mask; set => _mask = value; }
    }
    public class StencilTest
    {
        private bool _enableStencilFunc;
        private StencilFace _frontFace, _backFace;

        public bool EnableStencilFunc { get => _enableStencilFunc; set => _enableStencilFunc = value; }
        public StencilFace FrontFace { get => _frontFace; set => _frontFace = value; }
        public StencilFace BackFace { get => _backFace; set => _backFace = value; }
    }
    public class DepthTest
    {
        private bool _enableDepthTest;
        private bool _depthMask;
        private EComparison _depthFunction;

        public bool EnableDepthTest { get => _enableDepthTest; set => _enableDepthTest = value; }
        public bool DepthMask { get => _depthMask; set => _depthMask = value; }
        public EComparison DepthFunction { get => _depthFunction; set => _depthFunction = value; }
    }
    public class Blend
    {
        private bool _enableBlending;

        public bool EnableBlending { get => _enableBlending; set => _enableBlending = value; }
    }
    public class Material : FileObject
    {
        private AlphaTest _alpha = new AlphaTest();
        private DepthTest _depth = new DepthTest();
        private StencilTest _stencil = new StencilTest();
        private Blend _blend = new Blend();
        private bool _writeRed, _writeGreen, _writeBlue, _writeAlpha;

        [Category("Depth Test")]
        [DisplayName("Enable")]
        public bool EnableDepthTest { get => _depth.EnableDepthTest; set => _depth.EnableDepthTest = value; }
        [Category("Depth Test")]
        [DisplayName("Enable Depth Write")]
        public bool DepthMask { get => _depth.DepthMask; set => _depth.DepthMask = value; }
        [Category("Depth Test")]
        [DisplayName("Depth Function")]
        public EComparison DepthFunction { get => _depth.DepthFunction; set => DepthFunction = value; }

        [Category("Stencil Test")]
        [DisplayName("Enable")]
        public bool EnableStencilFunc { get => _stencil.EnableStencilFunc; set => _stencil.EnableStencilFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Func")]
        public EComparison FrontFaceStencilFunc { get => _stencil.FrontFace.Func; set => _stencil.FrontFace.Func = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Ref")]
        public int FrontFaceStencilRef { get => _stencil.FrontFace.Ref; set => _stencil.FrontFace.Ref = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Mask")]
        public int FrontFaceStencilMask { get => _stencil.FrontFace.Mask; set => _stencil.FrontFace.Mask = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Func")]
        public EComparison BackFaceStencilFunc { get => _stencil.BackFace.Func; set => _stencil.BackFace.Func = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Ref")]
        public int BackFaceStencilRef { get => _stencil.BackFace.Ref; set => _stencil.BackFace.Ref = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Mask")]
        public int BackFaceStencilMask { get => _stencil.BackFace.Mask; set => _stencil.BackFace.Mask = value; }
        
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
        private bool _hasTransparency = false;

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

        public bool HasTransparency => _blend.EnableBlending || _alpha.EnableAlphaTest;

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

        public Material()
            : this("NewMaterial", new List<GLVar>(), new List<TextureReference>()) { }
        public Material(string name, params Shader[] shaders) 
            : this(name, new List<GLVar>(), new List<TextureReference>(), shaders) { }
        public Material(string name, List<GLVar> parameters, params Shader[] shaders)
            : this(name, parameters, new List<TextureReference>(), shaders) { }
        public Material(string name, List<TextureReference> textures, params Shader[] shaders)
            : this(name, new List<GLVar>(), textures, shaders) { }
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

        public static Material GetUnlitTextureMaterial() => GetUnlitTextureMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitTextureMaterial(bool deferred)
        {
            Shader frag = deferred ? Shader.UnlitTextureFragDeferred() : Shader.UnlitTextureFragForward();
            return new Material("UnlitTextureMaterial", frag);
        }
        public static Material GetLitTextureMaterial() => GetLitTextureMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitTextureMaterial(bool deferred)
        {
            Shader frag = deferred ? Shader.LitTextureFragDeferred() : Shader.LitTextureFragForward();
            return new Material("LitTextureMaterial", frag);
        }

        public static Material GetUnlitColorMaterial()
            => GetUnlitColorMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitColorMaterial(bool deferred)
            => GetUnlitColorMaterial(Color.DarkTurquoise, deferred);
        public static Material GetUnlitColorMaterial(Color color)
            => GetUnlitColorMaterial(color, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitColorMaterial(Color color, bool deferred)
        {
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = deferred ? Shader.UnlitColorFragDeferred() : Shader.UnlitColorFragForward();
            List<GLVar> parameters = new List<GLVar>()
            {
                new GLVec4((ColorF4)color, "MatColor"),
            };
            return new Material("UnlitColorMaterial", parameters, refs, frag);
        }
        public static Material GetLitColorMaterial()
            => GetLitColorMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(bool deferred)
            => GetLitColorMaterial(Color.DarkTurquoise, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(Color color)
            => GetLitColorMaterial(color, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(Color color, bool deferred)
        {
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = deferred ? Shader.LitColorFragDeferred() : Shader.LitColorFragForward();
            List<GLVar> parameters = new List<GLVar>()
            {
                new GLVec4((ColorF4)color, "MatColor"),
                new GLFloat(20.0f, "MatSpecularIntensity"),
                new GLFloat(128.0f, "MatShininess"),
            };
            return new Material("TestMaterial", parameters, refs, frag);
        }
    }
}
