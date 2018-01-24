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
    public enum RenderLibrary
    {
        OpenGL,
        Direct3D11,
    }
    public enum AudioLibrary
    {
        OpenAL,
        //DirectSound,
    }
    public enum InputLibrary
    {
        OpenTK,
        XInput,
        Windows,
    }
    public enum PhysicsLibrary
    {
        Bullet,
        //PhysX,
        //Jitter,
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
    [FileDef("User Settings")]
    public class UserSettings : TSettings
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
        private PhysicsLibrary _physicsLibrary = PhysicsLibrary.Bullet;

        [TSerialize]
        public VSyncMode VSync { get => _vSyncMode; set => _vSyncMode = value; }
        [TSerialize]
        public EngineQuality TextureQuality { get => _textureQuality; set => _textureQuality = value; }
        [TSerialize]
        public EngineQuality ModelQuality { get => _modelQuality; set => _modelQuality = value; }
        [TSerialize]
        public EngineQuality SoundQuality { get => _soundQuality; set => _soundQuality = value; }
        [TSerialize]
        public RenderLibrary RenderLibrary { get => _renderLibrary; set => _renderLibrary = value; }
        [TSerialize]
        public AudioLibrary AudioLibrary { get => _audioLibrary; set => _audioLibrary = value; }
        [TSerialize]
        public InputLibrary InputLibrary { get => _inputLibrary; set => _inputLibrary = value; }
        [TSerialize]
        public PhysicsLibrary PhysicsLibrary { get => _physicsLibrary; set => _physicsLibrary = value; }
        [TSerialize]
        public WindowBorderStyle WindowBorderStyle { get => _windowBorderStyle; set => _windowBorderStyle = value; }
        [TSerialize]
        public bool FullScreen { get => _fullscreen; set => _fullscreen = value; }
    }
}
