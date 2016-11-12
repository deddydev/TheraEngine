using CustomEngine.Files;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CustomEngine
{
    public enum EngineQuality
    {
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }
    public enum RenderLibrary
    {
        OpenGL,
        Direct3D11,
    }
    public enum AudioLibrary
    {
        OpenAL,
        DirectSound,
    }
    public enum InputLibrary
    {
        OpenTK,
        XInput
    }
    [Serializable]
    public class UserSettings : FileObject
    {
        public bool VSync = true;
        public EngineQuality TextureQuality = EngineQuality.Highest;
        public EngineQuality ModelQuality = EngineQuality.Highest;
        public EngineQuality SoundQuality = EngineQuality.Highest;
        public RenderLibrary RenderLibrary = RenderLibrary.OpenGL;
        public AudioLibrary AudioLibrary = AudioLibrary.OpenAL;
        public InputLibrary InputLibrary = InputLibrary.OpenTK;
        
        public static UserSettings FromXML(string filePath)
        {
            return FromXML<UserSettings>(filePath);
        }
    }
}
