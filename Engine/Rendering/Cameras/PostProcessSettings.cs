using TheraEngine.Files;
using System;
using System.ComponentModel;

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

        [DisplayName("Anti-Alias Settings")]
        [Category("Post-Process Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AntiAliasSettings AntiAliasSettings { get => _antiAliasSettings; set => _antiAliasSettings = value; }
        [DisplayName("Lens Flare Settings")]
        [Category("Post-Process Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LensFlareSettings LensFlareSettings { get => _lensFlareSettings; set => _lensFlareSettings = value; }
        [DisplayName("Bloom Settings")]
        [Category("Post-Process Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BloomSettings BloomSettings { get => _bloomSettings; set => _bloomSettings = value; }
        [DisplayName("Color Grade Settings")]
        [Category("Post-Process Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ColorGradeSettings ColorGradeSettings { get => _colorGradeSettings; set => _colorGradeSettings = value; }
        [DisplayName("Depth Of Field Settings")]
        [Category("Post-Process Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DepthOfFieldSettings DepthOfFieldSettings { get => _depthOfFieldSettings; set => _depthOfFieldSettings = value; }
        [DisplayName("Vignette Settings")]
        [Category("Post-Process Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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

        public ColorF4 Color { get => _color; set => _color = value; }
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
        private EventColorF3 _tint = new ColorF3(1.0f, 1.0f, 1.0f);
        private float _exposure = 1.0f;
        private float _saturation = 1.0f;
        private float _contrast = 1.0f;
        private float _gamma = 2.2f;

        [Category("Color Grade Settings")]
        public EventColorF3 Tint { get => _tint; set => _tint = value; }
        [Category("Color Grade Settings")]
        public float Exposure { get => _exposure; set => _exposure = value; }
        [Category("Color Grade Settings")]
        public float Saturation { get => _saturation; set => _saturation = value; }
        [Category("Color Grade Settings")]
        public float Contrast { get => _contrast; set => _contrast = value; }
        [Category("Color Grade Settings")]
        public float Gamma { get => _gamma; set => _gamma = value; }

        internal void SetUniforms()
        {
            Engine.Renderer.Uniform("ColorGrade.Tint", Tint.Raw);
            Engine.Renderer.Uniform("ColorGrade.Exposure", Exposure);
            Engine.Renderer.Uniform("ColorGrade.Saturation", Saturation);
            Engine.Renderer.Uniform("ColorGrade.Contrast", Contrast);
            Engine.Renderer.Uniform("ColorGrade.Gamma", Gamma);
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
