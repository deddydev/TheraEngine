using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Cameras
{
    public class PostSettings
    {
        public bool _enabled;
    }
    public class PostProcessSettings
    {
        VignetteSettings _vignetteSettings;
        DepthOfFieldSettings _depthOfFieldSettings;
        ColorGradeSettings _colorGradeSettings;
        BloomSettings _bloomSettings;
        LensFlareSettings _lensFlareSettings;
        AntiAliasSettings _antiAliasSettings;
        ExposureSettings _exposureSettings;
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
