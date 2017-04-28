using System;
using System.Windows.Forms;
using System.Collections.Generic;
using CustomEngine.Rendering;
using CustomEngine.Worlds;
using CustomEngine.Input;
using CustomEngine.Audio;
using CustomEngine.Input.Devices;
using CustomEngine.Input.Devices.OpenTK;
using CustomEngine.Input.Devices.DirectX;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors;
using System.Drawing.Text;

namespace CustomEngine
{
    public static partial class Engine
    {
        public static string StartupPath = Application.StartupPath + "\\";
        public static string ContentFolderAbs = StartupPath + ContentFolderRel;
        public static string ContentFolderRel = "Content\\";
        public static string ConfigFolderAbs = StartupPath + ConfigFolderRel;
        public static string ConfigFolderRel = "Config\\";
        public static string EngineSettingsPathAbs = ConfigFolderAbs + "Engine.xcsettings";
        public static string EngineSettingsPathRel = ConfigFolderRel + "Engine.xcsettings";
        public static string UserSettingsPathAbs = ConfigFolderAbs + "User.xcsettings";
        public static string UserSettingsPathRel = ConfigFolderRel + "User.xcsettings";
        
        private static World _transitionWorld = null;
        private static World _currentWorld = null;
        public static SingleFileRef<EngineSettings> _engineSettings = new SingleFileRef<EngineSettings>(EngineSettingsPathRel);
        public static SingleFileRef<UserSettings> _userSettings = new SingleFileRef<UserSettings>(UserSettingsPathRel);

        public static EngineSettings Settings
        {
            get => _engineSettings.File;
            set => _engineSettings.File = value;
        }
        public static UserSettings UserSettings
        {
            get => _userSettings.File;
            set => _userSettings.File = value;
        }

        public static Dictionary<string, List<FileObject>> LoadedFiles = new Dictionary<string, List<FileObject>>();
        public static MonitoredList<LocalPlayerController> ActivePlayers = new MonitoredList<LocalPlayerController>();
        public static List<AIController> ActiveAI = new List<AIController>();
        public static List<World> LoadedWorlds = new List<World>();
        //public static int PhysicsSubsteps = 10;

        private static bool _isPaused = false;
        private static ComputerInfo _computerInfo;
        private static GlobalTimer _timer = new GlobalTimer();
        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;
        private static RenderLibrary _renderLibrary;
        private static AudioLibrary _audioLibrary;
        private static InputLibrary _inputLibrary;
        private static List<DateTime> _debugTimers = new List<DateTime>();
        private static InputAwaiter _inputAwaiter;
        private static Dictionary<PlayerIndex, Queue<IPawn>> _possessionQueue = new Dictionary<PlayerIndex, Queue<IPawn>>();
        //internal static List<PhysicsDriver> _queuedCollisions = new List<PhysicsDriver>();

        public static Viewport.TwoPlayerViewportPreference TwoPlayerPref = 
            Viewport.TwoPlayerViewportPreference.SplitHorizontally;
        public static Viewport.ThreePlayerViewportPreference ThreePlayerPref =
            Viewport.ThreePlayerViewportPreference.PreferFirstPlayer;

        public static Dictionary<ETickGroup, Dictionary<ETickOrder, List<ObjectBase>>> _tick = 
            new Dictionary<ETickGroup, Dictionary<ETickOrder, List<ObjectBase>>>();

        internal static AbstractRenderer Renderer
        {
            get => _renderer;
            set => _renderer = value;
        }
        internal static AbstractAudioManager AudioManager
        {
            get => _audioManager;
            set => _audioManager = value;
        }

        public static float RenderDelta => (float)_timer.RenderTime;
        public static float UpdateDelta => (float)_timer.UpdateTime;

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static double TargetRenderFreq
        {
            get => _timer.TargetRenderFrequency;
            set => _timer.TargetRenderFrequency = value;
        }

        /// <summary>
        /// Frames per second that the game will try to update at.
        /// </summary>
        public static double TargetUpdateFreq
        {
            get => _timer.TargetUpdateFrequency;
            set => _timer.TargetUpdateFrequency = value;
        }

        /// <summary>
        /// How fast/slow the game time looks
        /// </summary>
        public static double TimeDilation
        {
            get => _timer.TimeDilation;
            set => _timer.TimeDilation = value;
        }
        
        public static World TransitionWorld
        {
            get => _transitionWorld;
            set => _transitionWorld = value;
        }
        public static World World
        {
            get => _currentWorld;
            set => SetCurrentWorld(value, true);
        }

        public static bool IsPaused => _isPaused;

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine settings accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo => _computerInfo;
        public static RenderPanel CurrentPanel
        {
            get
            {
                RenderContext ctx = RenderContext.Current;
                if (ctx != null)
                    return ctx.Control;
                return null;
            }
        }
        public static RenderLibrary RenderLibrary
        {
            get => _renderLibrary;
            set
            {
                _renderLibrary = value;
                List<RenderContext> contexts = new List<RenderContext>(RenderContext.BoundContexts);
                foreach (RenderContext c in contexts)
                    c.Control?.CreateContext();
            }
        }
        public static AudioLibrary AudioLibrary
        {
            get => _audioLibrary;
            set
            {
                _audioLibrary = value;
                switch (_audioLibrary)
                {
                    case AudioLibrary.OpenAL:
                        AudioManager = new ALAudioManager();
                        break;
                }
            }
        }
        public static InputLibrary InputLibrary
        {
            get => _inputLibrary;
            set
            {
                _inputLibrary = value;
                _inputAwaiter?.Dispose();
                switch (_inputLibrary)
                {
                    case InputLibrary.OpenTK:
                        _inputAwaiter = new TKInputAwaiter(FoundInput);
                        break;
                    case InputLibrary.XInput:
                        _inputAwaiter = new DXInputAwaiter(FoundInput);
                        break;
                }
            }
        }
    }
}