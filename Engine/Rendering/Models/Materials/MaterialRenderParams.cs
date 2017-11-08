using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileClass("MTRP", "Material Rendering Parameters")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RenderingParameters : FileObject
    {
        [Browsable(false)]
        public bool HasTransparency => Blend.Enabled || AlphaTest.Enabled;

        [TSerialize]
        public bool WriteRed { get; set; } = true;
        [TSerialize]
        public bool WriteGreen { get; set; } = true;
        [TSerialize]
        public bool WriteBlue { get; set; } = true;
        [TSerialize]
        public bool WriteAlpha { get; set; } = true;
        [TSerialize]
        public Culling CullMode { get; set; } = Culling.Back;
        [TSerialize]
        public AlphaTest AlphaTest { get; set; } = new AlphaTest();
        [TSerialize]
        public DepthTest DepthTest { get; set; } = new DepthTest();
        [TSerialize]
        public StencilTest StencilTest { get; set; } = new StencilTest();
        [TSerialize]
        public Blend Blend { get; set; } = new Blend();
        [TSerialize]
        public float PointSize { get; set; } = AbstractRenderer.DefaultPointSize;
        [TSerialize]
        public float LineWidth { get; set; } = AbstractRenderer.DefaultLineSize;
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AlphaTest
    {
        private bool _enabled;
        private bool _useConstantAlpha;
        private float _constantAlphaValue;
        private bool _useAlphaToCoverage;
        private float _ref0, _ref1;
        private EComparison _comp0 = EComparison.Always, _comp1 = EComparison.Always;
        private ELogicGate _logicGate = ELogicGate.And;

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public bool UseConstantAlpha { get => _useConstantAlpha; set => _useConstantAlpha = value; }
        public float ConstantAlphaValue { get => _constantAlphaValue; set => _constantAlphaValue = value; }
        public bool UseAlphaToCoverage { get => _useAlphaToCoverage; set => _useAlphaToCoverage = value; }
        public float Ref0 { get => _ref0; set => _ref0 = value; }
        public float Ref1 { get => _ref1; set => _ref1 = value; }
        public EComparison Comp0 { get => _comp0; set => _comp0 = value; }
        public EComparison Comp1 { get => _comp1; set => _comp1 = value; }
        public ELogicGate LogicGate { get => _logicGate; set => _logicGate = value; }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StencilFace
    {
        private EComparison _func = EComparison.Always;
        private int _ref;
        private int _mask;

        public EComparison Func { get => _func; set => _func = value; }
        public int Ref { get => _ref; set => _ref = value; }
        public int Mask { get => _mask; set => _mask = value; }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StencilTest
    {
        private bool _enableStencilFunc = false;
        private StencilFace _frontFace = new StencilFace(), _backFace = new StencilFace();

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
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DepthTest
    {
        /// <summary>
        /// Determines if this material will test against the previously written depth value to determine if color fragments should be written or not.
        /// </summary>
        [TSerialize]
        [Description("Determines if this material will test against the previously written depth value to determine if color fragments should be written or not.")]
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// Determines if the material will update the depth value upon writing a new color fragment.
        /// </summary>
        [TSerialize]
        [Description("Determines if the material will update the depth value upon writing a new color fragment.")]
        public bool UpdateDepth { get; set; } = true;
        /// <summary>
        /// Determines the pass condition to write a new color fragment. Usually less or lequal, meaning closer to the camera than the previous depth means a success.
        /// </summary>
        [TSerialize]
        [Description("Determines the pass condition to write a new color fragment. Usually less or lequal, meaning closer to the camera than the previous depth means a success.")]
        public EComparison Function { get; set; } = EComparison.Lequal;
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Blend
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

        [TSerialize]
        public bool Enabled { get => _enabled; set => _enabled = value; }
        [TSerialize]
        public EBlendEquationMode RgbEquation { get => _rgbEquation; set => _rgbEquation = value; }
        [TSerialize]
        public EBlendEquationMode AlphaEquation { get => _alphaEquation; set => _alphaEquation = value; }
        [TSerialize]
        public EBlendingFactor RgbSrcFactor { get => _rgbSrcFactor; set => _rgbSrcFactor = value; }
        [TSerialize]
        public EBlendingFactor AlphaSrcFactor { get => _alphaSrcFactor; set => _alphaSrcFactor = value; }
        [TSerialize]
        public EBlendingFactor RgbDstFactor { get => _rgbDstFactor; set => _rgbDstFactor = value; }
        [TSerialize]
        public EBlendingFactor AlphaDstFactor { get => _alphaDstFactor; set => _alphaDstFactor = value; }
    }
}
