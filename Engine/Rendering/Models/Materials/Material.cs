using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Files;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum LogicGate
    {
        And,
        Nand,
        Or,
        Nor,
        Xor,
        Xnor,
    }
    public struct AlphaTest
    {
        private bool _enableAlphaTest;
        private bool _useConstantAlpha;
        private float _constantAlphaValue;
        private bool _useAlphaToCoverage;
        private float _ref0, _ref1;
        private EComparison _comp0, _comp1;
        private LogicGate _logicGate;

        public AlphaTest(
            bool enabled,
            float ref0, float ref1,
            EComparison comp0, EComparison comp1,
            LogicGate logicGate,
            bool constantAlpha, float constantAlphaValue, 
            bool useAlphaToCoverage)
        {
            _enableAlphaTest = enabled;
            _useConstantAlpha = constantAlpha;
            _constantAlphaValue = constantAlphaValue;
            _useAlphaToCoverage = useAlphaToCoverage;
            _ref0 = ref0;
            _ref1 = ref1;
            _logicGate = logicGate;
            _comp0 = comp0;
            _comp1 = comp1;
        }
        
        public bool EnableAlphaTest { get => _enableAlphaTest; set => _enableAlphaTest = value; }
        public bool UseConstantAlpha { get => _useConstantAlpha; set => _useConstantAlpha = value; }
        public float ConstantAlphaValue { get => _constantAlphaValue; set => _constantAlphaValue = value; }
        public bool UseAlphaToCoverage { get => _useAlphaToCoverage; set => _useAlphaToCoverage = value; }
    }
    public struct StencilFace
    {
        private EComparison _func;
        private int _ref;
        private int _mask;

        public EComparison Func { get => _func; set => _func = value; }
        public int Ref { get => _ref; set => _ref = value; }
        public int Mask { get => _mask; set => _mask = value; }
    }
    public struct StencilTest
    {
        private bool _enableStencilFunc;
        private StencilFace _frontFace, _backFace;

        public bool EnableStencilFunc { get => _enableStencilFunc; set => _enableStencilFunc = value; }

        public EComparison FrontFaceFunc { get => _frontFace.Func; set => _frontFace.Func = value; }
        public EComparison BackFaceFunc { get => _backFace.Func; set => _backFace.Func = value; }
        public int FrontFaceRef { get => _frontFace.Ref; set => _frontFace.Ref = value; }
        public int BackFaceRef { get => _backFace.Ref; set => _backFace.Ref = value; }
        public int FrontFaceMask { get => _frontFace.Mask; set => _frontFace.Mask = value; }
        public int BackFaceMask { get => _backFace.Mask; set => _backFace.Mask = value; }

        public StencilFace FrontFace { get => _frontFace; set => _frontFace = value; }
        public StencilFace BackFace { get => _backFace; set => _backFace = value; }
    }
    public struct DepthTest
    {
        private bool _enableDepthTest;
        private bool _enableDepthUpdate;
        private EComparison _depthFunction;

        public bool EnableDepthTest { get => _enableDepthTest; set => _enableDepthTest = value; }
        public bool EnableDepthUpdate { get => _enableDepthUpdate; set => _enableDepthUpdate = value; }
        public EComparison DepthFunction { get => _depthFunction; set => _depthFunction = value; }
    }
    public struct Blend
    {
        private bool _enableBlending;
        private EBlendEquationMode _rgbEquation, _alphaEquation;
        private EBlendingFactor _rgbSrcFactor, _alphaSrcFactor, _rgbDstFactor, _alphaDstFactor;
        
        public bool EnableBlending { get => _enableBlending; set => _enableBlending = value; }
        public EBlendEquationMode RgbEquation { get => _rgbEquation; set => _rgbEquation = value; }
        public EBlendEquationMode AlphaEquation { get => _alphaEquation; set => _alphaEquation = value; }
        public EBlendingFactor RgbSrcFactor { get => _rgbSrcFactor; set => _rgbSrcFactor = value; }
        public EBlendingFactor AlphaSrcFactor { get => _alphaSrcFactor; set => _alphaSrcFactor = value; }
        public EBlendingFactor RgbDstFactor { get => _rgbDstFactor; set => _rgbDstFactor = value; }
        public EBlendingFactor AlphaDstFactor { get => _alphaDstFactor; set => _alphaDstFactor = value; }
    }
    public enum MaterialRenderType
    {
        Deferred,
        Forward,
        ForwardOnTop,
    }
    public struct RenderingParameters
    {
        public static readonly RenderingParameters Default = new RenderingParameters() { };

        private MaterialRenderType renderType;
        private AlphaTest _alpha;
        private DepthTest _depth;
        private StencilTest _stencil;
        private Blend _blend;
        private bool _writeAlpha;
        private bool _writeRed;
        private bool _writeGreen;
        private bool _writeBlue;

        [Category("Blending")]
        [DisplayName("Enable")]
        public bool EnableBlending { get => _blend.EnableBlending; set => _blend.EnableBlending = value; }
        [Category("Blending")]
        [DisplayName("RGB Equation")]
        public EBlendEquationMode RgbEquation { get => _blend.RgbEquation; set => _blend.RgbEquation = value; }
        [Category("Blending")]
        [DisplayName("Alpha Equation")]
        public EBlendEquationMode AlphaEquation { get => _blend.AlphaEquation; set => _blend.AlphaEquation = value; }
        [Category("Blending")]
        [DisplayName("RGB Source Factor")]
        public EBlendingFactor RgbSrcFactor { get => _blend.RgbSrcFactor; set => _blend.RgbSrcFactor = value; }
        [Category("Blending")]
        [DisplayName("Alpha Source Factor")]
        public EBlendingFactor AlphaSrcFactor { get => _blend.AlphaSrcFactor; set => _blend.AlphaSrcFactor = value; }

        [Category("Depth Test")]
        [DisplayName("Enable")]
        public bool EnableDepthTest { get => _depth.EnableDepthTest; set => _depth.EnableDepthTest = value; }
        [Category("Depth Test")]
        [DisplayName("Enable Depth Write")]
        public bool EnableDepthUpdate { get => _depth.EnableDepthUpdate; set => _depth.EnableDepthUpdate = value; }
        [Category("Depth Test")]
        [DisplayName("Depth Function")]
        public EComparison DepthFunction { get => _depth.DepthFunction; set => _depth.DepthFunction = value; }

        [Category("Stencil Test")]
        [DisplayName("Enable")]
        public bool EnableStencilFunc { get => _stencil.EnableStencilFunc; set => _stencil.EnableStencilFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Func")]
        public EComparison FrontFaceStencilFunc { get => _stencil.FrontFaceFunc; set => _stencil.FrontFaceFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Ref")]
        public int FrontFaceStencilRef { get => _stencil.FrontFaceRef; set => _stencil.FrontFaceRef = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Mask")]
        public int FrontFaceStencilMask { get => _stencil.FrontFaceMask; set => _stencil.FrontFaceMask = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Func")]
        public EComparison BackFaceStencilFunc { get => _stencil.BackFaceFunc; set => _stencil.BackFaceFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Ref")]
        public int BackFaceStencilRef { get => _stencil.BackFaceRef; set => _stencil.BackFaceRef = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Mask")]
        public int BackFaceStencilMask { get => _stencil.BackFaceMask; set => _stencil.BackFaceMask = value; }

        public MaterialRenderType RenderType { get => renderType; set => renderType = value; }
        public AlphaTest Alpha { get => _alpha; set => _alpha = value; }
        public DepthTest Depth { get => _depth; set => _depth = value; }
        public StencilTest Stencil { get => _stencil; set => _stencil = value; }
        public Blend Blend { get => _blend; set => _blend = value; }
        public bool WriteRed { get => _writeRed; set => _writeRed = value; }
        public bool WriteGreen { get => _writeGreen; set => _writeGreen = value; }
        public bool WriteBlue { get => _writeBlue; set => _writeBlue = value; }
        public bool WriteAlpha { get => _writeAlpha; set => _writeAlpha = value; }
    }
    public class Material : FileObject
    {
        internal RenderingParameters _renderParams = new RenderingParameters()
        {
            EnableDepthTest = true,
            EnableDepthUpdate = true,
            DepthFunction = EComparison.Lequal,
        };

        [Category("Depth Test")]
        [DisplayName("Enable")]
        public bool EnableDepthTest { get => _renderParams.EnableDepthTest; set => _renderParams.EnableDepthTest = value; }
        [Category("Depth Test")]
        [DisplayName("Enable Depth Write")]
        public bool EnableDepthUpdate { get => _renderParams.EnableDepthUpdate; set => _renderParams.EnableDepthUpdate = value; }
        [Category("Depth Test")]
        [DisplayName("Depth Function")]
        public EComparison DepthFunction { get => _renderParams.DepthFunction; set => _renderParams.DepthFunction = value; }

        [Category("Stencil Test")]
        [DisplayName("Enable")]
        public bool EnableStencilFunc { get => _renderParams.EnableStencilFunc; set => _renderParams.EnableStencilFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Func")]
        public EComparison FrontFaceStencilFunc { get => _renderParams.FrontFaceStencilFunc; set => _renderParams.FrontFaceStencilFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Ref")]
        public int FrontFaceStencilRef { get => _renderParams.FrontFaceStencilRef; set => _renderParams.FrontFaceStencilRef = value; }
        [Category("Stencil Test")]
        [DisplayName("Front Face Mask")]
        public int FrontFaceStencilMask { get => _renderParams.FrontFaceStencilMask; set => _renderParams.FrontFaceStencilMask = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Func")]
        public EComparison BackFaceStencilFunc { get => _renderParams.BackFaceStencilFunc; set => _renderParams.BackFaceStencilFunc = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Ref")]
        public int BackFaceStencilRef { get => _renderParams.BackFaceStencilRef; set => _renderParams.BackFaceStencilRef = value; }
        [Category("Stencil Test")]
        [DisplayName("Back Face Mask")]
        public int BackFaceStencilMask { get => _renderParams.BackFaceStencilMask; set => _renderParams.BackFaceStencilMask = value; }
        
        public ShaderVar[] Parameters => _parameters;
        public TextureReference[] TexRefs
        {
            get => _textures;
            set
            {
                _textures = value;
                CollectFBOAttachments();
            }
        }
        
        internal void CollectFBOAttachments()
        {
            if (_frameBuffer != null && _textures != null && _textures.Length > 0)
            {
                List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
                foreach (TextureReference tref in _textures)
                {
                    if (!tref.FrameBufferAttachment.HasValue)
                        continue;
                    switch (tref.FrameBufferAttachment.Value)
                    {
                        case EFramebufferAttachment.Color:
                        case EFramebufferAttachment.Depth:
                        case EFramebufferAttachment.DepthAttachment:
                        case EFramebufferAttachment.DepthStencilAttachment:
                        case EFramebufferAttachment.Stencil:
                        case EFramebufferAttachment.StencilAttachment:
                            continue;
                    }
                    fboAttachments.Add((EDrawBuffersAttachment)(int)tref.FrameBufferAttachment.Value);
                }
                _fboAttachments = fboAttachments.ToArray();
            }
            else
                _fboAttachments = null;
        }

        public event Action SettingUniforms;

        private List<Shader> _geometryShaders;
        private List<Shader> _tessEvalShaders;
        private List<Shader> _tessCtrlShaders;
        private List<Shader> _fragmentShaders;
        private Shader[] _shaders;
        private EDrawBuffersAttachment[] _fboAttachments;

        private RenderProgram _program;
        private FrameBuffer _frameBuffer;

        protected ShaderVar[] _parameters;
        protected TextureReference[] _textures;

        private List<PrimitiveManager> _references = new List<PrimitiveManager>();
        private int _uniqueID = -1;

        public int UniqueID => _uniqueID;

        public bool HasTransparency => _renderParams.Blend.EnableBlending || _renderParams.Alpha.EnableAlphaTest;

        public RenderProgram Program
        {
            get
            {
                if (_program != null && !_program.IsActive)
                    _program.Generate();
                return _program;
            }
        }

        public FrameBuffer FrameBuffer
        {
            get => _frameBuffer;
            set
            {
                _frameBuffer = value;
                BindTextures = _frameBuffer != null ? (Action)BindTexturesFBO : BindTexturesNonFBO;
                CollectFBOAttachments();
            }
        }

        public void GenerateTextures()
        {
            if (_textures != null)
                foreach (TextureReference t in _textures)
                    t.Texture.Generate();
        }
        internal void AddReference(PrimitiveManager user)
        {
            if (_references.Count == 0)
                _uniqueID = Engine.Scene.AddActiveMaterial(this);
            _references.Add(user);
        }
        internal void RemoveReference(PrimitiveManager user)
        {
            _references.Add(user);
            if (_references.Count == 0)
            {
                Engine.Scene.RemoveActiveMaterial(this);
                _uniqueID = -1;
            }
        }
        public void SetUniforms()
        {
            foreach (ShaderVar v in _parameters)
                v.SetProgramUniform(Program.BindingId);

            Engine.Renderer.ApplyRenderParams(_renderParams);
            BindTextures();

            SettingUniforms?.Invoke();
        }
        /// <summary>
        /// Resizes the gbuffer's textures.
        /// Note that they will still fully cover the screen regardless of 
        /// if their dimensions match or not.
        /// </summary>
        public void ResizeTextures(int width, int height)
        {
            //Update each texture's dimensions
            foreach (TextureReference t in TexRefs)
                t.Resize(width, height);
        }
        public Action BindTextures;
        private void BindTexturesFBO()
        {
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, FrameBuffer.BindingId);
            BindTexturesNonFBO();
            Engine.Renderer.SetDrawBuffers(_fboAttachments);
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
        }
        internal void BindTexturesNonFBO()
        {
            for (int i = 0; i < TexRefs.Length; ++i)
            {
                Engine.Renderer.SetActiveTexture(i);
                Engine.Renderer.ProgramUniform(_program.BindingId, "Texture" + i, i);
                TexRefs[i].Texture.Bind();
            }
        }

        public Material()
            : this("NewMaterial", new ShaderVar[0], new TextureReference[0]) { }
        public Material(string name, params Shader[] shaders) 
            : this(name, new ShaderVar[0], new TextureReference[0], shaders) { }
        public Material(string name, ShaderVar[] parameters, params Shader[] shaders)
            : this(name, parameters, new TextureReference[0], shaders) { }
        public Material(string name, TextureReference[] textures, params Shader[] shaders)
            : this(name, new ShaderVar[0], textures, shaders) { }
        public Material(string name, ShaderVar[] parameters, TextureReference[] textures, params Shader[] shaders)
        {
            BindTextures = BindTexturesNonFBO;

            _name = name;
            _parameters = parameters ?? new ShaderVar[0];
            TexRefs = textures ?? new TextureReference[0];

            _shaders = shaders;
            _fragmentShaders = new List<Shader>();
            _geometryShaders = new List<Shader>();
            _tessCtrlShaders = new List<Shader>();
            _tessEvalShaders = new List<Shader>();

            if (shaders != null)
                foreach (Shader s in shaders)
                {
                    switch (s.ShaderType)
                    {
                        case ShaderMode.Vertex:
                            throw new Exception();
                        case ShaderMode.Fragment:
                            _fragmentShaders.Add(s);
                            break;
                        case ShaderMode.Geometry:
                            _geometryShaders.Add(s);
                            break;
                        case ShaderMode.TessControl:
                            _tessCtrlShaders.Add(s);
                            break;
                        case ShaderMode.TessEvaluation:
                            _tessEvalShaders.Add(s);
                            break;
                    }
                }

            _program = new RenderProgram(null, _shaders);
        }
        public static Material GetUnlitTextureMaterial(TextureReference texture) => GetUnlitTextureMaterial(texture, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitTextureMaterial(TextureReference texture, bool deferred)
        {
            TextureReference[] refs = new TextureReference[] { texture };
            ShaderVar[] parameters = new ShaderVar[0];
            Shader frag = deferred ? ShaderHelpers.UnlitTextureFragDeferred() : ShaderHelpers.UnlitTextureFragForward();
            return new Material("UnlitTextureMaterial", parameters, refs, frag);
        }
        public static Material GetUnlitTextureMaterial() => GetUnlitTextureMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitTextureMaterial(bool deferred)
        {
            Shader frag = deferred ? ShaderHelpers.UnlitTextureFragDeferred() : ShaderHelpers.UnlitTextureFragForward();
            return new Material("UnlitTextureMaterial", frag);
        }
        public static Material GetLitTextureMaterial() => GetLitTextureMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitTextureMaterial(bool deferred)
        {
            Shader frag = deferred ? ShaderHelpers.LitTextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
            return new Material("LitTextureMaterial", frag);
        }

        public static Material GetUnlitColorMaterial()
            => GetUnlitColorMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitColorMaterial(bool deferred)
            => GetUnlitColorMaterial(Color.DarkTurquoise, deferred);
        public static Material GetUnlitColorMaterial(ColorF4 color)
            => GetUnlitColorMaterial(color, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitColorMaterial(ColorF4 color, bool deferred)
        {
            TextureReference[] refs = new TextureReference[0];
            ShaderVar[] parameters = new ShaderVar[]
            {
                new GLVec4(color, "MatColor"),
            };
            Shader frag = deferred ? ShaderHelpers.UnlitColorFragDeferred() : ShaderHelpers.UnlitColorFragForward();
            return new Material("UnlitColorMaterial", parameters, refs, frag);
        }
        public static Material GetLitColorMaterial()
            => GetLitColorMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(bool deferred)
            => GetLitColorMaterial(Color.DarkTurquoise, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(ColorF4 color)
            => GetLitColorMaterial(color, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(ColorF4 color, bool deferred)
        {
            TextureReference[] refs = new TextureReference[0];
            ShaderVar[] parameters = new ShaderVar[]
            {
                new GLVec4(color, "MatColor"),
                new GLFloat(20.0f, "MatSpecularIntensity"),
                new GLFloat(128.0f, "MatShininess"),
            };
            Shader frag = deferred ? ShaderHelpers.LitColorFragDeferred() : ShaderHelpers.LitColorFragForward();
            return new Material("TestMaterial", parameters, refs, frag);
        }
    }
}
