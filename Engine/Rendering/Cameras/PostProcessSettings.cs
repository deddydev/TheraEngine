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

namespace TheraEngine.Rendering.Cameras
{
    public class PostSettings
    {
        public bool _enabled;
    }
    public class PostProcessSettings : FileObject
    {
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
        [Serialize("Exposure")]
        ExposureSettings _exposureSettings;

        public ExposureSettings ExposureSettings { get => _exposureSettings; set => _exposureSettings = value; }
        public AntiAliasSettings AntiAliasSettings { get => _antiAliasSettings; set => _antiAliasSettings = value; }
        public LensFlareSettings LensFlareSettings { get => _lensFlareSettings; set => _lensFlareSettings = value; }
        public BloomSettings BloomSettings { get => _bloomSettings; set => _bloomSettings = value; }
        public ColorGradeSettings ColorGradeSettings { get => _colorGradeSettings; set => _colorGradeSettings = value; }
        public DepthOfFieldSettings DepthOfFieldSettings { get => _depthOfFieldSettings; set => _depthOfFieldSettings = value; }
        public VignetteSettings VignetteSettings { get => _vignetteSettings; set => _vignetteSettings = value; }

        public static List<GLVar> GetParameterList()
        {
            List<GLVar> parameters = new List<GLVar>()
            {
                //new GLVec4((ColorF4)Color.Transparent, "Vignette.Color"),
                //new GLFloat(0.0f, "DOF.NearDistance"),
                //new GLFloat(0.0f, "DOF.FarDistance"),
            };
            return parameters;
        }
    }
    public class VignetteSettings
    {
        public ColorF4 _color;
    }
    public class DepthOfFieldSettings
    {
        public float _nearDist, _farDist;
    }
    public class ColorGradeSettings
    {
        public ColorF4 _tint;
    }
    public class BloomSettings
    {
        public float _intensity;
    }
    public class LensFlareSettings
    {
        public float _intensity;
    }
    public class AntiAliasSettings
    {
        public enum AntiAliasType
        {
            FXAA,
            SMAA,
        }
    }
    public class ExposureSettings
    {

    }
}
