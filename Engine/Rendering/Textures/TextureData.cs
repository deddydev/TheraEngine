﻿using CustomEngine.Files;
using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Textures
{
    public class TextureData : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.Texture; } }

        public Bitmap Bitmap { get { return _bitmap; } }

        Bitmap _bitmap;

        public TextureData(string path)
        {
            _bitmap = File.Exists(path) ? new FreeImageBitmap(path).ToBitmap() : null;
        }
        public TextureData(int width, int height)
        {
            _bitmap = new Bitmap(width, height);
        }
    }
    public enum PixelFormat
    {
        UnsignedShort,          //
        UnsignedInt,            //
        ColorIndex,             //
        StencilIndex,           //
        DepthComponent,         //
        Red,                    //
        Green,                  //
        Blue,                   //
        Alpha,                  //
        Rgb,                    //
        Rgba,                   //
        Luminance,              //
        LuminanceAlpha,         //
        Bgr,                    //
        Bgra,                   //
        Rg,                     //
        RgInteger,              //
        DepthStencil,           //
        RedInteger,             //
        GreenInteger,           //
        BlueInteger,            //
        AlphaInteger,           //
        RgbInteger,             //
        RgbaInteger,            //
        BgrInteger,             //
        BgraInteger             //
    }
    public enum PixelType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        Int = 5124,
        UnsignedInt = 5125,
        Float = 5126,
        HalfFloat = 5131,
        Bitmap = 6656,
        UnsignedByte332 = 32818,
        UnsignedByte332Ext = 32818,
        UnsignedShort4444 = 32819,
        UnsignedShort4444Ext = 32819,
        UnsignedShort5551 = 32820,
        UnsignedShort5551Ext = 32820,
        UnsignedInt8888 = 32821,
        UnsignedInt8888Ext = 32821,
        UnsignedInt1010102 = 32822,
        UnsignedInt1010102Ext = 32822,
        UnsignedByte233Reversed = 33634,
        UnsignedShort565 = 33635,
        UnsignedShort565Reversed = 33636,
        UnsignedShort4444Reversed = 33637,
        UnsignedShort1555Reversed = 33638,
        UnsignedInt8888Reversed = 33639,
        UnsignedInt2101010Reversed = 33640,
        UnsignedInt248 = 34042,
        UnsignedInt10F11F11FRev = 35899,
        UnsignedInt5999Rev = 35902,
        Float32UnsignedInt248Rev = 36269
    }
    public enum PixelInternalFormat
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
        DepthComponent16Sgix = 33189,
        DepthComponent24 = 33190,
        DepthComponent24Sgix = 33190,
        DepthComponent32 = 33191,
        DepthComponent32Sgix = 33191,
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
    public enum TextureTarget
    {
        Texture1D = 3552,
        Texture2D = 3553,
        ProxyTexture1D = 32867,
        ProxyTexture1DExt = 32867,
        ProxyTexture2D = 32868,
        ProxyTexture2DExt = 32868,
        Texture3D = 32879,
        Texture3DExt = 32879,
        Texture3DOes = 32879,
        ProxyTexture3D = 32880,
        ProxyTexture3DExt = 32880,
        DetailTexture2DSgis = 32917,
        Texture4DSgis = 33076,
        ProxyTexture4DSgis = 33077,
        TextureMinLod = 33082,
        TextureMinLodSgis = 33082,
        TextureMaxLod = 33083,
        TextureMaxLodSgis = 33083,
        TextureBaseLevel = 33084,
        TextureBaseLevelSgis = 33084,
        TextureMaxLevel = 33085,
        TextureMaxLevelSgis = 33085,
        TextureRectangle = 34037,
        TextureRectangleArb = 34037,
        TextureRectangleNv = 34037,
        ProxyTextureRectangle = 34039,
        TextureCubeMap = 34067,
        TextureBindingCubeMap = 34068,
        TextureCubeMapPositiveX = 34069,
        TextureCubeMapNegativeX = 34070,
        TextureCubeMapPositiveY = 34071,
        TextureCubeMapNegativeY = 34072,
        TextureCubeMapPositiveZ = 34073,
        TextureCubeMapNegativeZ = 34074,
        ProxyTextureCubeMap = 34075,
        Texture1DArray = 35864,
        ProxyTexture1DArray = 35865,
        Texture2DArray = 35866,
        ProxyTexture2DArray = 35867,
        TextureBuffer = 35882,
        TextureCubeMapArray = 36873,
        ProxyTextureCubeMapArray = 36875,
        Texture2DMultisample = 37120,
        ProxyTexture2DMultisample = 37121,
        Texture2DMultisampleArray = 37122,
        ProxyTexture2DMultisampleArray = 37123
    }
}
