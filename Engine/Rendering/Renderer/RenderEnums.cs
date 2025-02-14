﻿using System;

namespace TheraEngine.Rendering
{
    public enum EConditionalRenderType
    {
        QueryWait = 36371,
        QueryNoWait = 36372,
        QueryByRegionWait = 36373,
        QueryByRegionNoWait = 36374,
        QueryWaitInverted = 36375,
        QueryNoWaitInverted = 36376,
        QueryByRegionWaitInverted = 36377,
        QueryByRegionNoWaitInverted = 36378
    }
    [Flags]
    public enum EProgramStageMask
    {
        AllShaderBits           = -1,
        None                    = 00,
        VertexShaderBit         = 01,
        FragmentShaderBit       = 02,
        GeometryShaderBit       = 04,
        TessControlShaderBit    = 08,
        TessEvaluationShaderBit = 16,
        ComputeShaderBit        = 32
    }
    public enum EProgParam
    {
        ProgramBinaryRetrievableHint = 33367,
        ProgramSeparable = 33368,
        //ProgramLength = 34343,
        //ProgramBinding = 34423,
        //ProgramAluInstructionsArb = 34821,
        //ProgramTexInstructionsArb = 34822,
        //ProgramTexIndirectionsArb = 34823,
        //ProgramNativeAluInstructionsArb = 34824,
        //ProgramNativeTexInstructionsArb = 34825,
        //ProgramNativeTexIndirectionsArb = 34826,
        //MaxProgramAluInstructionsArb = 34827,
        //MaxProgramTexInstructionsArb = 34828,
        //MaxProgramTexIndirectionsArb = 34829,
        //MaxProgramNativeAluInstructionsArb = 34830,
        //MaxProgramNativeTexInstructionsArb = 34831,
        //MaxProgramNativeTexIndirectionsArb = 34832,
        //ProgramFormat = 34934,
        //ProgramInstruction = 34976,
        //MaxProgramInstructions = 34977,
        //ProgramNativeInstructions = 34978,
        //MaxProgramNativeInstructions = 34979,
        //ProgramTemporaries = 34980,
        //MaxProgramTemporaries = 34981,
        //ProgramNativeTemporaries = 34982,
        //MaxProgramNativeTemporaries = 34983,
        //ProgramParameters = 34984,
        //MaxProgramParameters = 34985,
        //ProgramNativeParameters = 34986,
        //MaxProgramNativeParameters = 34987,
        //ProgramAttribs = 34988,
        //MaxProgramAttribs = 34989,
        //ProgramNativeAttribs = 34990,
        //MaxProgramNativeAttribs = 34991,
        //ProgramAddressRegisters = 34992,
        //MaxProgramAddressRegisters = 34993,
        //ProgramNativeAddressRegisters = 34994,
        //MaxProgramNativeAddressRegisters = 34995,
        //MaxProgramLocalParameters = 34996,
        //MaxProgramEnvParameters = 34997,
        //ProgramUnderNativeLimits = 34998,
        //GeometryVerticesOut = 35094,
        //GeometryInputType = 35095,
        //GeometryOutputType = 35096
    }
    //public enum ETexCoordWrap
    //{
    //    Clamp,
    //    Repeat,
    //    Mirror
    //}
    //public enum EMinFilter : uint
    //{
    //    Nearest,
    //    Linear,
    //    NearestMipmapNearest,
    //    LinearMipmapNearest,
    //    NearestMipmapLinear,
    //    LinearMipmapLinear
    //}
    //public enum EMagFilter : uint
    //{
    //    Nearest,
    //    Linear,
    //}
    public enum EBlendingFactor
    {
        Zero = 0,
        One = 1,
        SrcColor = 768,
        OneMinusSrcColor = 769,
        SrcAlpha = 770,
        OneMinusSrcAlpha = 771,
        DstAlpha = 772,
        OneMinusDstAlpha = 773,
        DstColor = 774,
        OneMinusDstColor = 775,
        SrcAlphaSaturate = 776,
        ConstantColor = 32769,
        //ConstantColorExt = 32769,
        OneMinusConstantColor = 32770,
        //OneMinusConstantColorExt = 32770,
        ConstantAlpha = 32771,
        //ConstantAlphaExt = 32771,
        OneMinusConstantAlpha = 32772,
        //OneMinusConstantAlphaExt = 32772,
        Src1Alpha = 34185,
        Src1Color = 35065,
        OneMinusSrc1Color = 35066,
        OneMinusSrc1Alpha = 35067
    }
    public enum EBlendEquationMode
    {
        FuncAdd = 32774,
        Min = 32775,
        Max = 32776,
        FuncSubtract = 32778,
        FuncReverseSubtract = 32779
    }
    public enum ELogicGate
    {
        And,
        Nand,
        Or,
        Nor,
        Xor,
        Xnor,
    }
    public enum EComparison
    {
        Never,
        Less,
        Equal,
        Lequal,
        Greater,
        Nequal,
        Gequal,
        Always
    }
    public enum ERenderBufferStorage
    {
        DepthComponent = 6402,
        R3G3B2 = 10768,
        Alpha4 = 32827,
        Alpha8 = 32828,
        Alpha12 = 32829,
        Alpha16 = 32830,
        Rgb4 = 32847,
        Rgb5 = 32848,
        Rgb8 = 32849,
        Rgb10 = 32850,
        Rgb12 = 32851,
        Rgb16 = 32852,
        Rgba2 = 32853,
        Rgba4 = 32854,
        Rgba8 = 32856,
        Rgb10A2 = 32857,
        Rgba12 = 32858,
        Rgba16 = 32859,
        DepthComponent16 = 33189,
        DepthComponent24 = 33190,
        DepthComponent32 = 33191,
        R8 = 33321,
        R16 = 33322,
        Rg8 = 33323,
        Rg16 = 33324,
        R16f = 33325,
        R32f = 33326,
        Rg16f = 33327,
        Rg32f = 33328,
        R8i = 33329,
        R8ui = 33330,
        R16i = 33331,
        R16ui = 33332,
        R32i = 33333,
        R32ui = 33334,
        Rg8i = 33335,
        Rg8ui = 33336,
        Rg16i = 33337,
        Rg16ui = 33338,
        Rg32i = 33339,
        Rg32ui = 33340,
        DepthStencil = 34041,
        Rgba32f = 34836,
        Rgb32f = 34837,
        Rgba16f = 34842,
        Rgb16f = 34843,
        Depth24Stencil8 = 35056,
        R11fG11fB10f = 35898,
        Rgb9E5 = 35901,
        Srgb8 = 35905,
        Srgb8Alpha8 = 35907,
        DepthComponent32f = 36012,
        Depth32fStencil8 = 36013,
        StencilIndex1 = 36166,
        StencilIndex1Ext = 36166,
        StencilIndex4 = 36167,
        StencilIndex4Ext = 36167,
        StencilIndex8 = 36168,
        StencilIndex8Ext = 36168,
        StencilIndex16 = 36169,
        StencilIndex16Ext = 36169,
        Rgba32ui = 36208,
        Rgb32ui = 36209,
        Rgba16ui = 36214,
        Rgb16ui = 36215,
        Rgba8ui = 36220,
        Rgb8ui = 36221,
        Rgba32i = 36226,
        Rgb32i = 36227,
        Rgba16i = 36232,
        Rgb16i = 36233,
        Rgba8i = 36238,
        Rgb8i = 36239,
        Rgb10A2ui = 36975
    }
    public enum FeedbackPrimitiveType
    {
        Points,
        Lines,
        Triangles,
    }
    [Flags]
    public enum EMemoryBarrierFlags
    {
        AllBarrierBits = -1,
        VertexAttribArrayBarrierBit = 1,
        ElementArrayBarrierBit = 2,
        UniformBarrierBit = 4,
        TextureFetchBarrierBit = 8,
        ShaderImageAccessBarrierBit = 32,
        CommandBarrierBit = 64,
        PixelBufferBarrierBit = 128,
        TextureUpdateBarrierBit = 256,
        BufferUpdateBarrierBit = 512,
        FramebufferBarrierBit = 1024,
        TransformFeedbackBarrierBit = 2048,
        AtomicCounterBarrierBit = 4096,
        ShaderStorageBarrierBit = 8192,
        ClientMappedBufferBarrierBit = 16384,
        QueryBufferBarrierBit = 32768
    }
    [Flags]
    public enum EMemoryBarrierRegionFlags
    {
        GlAllBarrierBits = -1,
        GlUniformBarrierBit = 4,
        GlTextureFetchBarrierBit = 8,
        GlShaderImageAccessBarrierBit = 32,
        GlFramebufferBarrierBit = 1024,
        GlAtomicCounterBarrierBit = 4096
    }
    [Flags]
    public enum EFBOTextureType
    {
        None    = 0x0,
        Color   = 0x1,
        Depth   = 0x2,
        Stencil = 0x4,
        Accum   = 0x8,
        All     = 0xF,
    }
    public enum EPrimitive
    {
        Points          = 0,
        Lines           = 1,
        LineLoop        = 2,
        LineStrip       = 3,
        Triangles       = 4,
        TriangleStrip   = 5,
        TriangleFan     = 6,
        Quads           = 7,
        QuadStrip       = 8,
    }
    public enum EPixelType
    {
        Byte                        = 5120,
        UnsignedByte                = 5121,
        Short                       = 5122,
        UnsignedShort               = 5123,
        Int                         = 5124,
        UnsignedInt                 = 5125,
        Float                       = 5126,
        HalfFloat                   = 5131,
        Bitmap                      = 6656,
        UnsignedByte332             = 32818,
        //UnsignedByte332Ext          = 32818,
        UnsignedShort4444           = 32819,
        //UnsignedShort4444Ext        = 32819,
        UnsignedShort5551           = 32820,
        //UnsignedShort5551Ext        = 32820,
        UnsignedInt8888             = 32821,
        //UnsignedInt8888Ext          = 32821,
        UnsignedInt1010102          = 32822,
        //UnsignedInt1010102Ext       = 32822,
        UnsignedByte233Reversed     = 33634,
        UnsignedShort565            = 33635,
        UnsignedShort565Reversed    = 33636,
        UnsignedShort4444Reversed   = 33637,
        UnsignedShort1555Reversed   = 33638,
        UnsignedInt8888Reversed     = 33639,
        UnsignedInt2101010Reversed  = 33640,
        UnsignedInt248              = 34042,
        UnsignedInt10F11F11FRev     = 35899,
        UnsignedInt5999Rev          = 35902,
        Float32UnsignedInt248Rev    = 36269
    }
    public enum EPixelFormat
    {
        UnsignedShort               = 5123,
        UnsignedInt                 = 5125,
        ColorIndex                  = 6400,
        StencilIndex                = 6401,
        DepthComponent              = 6402,
        Red                         = 6403,
        //RedExt                      = 6403,
        Green                       = 6404,
        Blue                        = 6405,
        Alpha                       = 6406,
        Rgb                         = 6407,
        Rgba                        = 6408,
        Luminance                   = 6409,
        LuminanceAlpha              = 6410,
        //AbgrExt                     = 32768,
        //CmykExt                     = 32780,
        //CmykaExt                    = 32781,
        Bgr                         = 32992,
        Bgra                        = 32993,
        //Ycrcb422Sgix                = 33211,
        //Ycrcb444Sgix                = 33212,
        Rg                          = 33319,
        RgInteger                   = 33320,
        //R5G6B5IccSgix               = 33894,
        //R5G6B5A8IccSgix             = 33895,
        //Alpha16IccSgix              = 33896,
        //Luminance16IccSgix          = 33897,
        //Luminance16Alpha8IccSgix    = 33899,
        DepthStencil                = 34041,
        RedInteger                  = 36244,
        GreenInteger                = 36245,
        BlueInteger                 = 36246,
        AlphaInteger                = 36247,
        RgbInteger                  = 36248,
        RgbaInteger                 = 36249,
        BgrInteger                  = 36250,
        BgraInteger                 = 36251
    }
    public enum EDepthPrecision
    {
        Int16,
        Int24,
        Int32,
        Flt32,
    }
    public enum ESizedInternalFormat
    {
        Rgba8 = 32856,
        Rgba16 = 32859,
        R8 = 33321,
        R16 = 33322,
        Rg8 = 33323,
        Rg16 = 33324,
        R16f = 33325,
        R32f = 33326,
        Rg16f = 33327,
        Rg32f = 33328,
        R8i = 33329,
        R8ui = 33330,
        R16i = 33331,
        R16ui = 33332,
        R32i = 33333,
        R32ui = 33334,
        Rg8i = 33335,
        Rg8ui = 33336,
        Rg16i = 33337,
        Rg16ui = 33338,
        Rg32i = 33339,
        Rg32ui = 33340,
        Rgba32f = 34836,
        Rgba16f = 34842,
        Rgba32ui = 36208,
        Rgba16ui = 36214,
        Rgba8ui = 36220,
        Rgba32i = 36226,
        Rgba16i = 36232,
        Rgba8i = 36238
    }
    public enum EPixelInternalFormat
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        DepthComponent = 6402,
        Alpha = 6406,
        Rgb = 6407,
        Rgba = 6408,
        Luminance = 6409,
        LuminanceAlpha = 6410,
        R3G3B2 = 10768,
        Alpha4 = 32827,
        Alpha8 = 32828,
        Alpha12 = 32829,
        Alpha16 = 32830,
        Luminance4 = 32831,
        Luminance8 = 32832,
        Luminance12 = 32833,
        Luminance16 = 32834,
        Luminance4Alpha4 = 32835,
        Luminance6Alpha2 = 32836,
        Luminance8Alpha8 = 32837,
        Luminance12Alpha4 = 32838,
        Luminance12Alpha12 = 32839,
        Luminance16Alpha16 = 32840,
        Intensity = 32841,
        Intensity4 = 32842,
        Intensity8 = 32843,
        Intensity12 = 32844,
        Intensity16 = 32845,
        Rgb2Ext = 32846,
        Rgb4 = 32847,
        Rgb5 = 32848,
        Rgb8 = 32849,
        Rgb10 = 32850,
        Rgb12 = 32851,
        Rgb16 = 32852,
        Rgba2 = 32853,
        Rgba4 = 32854,
        Rgb5A1 = 32855,
        Rgba8 = 32856,
        Rgb10A2 = 32857,
        Rgba12 = 32858,
        Rgba16 = 32859,
        DualAlpha4Sgis = 33040,
        DualAlpha8Sgis = 33041,
        DualAlpha12Sgis = 33042,
        DualAlpha16Sgis = 33043,
        DualLuminance4Sgis = 33044,
        DualLuminance8Sgis = 33045,
        DualLuminance12Sgis = 33046,
        DualLuminance16Sgis = 33047,
        DualIntensity4Sgis = 33048,
        DualIntensity8Sgis = 33049,
        DualIntensity12Sgis = 33050,
        DualIntensity16Sgis = 33051,
        DualLuminanceAlpha4Sgis = 33052,
        DualLuminanceAlpha8Sgis = 33053,
        QuadAlpha4Sgis = 33054,
        QuadAlpha8Sgis = 33055,
        QuadLuminance4Sgis = 33056,
        QuadLuminance8Sgis = 33057,
        QuadIntensity4Sgis = 33058,
        QuadIntensity8Sgis = 33059,
        DepthComponent16 = 33189,
        //DepthComponent16Sgix = 33189,
        DepthComponent24 = 33190,
        //DepthComponent24Sgix = 33190,
        DepthComponent32 = 33191,
        //DepthComponent32Sgix = 33191,
        CompressedRed = 33317,
        CompressedRg = 33318,
        R8 = 33321,
        R16 = 33322,
        Rg8 = 33323,
        Rg16 = 33324,
        R16f = 33325,
        R32f = 33326,
        Rg16f = 33327,
        Rg32f = 33328,
        R8i = 33329,
        R8ui = 33330,
        R16i = 33331,
        R16ui = 33332,
        R32i = 33333,
        R32ui = 33334,
        Rg8i = 33335,
        Rg8ui = 33336,
        Rg16i = 33337,
        Rg16ui = 33338,
        Rg32i = 33339,
        Rg32ui = 33340,
        CompressedRgbS3tcDxt1Ext = 33776,
        CompressedRgbaS3tcDxt1Ext = 33777,
        CompressedRgbaS3tcDxt3Ext = 33778,
        CompressedRgbaS3tcDxt5Ext = 33779,
        RgbIccSgix = 33888,
        RgbaIccSgix = 33889,
        AlphaIccSgix = 33890,
        LuminanceIccSgix = 33891,
        IntensityIccSgix = 33892,
        LuminanceAlphaIccSgix = 33893,
        R5G6B5IccSgix = 33894,
        R5G6B5A8IccSgix = 33895,
        Alpha16IccSgix = 33896,
        Luminance16IccSgix = 33897,
        Intensity16IccSgix = 33898,
        Luminance16Alpha8IccSgix = 33899,
        CompressedAlpha = 34025,
        CompressedLuminance = 34026,
        CompressedLuminanceAlpha = 34027,
        CompressedIntensity = 34028,
        CompressedRgb = 34029,
        CompressedRgba = 34030,
        DepthStencil = 34041,
        Rgba32f = 34836,
        Rgb32f = 34837,
        Rgba16f = 34842,
        Rgb16f = 34843,
        Depth24Stencil8 = 35056,
        R11fG11fB10f = 35898,
        Rgb9E5 = 35901,
        Srgb = 35904,
        Srgb8 = 35905,
        SrgbAlpha = 35906,
        Srgb8Alpha8 = 35907,
        SluminanceAlpha = 35908,
        Sluminance8Alpha8 = 35909,
        Sluminance = 35910,
        Sluminance8 = 35911,
        CompressedSrgb = 35912,
        CompressedSrgbAlpha = 35913,
        CompressedSluminance = 35914,
        CompressedSluminanceAlpha = 35915,
        CompressedSrgbS3tcDxt1Ext = 35916,
        CompressedSrgbAlphaS3tcDxt1Ext = 35917,
        CompressedSrgbAlphaS3tcDxt3Ext = 35918,
        CompressedSrgbAlphaS3tcDxt5Ext = 35919,
        DepthComponent32f = 36012,
        Depth32fStencil8 = 36013,
        Rgba32ui = 36208,
        Rgb32ui = 36209,
        Rgba16ui = 36214,
        Rgb16ui = 36215,
        Rgba8ui = 36220,
        Rgb8ui = 36221,
        Rgba32i = 36226,
        Rgb32i = 36227,
        Rgba16i = 36232,
        Rgb16i = 36233,
        Rgba8i = 36238,
        Rgb8i = 36239,
        Float32UnsignedInt248Rev = 36269,
        CompressedRedRgtc1 = 36283,
        CompressedSignedRedRgtc1 = 36284,
        CompressedRgRgtc2 = 36285,
        CompressedSignedRgRgtc2 = 36286,
        CompressedRgbaBptcUnorm = 36492,
        CompressedSrgbAlphaBptcUnorm = 36493,
        CompressedRgbBptcSignedFloat = 36494,
        CompressedRgbBptcUnsignedFloat = 36495,
        R8Snorm = 36756,
        Rg8Snorm = 36757,
        Rgb8Snorm = 36758,
        Rgba8Snorm = 36759,
        R16Snorm = 36760,
        Rg16Snorm = 36761,
        Rgb16Snorm = 36762,
        Rgba16Snorm = 36763,
        Rgb10A2ui = 36975
    }
    public enum ETexWrapMode
    {
        /// <summary>
        /// Out-of-range image coordinates will return the border color.
        /// Same as ClampToBorder.
        /// </summary>
        Clamp = 10496,
        /// <summary>
        /// Out-of-range image coordinates are remapped back into range.
        /// </summary>
        Repeat = 10497,
        /// <summary>
        ///  Out-of-range image coordinates will return the border color.
        ///  Same as Clamp.
        /// </summary>
        ClampToBorder = 33069,
        /// <summary>
        /// Out-of-range image coordinates are clamped to the extent of the image.
        /// The border color is not sampled.
        /// </summary>
        ClampToEdge = 33071,
        /// <summary>
        /// Out-of-range image coordinates are remapped back into range.
        /// Every repetition is reversed.
        /// </summary>
        MirroredRepeat = 33648
    }
    public enum ETexMinFilter
    {
        Nearest = 9728,
        Linear = 9729,
        NearestMipmapNearest = 9984,
        LinearMipmapNearest = 9985,
        NearestMipmapLinear = 9986,
        LinearMipmapLinear = 9987,
        Filter4Sgis = 33094,
        LinearClipmapLinearSgix = 33136,
        PixelTexGenQCeilingSgix = 33156,
        PixelTexGenQRoundSgix = 33157,
        PixelTexGenQFloorSgix = 33158,
        NearestClipmapNearestSgix = 33869,
        NearestClipmapLinearSgix = 33870,
        LinearClipmapNearestSgix = 33871
    }
    public enum ETexMagFilter
    {
        Nearest = 9728,
        Linear = 9729,
        LinearDetailSgis = 32919,
        LinearDetailAlphaSgis = 32920,
        LinearDetailColorSgis = 32921,
        LinearSharpenSgis = 32941,
        LinearSharpenAlphaSgis = 32942,
        LinearSharpenColorSgis = 32943,
        Filter4Sgis = 33094,
        PixelTexGenQCeilingSgix = 33156,
        PixelTexGenQRoundSgix = 33157,
        PixelTexGenQFloorSgix = 33158
    }
    public enum ETexParamName
    {
        TextureBorderColor = 4100,
        TextureMagFilter = 10240,
        TextureMinFilter = 10241,
        TextureWrapS = 10242,
        TextureWrapT = 10243,
        TexturePriority = 32870,
        TexturePriorityExt = 32870,
        TextureDepth = 32881,
        TextureWrapR = 32882,
        TextureWrapRExt = 32882,
        TextureWrapROes = 32882,
        DetailTextureLevelSgis = 32922,
        DetailTextureModeSgis = 32923,
        ShadowAmbientSgix = 32959,
        TextureCompareFailValue = 32959,
        DualTextureSelectSgis = 33060,
        QuadTextureSelectSgis = 33061,
        ClampToBorder = 33069,
        ClampToEdge = 33071,
        TextureWrapQSgis = 33079,
        TextureMinLod = 33082,
        TextureMaxLod = 33083,
        TextureBaseLevel = 33084,
        TextureMaxLevel = 33085,
        TextureClipmapCenterSgix = 33137,
        TextureClipmapFrameSgix = 33138,
        TextureClipmapOffsetSgix = 33139,
        TextureClipmapVirtualDepthSgix = 33140,
        TextureClipmapLodOffsetSgix = 33141,
        TextureClipmapDepthSgix = 33142,
        PostTextureFilterBiasSgix = 33145,
        PostTextureFilterScaleSgix = 33146,
        TextureLodBiasSSgix = 33166,
        TextureLodBiasTSgix = 33167,
        TextureLodBiasRSgix = 33168,
        GenerateMipmap = 33169,
        GenerateMipmapSgis = 33169,
        TextureCompareSgix = 33178,
        TextureMaxClampSSgix = 33641,
        TextureMaxClampTSgix = 33642,
        TextureMaxClampRSgix = 33643,
        TextureLodBias = 34049,
        DepthTextureMode = 34891,
        TextureCompareMode = 34892,
        TextureCompareFunc = 34893,
        TextureSwizzleR = 36418,
        TextureSwizzleG = 36419,
        TextureSwizzleB = 36420,
        TextureSwizzleA = 36421,
        TextureSwizzleRgba = 36422,
        DepthStencilTextureMode = 37098,
    }
    public enum EBlitFramebufferFilter
    {
        Nearest,
        Linear
    }
    [Flags]
    public enum EClearBufferMask
    {
        None                = 0b00000,
        DepthBufferBit      = 0b00001,
        AccumBufferBit      = 0b00010,
        StencilBufferBit    = 0b00100,
        ColorBufferBit      = 0b01000,
        CoverageBufferBitNv = 0b10000,
    }
    public enum EGetQueryObject
    {
        QueryTarget = 33514,
        QueryResult = 34918,
        QueryResultAvailable = 34919,
        QueryResultNoWait = 37268
    }
    public enum EQueryTarget
    {
        /// <summary>
        /// Result returns how many nanoseconds have passed since the query was begun.
        /// </summary>
        TimeElapsed = 35007,
        /// <summary>
        /// Result returns how many samples passed the depth test.
        /// </summary>
        SamplesPassed = 35092,
        /// <summary>
        /// Result returns true if any samples passed the depth test.
        /// </summary>
        AnySamplesPassed = 35887,
        /// <summary>
        /// Returns how many elements (points, lines, triangles) were output by any programs that were rendered. 
        /// Includes elements generated by geometry and tessellation stages.
        /// </summary>
        PrimitivesGenerated = 35975,
        /// <summary>
        /// 
        /// </summary>
        TransformFeedbackPrimitivesWritten = 35976,
        /// <summary>
        /// Result returns true if any samples passed the depth test. Less accurate than <see cref="AnySamplesPassed"/> but faster.
        /// </summary>
        AnySamplesPassedConservative = 36202,
        /// <summary>
        /// Query result returns an arbitrary absolute time that is always increasing, measured as nanoseconds.
        /// </summary>
        Timestamp = 36392
    }
    public enum ETexTarget2D
    {
        Texture2D = 3553,
        ProxyTexture2D = 32868,
        TextureRectangle = 34037,
        ProxyTextureRectangle = 34039,
        TextureCubeMap = 34067,
        ProxyTextureCubeMap = 34075,
        Texture1DArray = 35864,
        ProxyTexture1DArray = 35865
    }
    public enum EWaitSyncStatus
    {
        /// <summary>
        /// The sync object was signaled before the function was called. 
        /// </summary>
        AlreadySignaled = 37146,
        /// <summary>
        /// The sync object did not signal within the given timeout period. 
        /// </summary>
        TimeoutExpired = 37147,
        /// <summary>
        /// The sync object was signaled within the given timeout period. 
        /// </summary>
        ConditionSatisfied = 37148,
        /// <summary>
        /// A GPU error occured.
        /// </summary>
        WaitFailed = 37149
    }
    public enum ETexTarget
    {
        Texture1D                       = 3552,
        Texture2D                       = 3553,
        ProxyTexture1D                  = 32867,
        ProxyTexture1DExt               = 32867,
        ProxyTexture2D                  = 32868,
        ProxyTexture2DExt               = 32868,
        Texture3D                       = 32879,
        Texture3DExt                    = 32879,
        Texture3DOes                    = 32879,
        ProxyTexture3D                  = 32880,
        ProxyTexture3DExt               = 32880,
        DetailTexture2DSgis             = 32917,
        Texture4DSgis                   = 33076,
        ProxyTexture4DSgis              = 33077,
        TextureMinLod                   = 33082,
        TextureMinLodSgis               = 33082,
        TextureMaxLod                   = 33083,
        TextureMaxLodSgis               = 33083,
        TextureBaseLevel                = 33084,
        TextureBaseLevelSgis            = 33084,
        TextureMaxLevel                 = 33085,
        TextureMaxLevelSgis             = 33085,
        TextureRectangle                = 34037,
        TextureRectangleArb             = 34037,
        TextureRectangleNv              = 34037,
        ProxyTextureRectangle           = 34039,
        TextureCubeMap                  = 34067,
        TextureBindingCubeMap           = 34068,
        TextureCubeMapPositiveX         = 34069,
        TextureCubeMapNegativeX         = 34070,
        TextureCubeMapPositiveY         = 34071,
        TextureCubeMapNegativeY         = 34072,
        TextureCubeMapPositiveZ         = 34073,
        TextureCubeMapNegativeZ         = 34074,
        ProxyTextureCubeMap             = 34075,
        Texture1DArray                  = 35864,
        ProxyTexture1DArray             = 35865,
        Texture2DArray                  = 35866,
        ProxyTexture2DArray             = 35867,
        TextureBuffer                   = 35882,
        TextureCubeMapArray             = 36873,
        ProxyTextureCubeMapArray        = 36875,
        Texture2DMultisample            = 37120,
        ProxyTexture2DMultisample       = 37121,
        Texture2DMultisampleArray       = 37122,
        ProxyTexture2DMultisampleArray  = 37123
    }
    public enum EStencilOp
    {
        Zero = 0,
        Invert = 5386,
        Keep = 7680,
        Replace = 7681,
        Incr = 7682,
        Decr = 7683,
        IncrWrap = 34055,
        DecrWrap = 34056
    }
    public enum EBufferRangeTarget
    {
        UniformBuffer = 35345,
        TransformFeedbackBuffer = 35982,
        ShaderStorageBuffer = 37074,
        AtomicCounterBuffer = 37568
    }
    public enum EFramebufferTarget
    {
        ReadFramebuffer = 36008,
        DrawFramebuffer = 36009,
        Framebuffer     = 36160,
    }
    public enum EDrawBuffersAttachment : ushort
    {
        None                = 0,
        FrontLeft           = 1024,
        FrontRight          = 1025,
        BackLeft            = 1026,
        BackRight           = 1027,
        Aux0                = 1033,
        Aux1                = 1034,
        Aux2                = 1035,
        Aux3                = 1036,
        ColorAttachment0    = 36064,
        ColorAttachment1    = 36065,
        ColorAttachment2    = 36066,
        ColorAttachment3    = 36067,
        ColorAttachment4    = 36068,
        ColorAttachment5    = 36069,
        ColorAttachment6    = 36070,
        ColorAttachment7    = 36071,
        ColorAttachment8    = 36072,
        ColorAttachment9    = 36073,
        ColorAttachment10   = 36074,
        ColorAttachment11   = 36075,
        ColorAttachment12   = 36076,
        ColorAttachment13   = 36077,
        ColorAttachment14   = 36078,
        ColorAttachment15   = 36079
    }
    public enum EFramebufferAttachment
    {
        FrontLeft               = 1024,
        FrontRight              = 1025,
        BackLeft                = 1026,
        BackRight               = 1027,
        Aux0                    = 1033,
        Aux1                    = 1034,
        Aux2                    = 1035,
        Aux3                    = 1036,
        Color                   = 6144,
        Depth                   = 6145,
        Stencil                 = 6146,
        DepthStencilAttachment  = 33306,
        ColorAttachment0        = 36064,
        ColorAttachment1        = 36065,
        ColorAttachment2        = 36066,
        ColorAttachment3        = 36067,
        ColorAttachment4        = 36068,
        ColorAttachment5        = 36069,
        ColorAttachment6        = 36070,
        ColorAttachment7        = 36071,
        ColorAttachment8        = 36072,
        ColorAttachment9        = 36073,
        ColorAttachment10       = 36074,
        ColorAttachment11       = 36075,
        ColorAttachment12       = 36076,
        ColorAttachment13       = 36077,
        ColorAttachment14       = 36078,
        ColorAttachment15       = 36079,
        DepthAttachment         = 36096,
        StencilAttachment       = 36128,
    }
    public enum EEnableCap
    {
        LineStipple,
        PolygonStipple,
        CullFace,
        Fog,
        DepthTest,
        StencilTest,
        AlphaTest,
        Dither,
        Blend,
        IndexLogicOp,
        ColorLogicOp,
        ScissorTest,
        AutoNormal,
        Map1Color4,
        Map1Index,
        Map1Normal,
        Map1TextureCoord1,
        Map1TextureCoord2,
        Map1TextureCoord3,
        Map1TextureCoord4,
        Map1Vertex3,
        Map1Vertex4,
        Map2Color4,
        Map2Index,
        Map2Normal,
        Map2TextureCoord1,
        Map2TextureCoord2,
        Map2TextureCoord3,
        Map2TextureCoord4,
        Map2Vertex3,
        Map2Vertex4,
        Texture1D,
        Texture2D,
        PolygonOffsetPoint,
        PolygonOffsetLine,
        ClipDistance0,
        ClipPlane0,
        ClipDistance1 = 12289,
        ClipPlane1 = 12289,
        ClipDistance2 = 12290,
        ClipPlane2 = 12290,
        ClipDistance3 = 12291,
        ClipPlane3 = 12291,
        ClipDistance4 = 12292,
        ClipPlane4 = 12292,
        ClipDistance5 = 12293,
        ClipPlane5 = 12293,
        ClipDistance6 = 12294,
        ClipDistance7 = 12295,
        Convolution1D = 32784,
        Convolution2D = 32785,
        Separable2D = 32786,
        Histogram = 32804,
        MinmaxExt = 32814,
        PolygonOffsetFill = 32823,
        RescaleNormal = 32826,
        RescaleNormalExt = 32826,
        Texture3DExt = 32879,
        VertexArray = 32884,
        NormalArray = 32885,
        ColorArray = 32886,
        IndexArray = 32887,
        TextureCoordArray = 32888,
        EdgeFlagArray = 32889,
        InterlaceSgix = 32916,
        Multisample = 32925,
        MultisampleSgis = 32925,
        SampleAlphaToCoverage = 32926,
        SampleAlphaToMaskSgis = 32926,
        SampleAlphaToOne = 32927,
        SampleAlphaToOneSgis = 32927,
        SampleCoverage = 32928,
        SampleMaskSgis = 32928,
        TextureColorTableSgi = 32956,
        ColorTable = 32976,
        ColorTableSgi = 32976,
        PostConvolutionColorTable = 32977,
        PostConvolutionColorTableSgi = 32977,
        PostColorMatrixColorTable = 32978,
        PostColorMatrixColorTableSgi = 32978,
        Texture4DSgis = 33076,
        PixelTexGenSgix = 33081,
        SpriteSgix = 33096,
        ReferencePlaneSgix = 33149,
        IrInstrument1Sgix = 33151,
        CalligraphicFragmentSgix = 33155,
        FramezoomSgix = 33163,
        FogOffsetSgix = 33176,
        SharedTexturePaletteExt = 33275,
        DebugOutputSynchronous = 33346,
        AsyncHistogramSgix = 33580,
        PixelTextureSgis = 33619,
        AsyncTexImageSgix = 33628,
        AsyncDrawPixelsSgix = 33629,
        AsyncReadPixelsSgix = 33630,
        FragmentLightingSgix = 33792,
        FragmentColorMaterialSgix = 33793,
        FragmentLight0Sgix = 33804,
        FragmentLight1Sgix = 33805,
        FragmentLight2Sgix = 33806,
        FragmentLight3Sgix = 33807,
        FragmentLight4Sgix = 33808,
        FragmentLight5Sgix = 33809,
        FragmentLight6Sgix,
        FragmentLight7Sgix,
        FogCoordArray,
        ColorSum,
        SecondaryColorArray,
        TextureRectangle,
        TextureCubeMap,
        ProgramPointSize,
        VertexProgramPointSize,
        VertexProgramTwoSide,
        DepthClamp,
        TextureCubeMapSeamless,
        PointSprite,
        SampleShading,
        RasterizerDiscard,
        PrimitiveRestartFixedIndex,
        FramebufferSrgb,
        SampleMask,
        PrimitiveRestart,
        DebugOutput
    }
}
