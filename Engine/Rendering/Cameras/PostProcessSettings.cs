using TheraEngine.Core.Files;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Cameras
{
    public class PostSettings
    {
        //private bool _enabled;

        //public bool Enabled { get => _enabled; set => _enabled = value; }
    }
    [TFileExt("campost")]
    [TFileDef("Camera Post-Processing Settings")]
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

        internal void SetUniforms(RenderProgram program)
        {
            Vignette.SetUniforms(program);
            DepthOfField.SetUniforms(program);
            ColorGrading.SetUniforms(program);
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

        public void Lerp(PostProcessSettings from, PostProcessSettings to, float time)
        {
            ColorGrading.Lerp(from.ColorGrading, to.ColorGrading, time);
            AmbientOcclusion.Lerp(from.AmbientOcclusion, to.AmbientOcclusion, time);
            Bloom.Lerp(from.Bloom, to.Bloom, time);
            Vignette.Lerp(from.Vignette, to.Vignette, time);
            DepthOfField.Lerp(from.DepthOfField, to.DepthOfField, time);
        }
    }
    public class AmbientOcclusionSettings : PostSettings
    {
        [TSerialize]
        [Category("Ambient Occlusion Settings")]
        public float Radius { get; set; } = 1.75f;
        
        [TSerialize]
        [Category("Ambient Occlusion Settings")]
        public float Power { get; set; } = 2.0f;

        public void Lerp(AmbientOcclusionSettings from, AmbientOcclusionSettings to, float time)
        {
            Radius = Interp.Lerp(from.Radius, to.Radius, time);
            Power = Interp.Lerp(from.Power, to.Power, time);
        }

        internal void SetUniforms(RenderProgram program)
        {
            program.Uniform("Radius", Radius);
            program.Uniform("Power", Power);
        }
    }
    public class VignetteSettings : PostSettings
    {
        public const string VignetteUniformName = "Vignette";

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
            program.Uniform($"{VignetteUniformName}.{nameof(Color)}", Color);
            program.Uniform($"{VignetteUniformName}.{nameof(Intensity)}", Intensity);
            program.Uniform($"{VignetteUniformName}.{nameof(Power)}", Power);
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

        public void Lerp(VignetteSettings from, VignetteSettings to, float time)
        {
            Color = Interp.Lerp(from.Color, to.Color, time);
            Intensity = Interp.Lerp(from.Intensity, to.Intensity, time);
            Power = Interp.Lerp(from.Power, to.Power, time);
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

        internal void Lerp(DepthOfFieldSettings depthOfField1, DepthOfFieldSettings depthOfField2, float time) => throw new NotImplementedException();
    }
    public class ColorGradeSettings : PostSettings
    {
        public const string ColorGradeUniformName = "ColorGrade";

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

        /// <summary>
        /// If <see langword="true"/>, adjusts the exposure value depending on the scene's average luminosity.
        /// </summary>
        [TSerialize]
        [Category("Color Grade Settings")]
        public bool AutoExposure { get; set; } = true;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float AutoExposureBias { get; set; } = 0.0f;
        [TSerialize]
        [Category("Color Grade Settings")]
        public float AutoExposureScale { get; set; } = 1.0f;
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
            program.Uniform($"{ColorGradeUniformName}.{nameof(Tint)}", Tint.Raw);
            program.Uniform($"{ColorGradeUniformName}.{nameof(Exposure)}", Exposure);
            program.Uniform($"{ColorGradeUniformName}.{nameof(Contrast)}", _contrastUniformValue);
            program.Uniform($"{ColorGradeUniformName}.{nameof(Gamma)}", Gamma);
            program.Uniform($"{ColorGradeUniformName}.{nameof(Hue)}", Hue);
            program.Uniform($"{ColorGradeUniformName}.{nameof(Saturation)}", Saturation);
            program.Uniform($"{ColorGradeUniformName}.{nameof(Brightness)}", Brightness);
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

        [Browsable(false)]
        public bool AllowsAutoExposure => AutoExposure || Exposure < MinExposure || Exposure > MaxExposure;
        
        public unsafe void UpdateExposure(TexRef2D hdrSceneTexture)
        {
            if (!AllowsAutoExposure)
                return;
            
            //Calculate average color value using 1x1 mipmap of scene
            var tex = hdrSceneTexture.RenderTextureGeneric;
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
            float lumDot = rgb.Dot(Vec3.Luminance);

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

            float exposure = ExposureDividend / lumDot;
            exposure = AutoExposureBias + AutoExposureScale * exposure;
            exposure = exposure.Clamp(MinExposure, MaxExposure);

            //If the current exposure is an invalid value, that means we want the exposure to be set immediately.
            if (Exposure < MinExposure || Exposure > MaxExposure)
                Exposure = exposure;
            else
                Exposure = Interp.Lerp(Exposure, exposure, ExposureTransitionSpeed);
        }

        internal void Lerp(ColorGradeSettings colorGrading1, ColorGradeSettings colorGrading2, float time) => throw new NotImplementedException();
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

        internal void Lerp(BloomSettings bloom1, BloomSettings bloom2, float time) => throw new NotImplementedException();
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
        public enum EAntiAliasType
        {
            FXAA,
            SMAA,
            MSAA,
            TXAA,
        }
    }
}
