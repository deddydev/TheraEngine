using TheraEngine.Files;
using System;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Cameras
{
    public class PostSettings
    {
        //private bool _enabled;

        //public bool Enabled { get => _enabled; set => _enabled = value; }
    }
    [FileExt("campost")]
    [FileDef("Camera Post-Processing Settings")]
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
            _ambientOcclusionSettings = new AmbientOcclusionSettings();
        }

        [TSerialize("Vignette")]
        VignetteSettings _vignetteSettings;
        [TSerialize("DOF")]
        DepthOfFieldSettings _depthOfFieldSettings;
        [TSerialize("ColorGrade")]
        ColorGradeSettings _colorGradeSettings;
        [TSerialize("Bloom")]
        BloomSettings _bloomSettings;
        [TSerialize("Lens Flare")]
        LensFlareSettings _lensFlareSettings;
        [TSerialize("Anti-Alias")]
        AntiAliasSettings _antiAliasSettings;
        [TSerialize("SSAO")]
        AmbientOcclusionSettings _ambientOcclusionSettings;

        [DisplayName("Anti-Aliasing")]
        [Category("Post-Process Settings")]
        public AntiAliasSettings AntiAliasing { get => _antiAliasSettings; set => _antiAliasSettings = value; }
        [Category("Post-Process Settings")]
        public LensFlareSettings LensFlare { get => _lensFlareSettings; set => _lensFlareSettings = value; }
        [Category("Post-Process Settings")]
        public BloomSettings Bloom { get => _bloomSettings; set => _bloomSettings = value; }
        [Category("Post-Process Settings")]
        public ColorGradeSettings ColorGrading { get => _colorGradeSettings; set => _colorGradeSettings = value; }
        [Category("Post-Process Settings")]
        public DepthOfFieldSettings DepthOfField { get => _depthOfFieldSettings; set => _depthOfFieldSettings = value; }
        [Category("Post-Process Settings")]
        public VignetteSettings Vignette { get => _vignetteSettings; set => _vignetteSettings = value; }
        [Category("Post-Process Settings")]
        public AmbientOcclusionSettings AmbientOcclusion { get => _ambientOcclusionSettings; set => _ambientOcclusionSettings = value; }
        
        internal void SetUniforms(int programBindingId)
        {
            _vignetteSettings.SetUniforms(programBindingId);
            _depthOfFieldSettings.SetUniforms(programBindingId);
            _colorGradeSettings.SetUniforms(programBindingId);
            _bloomSettings.SetUniforms(programBindingId);
            _lensFlareSettings.SetUniforms(programBindingId);
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
    public class AmbientOcclusionSettings : PostSettings
    {
        private float _radius = 0.75f;
        private float _power = 4.0f;

        //[DragRange(0.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Ambient Occlusion Settings")]
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }

        //[DragRange(0.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Ambient Occlusion Settings")]
        public float Power
        {
            get => _power;
            set => _power = value;
        }

        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "SSAORadius", Radius);
            Engine.Renderer.Uniform(programBindingId, "SSAOPower", Power);
        }
    }
    public class VignetteSettings : PostSettings
    {
        private ColorF3 _color = new ColorF3(0.0f, 0.0f, 0.0f);
        private float _intensity = 15.0f;
        private float _power = 0.0f;

        [Category("Vignette Settings")]
        public ColorF3 Color { get => _color; set => _color = value; }

        //[DragRange(0.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Vignette Settings")]
        public float Intensity { get => _intensity; set => _intensity = value; }

        //[DragRange(0.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Vignette Settings")]
        public float Power { get => _power; set => _power = value; }
        
        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "Vignette.Color", Color);
            Engine.Renderer.Uniform(programBindingId, "Vignette.Intensity", Intensity);
            Engine.Renderer.Uniform(programBindingId, "Vignette.Power", Power);
        }

        internal static string WriteShaderSetup()
        {
            return @"
struct VignetteStruct
{
    vec3 Color;
    float Intensity;
    float Power;
};
uniform VignetteStruct Vignette;";
        }
    }
    public class DepthOfFieldSettings : PostSettings
    {
        public float _nearDist, _farDist;

        internal void SetUniforms(int programBindingId)
        {

        }

        internal static string WriteShaderSetup()
        {
            return @"";
        }
    }
    public class ColorGradeSettings : PostSettings
    {
        public ColorGradeSettings()
        {
            Contrast = 0.0f;
        }

        private EventColorF3 _tint = new ColorF3(1.0f, 1.0f, 1.0f);

        private float _exposure = 1.0f;
        private float _contrast;
        private float _gamma = 2.2f;

        private float _hue = 1.0f;
        private float _saturation = 1.0f;
        private float _brightness = 1.0f;

        private float _contrastUniformValue;

        [Category("Color Grade Settings")]
        public EventColorF3 Tint { get => _tint; set => _tint = value; }

        //[DragRange(0.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Color Grade Settings")]
        public float Exposure { get => _exposure; set => _exposure = value; }

        //[DragRange(-100.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Color Grade Settings")]
        public float Contrast
        {
            get => _contrast;
            set
            {
                _contrast = value;
                _contrastUniformValue = (100.0f + _contrast) / 100.0f;
                _contrastUniformValue *= _contrastUniformValue;
            }
        }

        //[DragRange(0.0f, 50.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Color Grade Settings")]
        public float Gamma { get => _gamma; set => _gamma = value; }

        //[DragRange(0.0f, 1.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Color Grade Settings")]
        public float Hue { get => _hue; set => _hue = value; }

        //[DragRange(0.0f, 2.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Color Grade Settings")]
        public float Saturation { get => _saturation; set => _saturation = value; }

        //[DragRange(0.0f, 100.0f)]
        //[Editor(typeof(FloatDragEditor), typeof(UITypeEditor))]
        [Category("Color Grade Settings")]
        public float Brightness { get => _brightness; set => _brightness = value; }

        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Tint", Tint.Raw);

            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Exposure", Exposure);
            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Contrast", _contrastUniformValue);
            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Gamma", Gamma);

            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Hue", Hue);
            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Saturation", Saturation);
            Engine.Renderer.Uniform(programBindingId, "ColorGrade.Brightness", Brightness);
        }

        internal static string WriteShaderSetup()
        {
            return @"
struct ColorGradeStruct
{
    vec3 Tint;

    float Exposure;
    float Contrast;
    float Gamma;

    float Hue;
    float Saturation;
    float Brightness;
};
uniform ColorGradeStruct ColorGrade;";
        }
    }
    public class BloomSettings : PostSettings
    {
        public float _intensity;

        internal void SetUniforms(int programBindingId)
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

        internal void SetUniforms(int programBindingId)
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
