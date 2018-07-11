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
        [Category("Post-Process Settings")]
        [TSerialize("Shadows")]
        public ShadowSettings Shadows { get; set; }

        internal void SetUniforms(RenderProgram program)
        {
            Vignette.SetUniforms(program);
            DepthOfField.SetUniforms(program);
            ColorGrading.SetUniforms(program);
            Bloom.SetUniforms(program);
            LensFlare.SetUniforms(program);
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
        public float ShadowBase { get; set; } = 2.0f;
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowMult { get; set; } = 6.0f;
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowBiasMin { get; set; } = 0.00001f;
        [TSerialize]
        [Category("Shadow Map Settings")]
        public float ShadowBiasMax { get; set; } = 0.004f;

        internal void SetUniforms(RenderProgram program)
        {
            program.Uniform("ShadowBase", ShadowBase);
            program.Uniform("ShadowMult", ShadowMult);
            program.Uniform("ShadowBiasMin", ShadowBiasMin);
            program.Uniform("ShadowBiasMax", ShadowBiasMax);
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
        
        internal void SetUniforms(RenderProgram program)
        {
            program.Uniform("Radius", Radius);
            program.Uniform("Power", Power);
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

        internal void SetUniforms(RenderProgram program)
        {
            program.Uniform("Vignette.Color", Color);
            program.Uniform("Vignette.Intensity", Intensity);
            program.Uniform("Vignette.Power", Power);
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
        internal void SetUniforms(RenderProgram program)
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
        public float ExposureDividend { get; set; } = 0.5f;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float MinExposure { get; set; } = 0.01f;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float MaxExposure { get; set; } = 500.0f;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float ExposureTransitionSpeed
        {
            get => _exposureTransitionSpeed;
            set => _exposureTransitionSpeed = value.Clamp(0.0f, 1.0f);
        }
        [TSerialize]
        [Category("Color Grade Settings")]
        public float Exposure { get; set; } = -1.0f;

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

        internal void SetUniforms(RenderProgram program)
        {
            program.Uniform("ColorGrade.Tint", Tint.Raw);
            program.Uniform("ColorGrade.Exposure", Exposure);
            program.Uniform("ColorGrade.Contrast", _contrastUniformValue);
            program.Uniform("ColorGrade.Gamma", Gamma);
            program.Uniform("ColorGrade.Hue", Hue);
            program.Uniform("ColorGrade.Saturation", Saturation);
            program.Uniform("ColorGrade.Brightness", Brightness);
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
        public unsafe void UpdateExposure(TexRef2D hdrSceneTexture)
        {
            if (!AutoExposure && Exposure >= MinExposure && Exposure <= MaxExposure)
                return;

            //Calculate average color value using 1x1 mipmap of scene
            var tex = hdrSceneTexture.GetRenderTextureGeneric(true);
            tex.Bind();
            tex.GenerateMipmaps();

            //Get the average color from the scene texture
            Vec3 rgb = new Vec3();
            IntPtr addr = (IntPtr)rgb.Data;
            Engine.Renderer.GetTexImage(tex.BindingId, tex.SmallestMipmapLevel, EPixelFormat.Rgb, EPixelType.Float, sizeof(Vec3), addr);

            if (float.IsNaN(rgb.X)) return;
            if (float.IsNaN(rgb.Y)) return;
            if (float.IsNaN(rgb.Z)) return;

            //Calculate luminance factor off of the average color
            float lumDot = rgb.Dot(_luminance);

            //If the dot factor is zero, this means the screen is perfectly black.
            //Usually that means nothing is being rendered, so don't update the exposure now.
            //If we were to update the exposure now, the scene would look very bright once it finally starts rendering.
            if (lumDot <= 0.0f)
            {
                //if (Exposure < MinExposure)
                //    Exposure = MinExposure;
                //if (Exposure > MaxExposure)
                //    Exposure = MaxExposure;
                return;
            }

            float target = (ExposureDividend / lumDot).Clamp(MinExposure, MaxExposure);

            //If the current exposure is an invalid value, that means we want the exposure to be set immediately.
            if (Exposure < MinExposure || Exposure > MaxExposure)
                Exposure = target;
            else
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

        internal void SetUniforms(RenderProgram program)
        {
            program.Uniform("BloomIntensity", Intensity);
            program.Uniform("BloomThreshold", Threshold);
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
        internal void SetUniforms(RenderProgram program)
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
