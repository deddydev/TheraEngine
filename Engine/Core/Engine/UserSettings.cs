using System;
using System.ComponentModel;

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
    public enum ERenderLibrary
    {
        OpenGL,
        Direct3D11,
    }
    public enum EAudioLibrary
    {
        OpenAL,
        //DirectSound,
    }
    public enum EInputLibrary
    {
        OpenTK,
        XInput,
        Windows,
    }
    public enum EPhysicsLibrary
    {
        Bullet,
        //PhysX,
        Jitter,
        //Havok,
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
    [Serializable]
    [TFileDef("User Settings")]
    public class UserSettings : TSettings
    {
        private WindowBorderStyle _windowBorderStyle = WindowBorderStyle.Sizable;
        private bool _fullscreen = false;
        private EVSyncMode _vSyncMode = EVSyncMode.Adaptive;
        private EngineQuality _textureQuality = EngineQuality.Highest;
        private EngineQuality _modelQuality = EngineQuality.Highest;
        private EngineQuality _soundQuality = EngineQuality.Highest;

        //Preferred libraries - will use whichever is available if the preferred one is not.
        private ERenderLibrary _renderLibrary = ERenderLibrary.OpenGL;
        private EAudioLibrary _audioLibrary = EAudioLibrary.OpenAL;
        private EInputLibrary _inputLibrary = EInputLibrary.OpenTK;
        private EPhysicsLibrary _physicsLibrary = EPhysicsLibrary.Bullet;

        [TSerialize]
        public EVSyncMode VSync { get => _vSyncMode; set => _vSyncMode = value; }
        [TSerialize]
        public EngineQuality TextureQuality { get => _textureQuality; set => _textureQuality = value; }
        [TSerialize]
        public EngineQuality ModelQuality { get => _modelQuality; set => _modelQuality = value; }
        [TSerialize]
        public EngineQuality SoundQuality { get => _soundQuality; set => _soundQuality = value; }
        [TSerialize]
        public ERenderLibrary RenderLibrary { get => _renderLibrary; set => _renderLibrary = value; }
        [TSerialize]
        public EAudioLibrary AudioLibrary { get => _audioLibrary; set => _audioLibrary = value; }
        [TSerialize]
        public EInputLibrary InputLibrary { get => _inputLibrary; set => _inputLibrary = value; }
        [TSerialize]
        public EPhysicsLibrary PhysicsLibrary { get => _physicsLibrary; set => _physicsLibrary = value; }
        [TSerialize]
        public WindowBorderStyle WindowBorderStyle { get => _windowBorderStyle; set => _windowBorderStyle = value; }
        [TSerialize]
        public bool FullScreen { get => _fullscreen; set => _fullscreen = value; }
    }
}
