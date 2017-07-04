using TheraEngine.Files;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheraEngine
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
        XInput,
        Raw,
    }
    public enum WindowState
    {
        Unchanged = 0,
        Minimized = 1,
        Maximized = 2
    }
    public enum WindowBorderStyle
    {
        None,
        Fixed,
        Sizable,
    }
    [FileClass("USET", "User Settings")]
    public class UserSettings : FileObject
    {
        private WindowBorderStyle _windowBorderStyle = WindowBorderStyle.Sizable;
        private bool _fullscreen = false;
        private VSyncMode _vSyncMode = VSyncMode.Adaptive;
        private EngineQuality _textureQuality = EngineQuality.Highest;
        private EngineQuality _modelQuality = EngineQuality.Highest;
        private EngineQuality _soundQuality = EngineQuality.Highest;

        //Preferred libraries - will use whichever is available if the preferred one is not.
        private RenderLibrary _renderLibrary = RenderLibrary.OpenGL;
        private AudioLibrary _audioLibrary = AudioLibrary.OpenAL;
        private InputLibrary _inputLibrary = InputLibrary.OpenTK;

        [Serialize]
        public VSyncMode VSync { get => _vSyncMode; set => _vSyncMode = value; }
        [Serialize]
        public EngineQuality TextureQuality { get => _textureQuality; set => _textureQuality = value; }
        [Serialize]
        public EngineQuality ModelQuality { get => _modelQuality; set => _modelQuality = value; }
        [Serialize]
        public EngineQuality SoundQuality { get => _soundQuality; set => _soundQuality = value; }
        [Serialize]
        public RenderLibrary RenderLibrary { get => _renderLibrary; set => _renderLibrary = value; }
        [Serialize]
        public AudioLibrary AudioLibrary { get => _audioLibrary; set => _audioLibrary = value; }
        [Serialize]
        public InputLibrary InputLibrary { get => _inputLibrary; set => _inputLibrary = value; }
        [Serialize]
        public WindowBorderStyle WindowBorderStyle { get => _windowBorderStyle; set => _windowBorderStyle = value; }
        [Serialize]
        public bool FullScreen { get => _fullscreen; set => _fullscreen = value; }
        
        public static UserSettings FromXML(string filePath)
        {
            return FromXML<UserSettings>(filePath);
        }
    }
}
