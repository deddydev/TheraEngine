using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

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

        internal Shader
            _vertexShader,
            _fragmentShader, 
            _geometryShader,
            _tessellationControlShader, 
            _tessellationEvaluationShader;

        private List<TextureReference> _textures = new List<TextureReference>();
        private List<GLVar> _parameters = new List<GLVar>();
        private List<I3DRenderable> _renderingReferences = new List<I3DRenderable>();
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

        public bool HasTransparency => _renderParams.Blend.EnableBlending || _renderParams.Alpha.EnableAlphaTest;

        internal void AddReference(I3DRenderable user)
        {
            if (_renderingReferences.Count == 0)
                _bindingId = Engine.Scene.AddActiveMaterial(this);
            _renderingReferences.Add(user);
        }
        internal void RemoveReference(I3DRenderable user)
        {
            _renderingReferences.Add(user);
            if (_renderingReferences.Count == 0)
            {
                Engine.Scene.RemoveActiveMaterial(this);
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
            _parameters = parameters ?? new List<GLVar>();
            _textures = textures ?? new List<TextureReference>();
            if (shaders != null)
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
        public static Material GetUnlitTextureMaterial(TextureReference texture) => GetUnlitTextureMaterial(texture, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetUnlitTextureMaterial(TextureReference texture, bool deferred)
        {
            List<TextureReference> refs = new List<TextureReference>() { texture };
            List<GLVar> parameters = new List<GLVar>();
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
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = deferred ? ShaderHelpers.UnlitColorFragDeferred() : ShaderHelpers.UnlitColorFragForward();
            List<GLVar> parameters = new List<GLVar>()
            {
                new GLVec4(color, "MatColor"),
            };
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
            List<TextureReference> refs = new List<TextureReference>();
            Shader frag = deferred ? ShaderHelpers.LitColorFragDeferred() : ShaderHelpers.LitColorFragForward();
            List<GLVar> parameters = new List<GLVar>()
            {
                new GLVec4(color, "MatColor"),
                new GLFloat(20.0f, "MatSpecularIntensity"),
                new GLFloat(128.0f, "MatShininess"),
            };
            return new Material("TestMaterial", parameters, refs, frag);
        }
    }
}
