using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileClass("MTRP", "Material Rendering Parameters")]
    public class RenderingParameters : FileObject
    {
        AlphaTest _alphaTest = new AlphaTest();
        DepthTest _depthTest = new DepthTest();
        StencilTest _stencilTest = new StencilTest();
        BlendMode _blendMode = new BlendMode();

        [Browsable(false)]
        public bool HasTransparency => BlendMode.Enabled || AlphaTest.Enabled;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteRed { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteGreen { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteBlue { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteAlpha { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Culling CullMode { get; set; } = Culling.Back;
        [TSerialize]
        public AlphaTest AlphaTest { get => _alphaTest; set => _alphaTest = value ?? new AlphaTest(); }
        [TSerialize]
        public DepthTest DepthTest { get => _depthTest; set => _depthTest = value ?? new DepthTest(); }
        [TSerialize]
        public StencilTest StencilTest { get => _stencilTest; set => _stencilTest = value ?? new StencilTest(); }
        [TSerialize]
        public BlendMode BlendMode { get => _blendMode; set => _blendMode = value ?? new BlendMode(); }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float PointSize { get; set; } = AbstractRenderer.DefaultPointSize;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float LineWidth { get; set; } = AbstractRenderer.DefaultLineSize;
    }
    public class AlphaTest
    {
        private bool _enabled = false;
        private bool _useConstantAlpha;
        private float _constantAlphaValue;
        private bool _useAlphaToCoverage;
        private float _ref0, _ref1;
        private EComparison _comp0 = EComparison.Always, _comp1 = EComparison.Always;
        private ELogicGate _logicGate = ELogicGate.And;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool Enabled { get => _enabled; set => _enabled = value; }
        [TSerialize(Condition = "Enabled")]
        public bool UseConstantAlpha { get => _useConstantAlpha; set => _useConstantAlpha = value; }
        [TSerialize(Condition = "Enabled")]
        public float ConstantAlphaValue { get => _constantAlphaValue; set => _constantAlphaValue = value; }
        [TSerialize(Condition = "Enabled")]
        public bool UseAlphaToCoverage { get => _useAlphaToCoverage; set => _useAlphaToCoverage = value; }
        [TSerialize(Condition = "Enabled")]
        public float Ref0 { get => _ref0; set => _ref0 = value; }
        [TSerialize(Condition = "Enabled")]
        public float Ref1 { get => _ref1; set => _ref1 = value; }
        [TSerialize(Condition = "Enabled")]
        public EComparison Comp0 { get => _comp0; set => _comp0 = value; }
        [TSerialize(Condition = "Enabled")]
        public EComparison Comp1 { get => _comp1; set => _comp1 = value; }
        [TSerialize(Condition = "Enabled")]
        public ELogicGate LogicGate { get => _logicGate; set => _logicGate = value; }
    }
    public class StencilFace
    {
        private EComparison _func = EComparison.Always;
        private int _ref;
        private int _mask;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EComparison Func { get => _func; set => _func = value; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int Ref { get => _ref; set => _ref = value; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int Mask { get => _mask; set => _mask = value; }
    }
    public class StencilTest
    {
        private bool _enableStencilFunc = false;
        private StencilFace 
            _frontFace = new StencilFace(),
            _backFace = new StencilFace();

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool EnableStencilFunc { get => _enableStencilFunc; set => _enableStencilFunc = value; }
        [TSerialize(Condition = "EnableStencilFunc")]
        public StencilFace FrontFace { get => _frontFace; set => _frontFace = value ?? new StencilFace(); }
        [TSerialize(Condition = "EnableStencilFunc")]
        public StencilFace BackFace { get => _backFace; set => _backFace = value ?? new StencilFace(); }

        //public EComparison FrontFaceFunc { get => _frontFace.Func; set => _frontFace.Func = value; }
        //public EComparison BackFaceFunc { get => _backFace.Func; set => _backFace.Func = value; }
        //public int FrontFaceRef { get => _frontFace.Ref; set => _frontFace.Ref = value; }
        //public int BackFaceRef { get => _backFace.Ref; set => _backFace.Ref = value; }
        //public int FrontFaceMask { get => _frontFace.Mask; set => _frontFace.Mask = value; }
        //public int BackFaceMask { get => _backFace.Mask; set => _backFace.Mask = value; }
    }
    public class DepthTest
    {
        /// <summary>
        /// Determines if this material will test against the previously written depth value to determine if color fragments should be written or not.
        /// </summary>
        [TSerialize(Order = 0, XmlNodeType = EXmlNodeType.Attribute)]
        [Description("Determines if this material will test against the previously written depth value to determine if color fragments should be written or not.")]
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// Determines if the material will update the depth value upon writing a new color fragment.
        /// </summary>
        [TSerialize(Order = 1, XmlNodeType = EXmlNodeType.Attribute)]
        [Description("Determines if the material will update the depth value upon writing a new color fragment.")]
        public bool UpdateDepth { get; set; } = true;
        /// <summary>
        /// Determines the pass condition to write a new color fragment. Usually less or lequal, meaning closer to the camera than the previous depth means a success.
        /// </summary>
        [TSerialize(Order = 2, XmlNodeType = EXmlNodeType.Attribute, Condition = "Enabled")]
        [Description("Determines the pass condition to write a new color fragment. Usually less or lequal, meaning closer to the camera than the previous depth means a success.")]
        public EComparison Function { get; set; } = EComparison.Lequal;
    }
    public class BlendMode
    {
        private bool _enabled = false;

        private EBlendEquationMode 
            _rgbEquation = EBlendEquationMode.FuncAdd, 
            _alphaEquation = EBlendEquationMode.FuncAdd;

        private EBlendingFactor 
            _rgbSrcFactor = EBlendingFactor.ConstantColor,
            _alphaSrcFactor = EBlendingFactor.ConstantAlpha,
            _rgbDstFactor = EBlendingFactor.ConstantColor,
            _alphaDstFactor = EBlendingFactor.ConstantAlpha;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool Enabled { get => _enabled; set => _enabled = value; }
        [TSerialize(Condition = "Enabled")]
        public EBlendEquationMode RgbEquation { get => _rgbEquation; set => _rgbEquation = value; }
        [TSerialize(Condition = "Enabled")]
        public EBlendEquationMode AlphaEquation { get => _alphaEquation; set => _alphaEquation = value; }
        [TSerialize(Condition = "Enabled")]
        public EBlendingFactor RgbSrcFactor { get => _rgbSrcFactor; set => _rgbSrcFactor = value; }
        [TSerialize(Condition = "Enabled")]
        public EBlendingFactor AlphaSrcFactor { get => _alphaSrcFactor; set => _alphaSrcFactor = value; }
        [TSerialize(Condition = "Enabled")]
        public EBlendingFactor RgbDstFactor { get => _rgbDstFactor; set => _rgbDstFactor = value; }
        [TSerialize(Condition = "Enabled")]
        public EBlendingFactor AlphaDstFactor { get => _alphaDstFactor; set => _alphaDstFactor = value; }
    }
}
