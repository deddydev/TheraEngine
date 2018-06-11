using System;
using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum ERenderParamUsage
    {
        Unchanged,
        Disabled,
        Enabled,
    }
    [Flags]
    public enum EUniformRequirements
    {
        None = 0b00000,
        Camera = 0b00001,
        Lights = 0b00010,
        RenderTime = 0b00100,
        ViewportDimensions = 0b01000,
        MousePosition = 0b10000,

        LightsAndCamera = Lights | Camera,
    }
    /// <summary>
    /// Contains parameters for rendering an object, such as blending and depth testing.
    /// </summary>
    [FileExt("rendparams")]
    [FileDef("Rendering Parameters", 
        "Contains parameters for rendering an object, such as blending and depth testing.")]
    public class RenderingParameters : TFileObject
    {
        //AlphaTest _alphaTest = new AlphaTest();
        DepthTest _depthTest = new DepthTest();
        StencilTest _stencilTest = new StencilTest();
        BlendMode _blendMode = new BlendMode();

        [Browsable(false)]
        public bool HasTransparency => BlendMode.Enabled == ERenderParamUsage.Enabled;

        public RenderingParameters() { }
        public RenderingParameters(bool defaultBlendEnabled/*, float? defaultAlphaTestDiscardMax*/)
        {
            if (defaultBlendEnabled)
            {
                BlendMode.Enabled = ERenderParamUsage.Enabled;
                BlendMode.RgbSrcFactor = EBlendingFactor.SrcAlpha;
                BlendMode.AlphaSrcFactor = EBlendingFactor.SrcAlpha;
                BlendMode.RgbDstFactor = EBlendingFactor.OneMinusSrcAlpha;
                BlendMode.AlphaDstFactor = EBlendingFactor.OneMinusSrcAlpha;
                BlendMode.RgbEquation = EBlendEquationMode.FuncAdd;
                BlendMode.AlphaEquation = EBlendEquationMode.FuncAdd;
            }
            //if (defaultAlphaTestDiscardMax != null)
            //{
            //    AlphaTest.Enabled = ERenderParamUsage.Enabled;
            //    AlphaTest.Ref = defaultAlphaTestDiscardMax.Value;
            //    AlphaTest.Comp = EComparison.Lequal;
            //}
        }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EUniformRequirements Requirements { get; set; } = EUniformRequirements.None;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteRed { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteGreen { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteBlue { get; set; } = true;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool WriteAlpha { get; set; } = true;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public ECulling CullMode { get; set; } = ECulling.Back;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float PointSize { get; set; } = AbstractRenderer.DefaultPointSize;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float LineWidth { get; set; } = AbstractRenderer.DefaultLineSize;

        //[TSerialize]
        //public AlphaTest AlphaTest { get => _alphaTest; set => _alphaTest = value ?? new AlphaTest(); }
        [TSerialize]
        public DepthTest DepthTest { get => _depthTest; set => _depthTest = value ?? new DepthTest(); }
        [TSerialize]
        public StencilTest StencilTest { get => _stencilTest; set => _stencilTest = value ?? new StencilTest(); }
        [TSerialize]
        public BlendMode BlendMode { get => _blendMode; set => _blendMode = value ?? new BlendMode(); }

        internal void SetUniforms(int programBindingId, ref float secondsLive)
        {
            //Set engine uniforms
            if (Requirements.HasFlag(EUniformRequirements.Camera))
                AbstractRenderer.CurrentCamera.SetUniforms(programBindingId);
            if (Requirements.HasFlag(EUniformRequirements.Lights))
                AbstractRenderer.Current3DScene.Lights.SetUniforms(programBindingId);
            if (Requirements.HasFlag(EUniformRequirements.RenderTime))
            {
                secondsLive += Engine.UpdateDelta;
                Engine.Renderer.Uniform(programBindingId, nameof(EUniformRequirements.RenderTime), secondsLive);
            }
            if (Requirements.HasFlag(EUniformRequirements.ViewportDimensions))
            {
                //Engine.Renderer.Uniform(programBindingId, nameof(UniformRequirements.ViewportDimensions), viewportDimensions);
            }
            if (Requirements.HasFlag(EUniformRequirements.MousePosition))
            {
                //Engine.Renderer.Uniform(programBindingId, nameof(UniformRequirements.MousePosition), mousePosition);
            }
        }
    }
    //public class AlphaTest
    //{
    //    //private bool _useConstantAlpha;
    //    //private float _constantAlphaValue;
    //    //private bool _useAlphaToCoverage;
    //    //private ELogicGate _logicGate = ELogicGate.And;

    //    [Browsable(false)]
    //    public bool IsEnabled => Enabled == ERenderParamUsage.Enabled;
    //    [Browsable(false)]
    //    public bool IsDisable => Enabled == ERenderParamUsage.Disabled;
    //    [Browsable(false)]
    //    public bool IsUnchanged => Enabled == ERenderParamUsage.Unchanged;

    //    [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
    //    public ERenderParamUsage Enabled { get; set; } = ERenderParamUsage.Disabled;
    //    //[TSerialize(Condition = "Enabled")]
    //    //public bool UseConstantAlpha { get => _useConstantAlpha; set => _useConstantAlpha = value; }
    //    //[TSerialize(Condition = "Enabled")]
    //    //public float ConstantAlphaValue { get => _constantAlphaValue; set => _constantAlphaValue = value; }
    //    //[TSerialize(Condition = "Enabled")]
    //    //public bool UseAlphaToCoverage { get => _useAlphaToCoverage; set => _useAlphaToCoverage = value; }
    //    [TSerialize(Condition = nameof(IsEnabled))]
    //    public float Ref { get; set; }
    //    //[TSerialize(Condition = "Enabled")]
    //    //public float Ref1 { get => _ref1; set => _ref1 = value; }
    //    [TSerialize(Condition = nameof(IsEnabled))]
    //    public EComparison Comp { get; set; } = EComparison.Always;
    //    //[TSerialize(Condition = "Enabled")]
    //    //public EComparison Comp1 { get => _comp1; set => _comp1 = value; }
    //    //[TSerialize(Condition = "Enabled")]
    //    //public ELogicGate LogicGate { get => _logicGate; set => _logicGate = value; }
    //}
    public class StencilTestFace
    {
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EStencilOp BothFailOp { get; set; } = EStencilOp.Keep;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EStencilOp StencilPassDepthFailOp { get; set; } = EStencilOp.Keep;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EStencilOp BothPassOp { get; set; } = EStencilOp.Keep;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EComparison Func { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int Ref { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int ReadMask { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int WriteMask { get; set; }

        public override string ToString()
        {
            return string.Format("{0} Ref:{1} Read Mask:{2} Write Mask:{3}", Func, Ref, ReadMask, WriteMask);
        }
    }
    public class StencilTest
    {
        private StencilTestFace 
            _frontFace = new StencilTestFace(),
            _backFace = new StencilTestFace();

        [Browsable(false)]
        public bool IsEnabled => Enabled == ERenderParamUsage.Enabled;
        [Browsable(false)]
        public bool IsDisable => Enabled == ERenderParamUsage.Disabled;
        [Browsable(false)]
        public bool IsUnchanged => Enabled == ERenderParamUsage.Unchanged;
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public ERenderParamUsage Enabled { get; set; } = ERenderParamUsage.Disabled;
        [TSerialize(Condition = nameof(IsEnabled))]
        public StencilTestFace FrontFace { get => _frontFace; set => _frontFace = value ?? new StencilTestFace(); }
        [TSerialize(Condition = nameof(IsEnabled))]
        public StencilTestFace BackFace { get => _backFace; set => _backFace = value ?? new StencilTestFace(); }
        
        public override string ToString()
        {
            return 
                Enabled == ERenderParamUsage.Unchanged ? "Unchanged" : 
                Enabled == ERenderParamUsage.Disabled ? "Disabled" : 
                string.Format("[Front: {0}] - [Back: {1}]", FrontFace.ToString(), BackFace.ToString());
        }
    }
    public class DepthTest
    {
        [Browsable(false)]
        public bool IsEnabled => Enabled == ERenderParamUsage.Enabled;
        [Browsable(false)]
        public bool IsDisable => Enabled == ERenderParamUsage.Disabled;
        [Browsable(false)]
        public bool IsUnchanged => Enabled == ERenderParamUsage.Unchanged;

        /// <summary>
        /// Determines if this material will test against the previously written depth value to determine if color fragments should be written or not.
        /// </summary>
        [TSerialize(Order = 0, XmlNodeType = EXmlNodeType.Attribute)]
        [Description("Determines if this material will test against the previously written depth value to determine if color fragments should be written or not.")]
        public ERenderParamUsage Enabled { get; set; } = ERenderParamUsage.Enabled;
        /// <summary>
        /// Determines if the material will update the depth value upon writing a new color fragment.
        /// </summary>
        [TSerialize(Order = 1, XmlNodeType = EXmlNodeType.Attribute, Condition = nameof(IsEnabled))]
        [Description("Determines if the material will update the depth value upon writing a new color fragment.")]
        public bool UpdateDepth { get; set; } = true;
        /// <summary>
        /// Determines the pass condition to write a new color fragment. Usually less or lequal, meaning closer to the camera than the previous depth means a success.
        /// </summary>
        [TSerialize(Order = 2, XmlNodeType = EXmlNodeType.Attribute, Condition = nameof(IsEnabled))]
        [Description("Determines the pass condition to write a new color fragment. Usually less or lequal, meaning closer to the camera than the previous depth means a success.")]
        public EComparison Function { get; set; } = EComparison.Lequal;

        public override string ToString()
        {
            return
                Enabled == ERenderParamUsage.Unchanged ? "Unchanged" :
                Enabled == ERenderParamUsage.Disabled ? "Disabled" : 
                string.Format("[{0}, Write Depth:{1}]", Function, UpdateDepth);
        }
    }
    public class BlendMode
    {
        [Browsable(false)]
        public bool IsEnabled => Enabled == ERenderParamUsage.Enabled;
        [Browsable(false)]
        public bool IsDisable => Enabled == ERenderParamUsage.Disabled;
        [Browsable(false)]
        public bool IsUnchanged => Enabled == ERenderParamUsage.Unchanged;
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public ERenderParamUsage Enabled { get; set; } = ERenderParamUsage.Disabled;
        [TSerialize(Condition = nameof(IsEnabled))]
        public EBlendEquationMode RgbEquation { get; set; } = EBlendEquationMode.FuncAdd;
        [TSerialize(Condition = nameof(IsEnabled))]
        public EBlendEquationMode AlphaEquation { get; set; } = EBlendEquationMode.FuncAdd;
        [TSerialize(Condition = nameof(IsEnabled))]
        public EBlendingFactor RgbSrcFactor { get; set; } = EBlendingFactor.ConstantColor;
        [TSerialize(Condition = nameof(IsEnabled))]
        public EBlendingFactor AlphaSrcFactor { get; set; } = EBlendingFactor.ConstantAlpha;
        [TSerialize(Condition = nameof(IsEnabled))]
        public EBlendingFactor RgbDstFactor { get; set; } = EBlendingFactor.ConstantColor;
        [TSerialize(Condition = nameof(IsEnabled))]
        public EBlendingFactor AlphaDstFactor { get; set; } = EBlendingFactor.ConstantAlpha;

        public override string ToString()
        {
            return
                Enabled == ERenderParamUsage.Unchanged ? "Unchanged" :
                Enabled == ERenderParamUsage.Disabled ? "Disabled" : 
                string.Format("[RGB: {0} {1} {2}] - [Alpha: {3} {4} {5}]", RgbEquation, RgbSrcFactor, RgbDstFactor, AlphaEquation, AlphaSrcFactor, AlphaDstFactor);
        }
    }
}
