using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;

namespace TheraEngine.Rendering.Cameras
{
    public class PostSettings
    {
        private bool _enabled;

        public bool Enabled { get => _enabled; set => _enabled = value; }
    }
    public class PostProcessSettings : FileObject
    {
        public PostProcessSettings()
        {
            _vignetteSettings = new VignetteSettings();
            _colorGradeSettings = new ColorGradeSettings();
            _depthOfFieldSettings = new DepthOfFieldSettings();
            _bloomSettings = new BloomSettings();
            _lensFlareSettings = new LensFlareSettings();
            _antiAliasSettings = new AntiAliasSettings();
        }

        [Serialize("Vignette")]
        VignetteSettings _vignetteSettings;
        [Serialize("DOF")]
        DepthOfFieldSettings _depthOfFieldSettings;
        [Serialize("ColorGrade")]
        ColorGradeSettings _colorGradeSettings;
        [Serialize("Bloom")]
        BloomSettings _bloomSettings;
        [Serialize("Lens Flare")]
        LensFlareSettings _lensFlareSettings;
        [Serialize("Anti-Alias")]
        AntiAliasSettings _antiAliasSettings;
        
        public AntiAliasSettings AntiAliasSettings { get => _antiAliasSettings; set => _antiAliasSettings = value; }
        public LensFlareSettings LensFlareSettings { get => _lensFlareSettings; set => _lensFlareSettings = value; }
        public BloomSettings BloomSettings { get => _bloomSettings; set => _bloomSettings = value; }
        public ColorGradeSettings ColorGradeSettings { get => _colorGradeSettings; set => _colorGradeSettings = value; }
        public DepthOfFieldSettings DepthOfFieldSettings { get => _depthOfFieldSettings; set => _depthOfFieldSettings = value; }
        public VignetteSettings VignetteSettings { get => _vignetteSettings; set => _vignetteSettings = value; }

        internal void SetUniforms()
        {
            _vignetteSettings.SetUniforms();
            _depthOfFieldSettings.SetUniforms();
            _colorGradeSettings.SetUniforms();
            _bloomSettings.SetUniforms();
            _lensFlareSettings.SetUniforms();
        }

        internal static string ShaderSetup()
        {
            string vignette = VignetteSettings.WriteShaderSetup();
            string lensFlare = LensFlareSettings.WriteShaderSetup();
            string bloom = BloomSettings.WriteShaderSetup();
            string color = ColorGradeSettings.WriteShaderSetup();
            string dof = DepthOfFieldSettings.WriteShaderSetup();
            return
                "\n" + vignette +
                "\n" + lensFlare +
                "\n" + bloom +
                "\n" + color +
                "\n" + dof;
        }
    }
    public class VignetteSettings : PostSettings
    {
        private ColorF4 _color = System.Drawing.Color.Black;
        private float _intensity = 1.0f;

        public ColorF3 Color { get => _color; set => _color = value; }
        public float Intensity { get => _intensity; set => _intensity = value; }

        internal void SetUniforms()
        {
            Engine.Renderer.Uniform("Vignette.Color", Color);
            Engine.Renderer.Uniform("Vignette.Intensity", Intensity);
        }

        internal static string WriteShaderSetup()
        {
            return @"
struct VignetteStruct
{
    vec4 Color;
    float Intensity;
};
uniform VignetteStruct Vignette;";
        }
    }
    public class DepthOfFieldSettings : PostSettings
    {
        public float _nearDist, _farDist;

        internal void SetUniforms()
        {

        }

        internal static string WriteShaderSetup()
        {
            return @"";
        }
    }
    public class ColorGradeSettings : PostSettings
    {
        private ColorF3 _tint = new ColorF3(1.0f, 1.0f, 1.0f);
        private float _exposure = 1.0f;
        private float _saturation = 1.0f;
        private float _contrast = 1.0f;
        private float _gamma = 2.2f;

        internal void SetUniforms()
        {
            Engine.Renderer.Uniform("ColorGrade.Tint", _tint);
            Engine.Renderer.Uniform("ColorGrade.Exposure", _exposure);
            Engine.Renderer.Uniform("ColorGrade.Saturation", _saturation);
            Engine.Renderer.Uniform("ColorGrade.Contrast", _contrast);
            Engine.Renderer.Uniform("ColorGrade.Gamma", _gamma);
        }

        internal static string WriteShaderSetup()
        {
            return @"
struct ColorGradeStruct
{
    vec3 Tint;
    float Exposure;
    float Saturation;
    float Contrast;
    float Gamma;
};
uniform ColorGradeStruct ColorGrade;";
        }
    }
    public class BloomSettings : PostSettings
    {
        public float _intensity;

        internal void SetUniforms()
        {

        }

        internal static string WriteShaderSetup()
        {
            return @"";
        }
    }
    public class LensFlareSettings : PostSettings
    {
        public float _intensity;

        internal void SetUniforms()
        {

        }

        internal static string WriteShaderSetup()
        {
            return @"";
        }
    }
    public class AntiAliasSettings : PostSettings
    {
        public enum AntiAliasType
        {
            FXAA,
            SMAA,
            MSAA,
            TXAA,
        }
    }
}
