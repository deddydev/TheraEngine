using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Maths;

namespace TheraEngine.Rendering.Cameras
{
    public class PostSettings
    {
        //private bool _enabled;

        //public bool Enabled { get => _enabled; set => _enabled = value; }
    }
    [FileExt("campost")]
    [FileDef("Camera Post-Processing Settings")]
    public class PostProcessSettings : TFileObject
    {
        public PostProcessSettings()
        {
            Vignette = new VignetteSettings();
            ColorGrading = new ColorGradeSettings();
            DepthOfField = new DepthOfFieldSettings();
            Bloom = new BloomSettings();
            LensFlare = new LensFlareSettings();
            AntiAliasing = new AntiAliasSettings();
            AmbientOcclusion = new AmbientOcclusionSettings();
            Shadows = new ShadowSettings();
        }
        
        [TSerialize("AntiAliasing")]
        [DisplayName("Anti-Aliasing")]
        [Category("Post-Process Settings")]
        public AntiAliasSettings AntiAliasing { get; set; }
        [TSerialize("LensFlare")]
        [Category("Post-Process Settings")]
        public LensFlareSettings LensFlare { get; set; }
        [TSerialize("Bloom")]
        [Category("Post-Process Settings")]
        public BloomSettings Bloom { get; set; }
        [TSerialize("ColorGrade")]
        [Category("Post-Process Settings")]
        public ColorGradeSettings ColorGrading { get; set; }
        [TSerialize("DOF")]
        [Category("Post-Process Settings")]
        public DepthOfFieldSettings DepthOfField { get; set; }
        [TSerialize("Vignette")]
        [Category("Post-Process Settings")]
        public VignetteSettings Vignette { get; set; }
        [TSerialize("SSAO")]
        [Category("Post-Process Settings")]
        public AmbientOcclusionSettings AmbientOcclusion { get; set; }
        [Category("Post-Process Settings")]
        [TSerialize]
        public GlobalFileRef<TMaterial> PostProcessMaterial { get; set; }
        [Category("Shadow Settings")]
        [TSerialize]
        public ShadowSettings Shadows { get; set; }

        internal void SetUniforms(int programBindingId)
        {
            Vignette.SetUniforms(programBindingId);
            DepthOfField.SetUniforms(programBindingId);
            ColorGrading.SetUniforms(programBindingId);
            Bloom.SetUniforms(programBindingId);
            LensFlare.SetUniforms(programBindingId);
        }

        internal static string ShaderSetup()
        {
            string vignette = VignetteSettings.WriteShaderSetup();
            string lensFlare = LensFlareSettings.WriteShaderSetup();
            string bloom = BloomSettings.WriteShaderSetup();
            string color = ColorGradeSettings.WriteShaderSetup();
            string dof = DepthOfFieldSettings.WriteShaderSetup();
            return
                Environment.NewLine + vignette +
                Environment.NewLine + lensFlare +
                Environment.NewLine + bloom +
                Environment.NewLine + color +
                Environment.NewLine + dof;
        }
    }
    public class ShadowSettings : PostSettings
    {
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowBase { get; set; } = 3.0f;
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowMult { get; set; } = 6.0f;
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowBiasMin { get; set; } = 0.00001f;
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowBiasMax { get; set; } = 0.004f;

        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "ShadowBase", ShadowBase);
            Engine.Renderer.Uniform(programBindingId, "ShadowMult", ShadowMult);
            Engine.Renderer.Uniform(programBindingId, "ShadowBiasMin", ShadowBiasMin);
            Engine.Renderer.Uniform(programBindingId, "ShadowBiasMax", ShadowBiasMax);
        }
    }
    public class AmbientOcclusionSettings : PostSettings
    {
        [TSerialize]
        [Category("Ambient Occlusion Settings")]
        public float Radius { get; set; } = 0.75f;
        
        [TSerialize]
        [Category("Ambient Occlusion Settings")]
        public float Power { get; set; } = 4.0f;
        
        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "Radius", Radius);
            Engine.Renderer.Uniform(programBindingId, "Power", Power);
        }
    }
    public class VignetteSettings : PostSettings
    {
        [TSerialize]
        [Category("Vignette Settings")]
        public ColorF3 Color { get; set; } = new ColorF3(0.0f, 0.0f, 0.0f);
        
        [TSerialize]
        [Category("Vignette Settings")]
        public float Intensity { get; set; } = 15.0f;
        
        [TSerialize]
        [Category("Vignette Settings")]
        public float Power { get; set; } = 0.0f;

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
        
        private float _contrast;
        private float _contrastUniformValue;
        private float _exposureTransitionSpeed = 0.05f;

        [TSerialize]
        [Category("Color Grade Settings")]
        public EventColorF3 Tint { get; set; } = new ColorF3(1.0f, 1.0f, 1.0f);

        [TSerialize]
        [Category("Color Grade Settings")]
        public bool AutoExposure { get; set; } = true;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float MinExposure { get; set; } = 0.01f;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float MaxExposure { get; set; } = 50.0f;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float ExposureTransitionSpeed
        {
            get => _exposureTransitionSpeed;
            set => _exposureTransitionSpeed = value.Clamp(0.0f, 1.0f);
        }
        [TSerialize]
        [Category("Color Grade Settings")]
        public float Exposure { get; set; } = 0.0f;

        [TSerialize]
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

        [TSerialize]
        [Category("Color Grade Settings")]
        public float Gamma { get; set; } = 2.2f;

        [TSerialize]
        [Category("Color Grade Settings")]
        public float Hue { get; set; } = 1.0f;

        [TSerialize]
        [Category("Color Grade Settings")]
        public float Saturation { get; set; } = 1.0f;
        
        [TSerialize]
        [Category("Color Grade Settings")]
        public float Brightness { get; set; } = 1.0f;

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
        
        private Vec3 _luminance = new Vec3(0.299f, 0.587f, 0.114f);
        private Half[] _rgb = new Half[3];
        public void UpdateExposure(TexRef2D hdrSceneTexture)
        {
            if (!AutoExposure)
                return;

            var tex = hdrSceneTexture.GetTextureGeneric(true);
            Engine.Renderer.SetActiveTexture(0);
            tex.Bind();
            tex.GenerateMipmaps();
            Engine.Renderer.GetTexImage(ETexTarget.Texture2D, tex.SmallestMipmapLevel, tex.PixelFormat, tex.PixelType, _rgb);
            Vec3 rgb = new Vec3(_rgb[0], _rgb[1], _rgb[2]);
            if (float.IsNaN(rgb.X)) return;
            if (float.IsNaN(rgb.Y)) return;
            if (float.IsNaN(rgb.Z)) return;
            float target = (0.5f / rgb.Dot(_luminance)).Clamp(MinExposure, MaxExposure);
            Exposure = Interp.Lerp(Exposure, target, ExposureTransitionSpeed);
        }
    }
    public class BloomSettings : PostSettings
    {
        [TSerialize]
        [Category("Bloom Settings")]
        public float Intensity { get; set; } = 1.0f;
        [TSerialize]
        [Category("Bloom Settings")]
        public float Threshold { get; set; } = 1.0f;

        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "BloomIntensity", Intensity);
            Engine.Renderer.Uniform(programBindingId, "BloomThreshold", Threshold);
        }

        internal static string WriteShaderSetup()
        {
            return @"
uniform float BloomIntensity = 1.0f;
uniform float BloomThreshold = 1.0f;";
        }
    }
    public class LensFlareSettings : PostSettings
    {
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
