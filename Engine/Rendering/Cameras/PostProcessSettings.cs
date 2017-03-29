using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomEngine.Rendering.Cameras
{
    public class PostSettings
    {
        public bool _enabled;
    }
    public class PostProcessSettings : FileObject
    {
        VignetteSettings _vignetteSettings;
        DepthOfFieldSettings _depthOfFieldSettings;
        ColorGradeSettings _colorGradeSettings;
        BloomSettings _bloomSettings;
        LensFlareSettings _lensFlareSettings;
        AntiAliasSettings _antiAliasSettings;
        ExposureSettings _exposureSettings;

        public ExposureSettings ExposureSettings { get => _exposureSettings; set => _exposureSettings = value; }
        public AntiAliasSettings AntiAliasSettings { get => _antiAliasSettings; set => _antiAliasSettings = value; }
        public LensFlareSettings LensFlareSettings { get => _lensFlareSettings; set => _lensFlareSettings = value; }
        public BloomSettings BloomSettings { get => _bloomSettings; set => _bloomSettings = value; }
        public ColorGradeSettings ColorGradeSettings { get => _colorGradeSettings; set => _colorGradeSettings = value; }
        public DepthOfFieldSettings DepthOfFieldSettings { get => _depthOfFieldSettings; set => _depthOfFieldSettings = value; }
        public VignetteSettings VignetteSettings { get => _vignetteSettings; set => _vignetteSettings = value; }
        
        public unsafe override void Write(VoidPtr address, StringTable table)
        {
            *(Header*)address = this;
        }
        public unsafe override void Read(VoidPtr address, VoidPtr strings)
        {
            Header h = *(Header*)address;

        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("postProcess");

            writer.WriteEndElement();
        }
        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("postProcess", true))
                throw new Exception();
            while (reader.BeginElement())
            {
                reader.EndElement();
            }
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {


            public static implicit operator Header(PostProcessSettings p)
            {
                return new Header()
                {

                };
            }
            public static implicit operator PostProcessSettings(Header h)
            {
                return new PostProcessSettings();
            }
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
