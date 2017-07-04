using System;
using System.Windows.Forms;
using System.Collections.Generic;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using TheraEngine.Input;
using TheraEngine.Audio;
using TheraEngine.Input.Devices;
using TheraEngine.Input.Devices.OpenTK;
using TheraEngine.Input.Devices.DirectX;
using TheraEngine.Files;
using TheraEngine.Worlds.Actors;
using System.Drawing.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using TheraEngine.Timers;

namespace TheraEngine
{
    public static partial class Engine
    {
        public static string StartupPath = Application.StartupPath + "\\";
        public static string ContentFolderAbs = StartupPath + "Content\\";
        public static string ContentFolderRel = "Content\\";
        public static string ConfigFolderAbs = StartupPath + "Config\\";
        public static string ConfigFolderRel = "Config\\";
        public static string EngineSettingsPathAbs = ConfigFolderAbs + "Engine.xeset";
        public static string EngineSettingsPathRel = ConfigFolderRel + "Engine.xeset";
        public static string UserSettingsPathAbs = ConfigFolderAbs + "User.xuset";
        public static string UserSettingsPathRel = ConfigFolderRel + "User.xuset";

        public static event Action<bool, PlayerIndex> Paused;

        public static Dictionary<string, List<FileObject>> LoadedFiles = new Dictionary<string, List<FileObject>>();
        public static MonitoredList<LocalPlayerController> ActivePlayers = new MonitoredList<LocalPlayerController>();
        public static List<AIController> ActiveAI = new List<AIController>();
        public static List<World> LoadedWorlds = new List<World>();

        public static SceneProcessor Scene => _scene;
        private static SceneProcessor _scene = new SceneProcessor();

        private static World _currentWorld = null;

        //public static SingleFileRef<EngineSettings> _engineSettings = new SingleFileRef<EngineSettings>(EngineSettingsPathRel);
        //public static SingleFileRef<UserSettings> _userSettings = new SingleFileRef<UserSettings>(UserSettingsPathRel);

        public static Game Game => _game;
        public static EngineSettings Settings => Game?.EngineSettings;
        public static UserSettings UserSettings => Game?.UserSettings;
        
        /// <summary>
        /// The index of the currently ticking list of functions (group + order)
        /// </summary>
        private static int _currentTickList = -1;
        /// <summary>
        /// Queue for adding to or removing from the currently ticking list
        /// </summary>
        private static ConcurrentQueue<Tuple<bool, DelTick>> _tickListQueue = new ConcurrentQueue<Tuple<bool, DelTick>>();
        public static ThreadSafeList<DelTick>[] _tickLists;

        private static Game _game;
        private static RenderLibrary _renderLibrary;
        private static AudioLibrary _audioLibrary;
        private static InputLibrary _inputLibrary;

        private static bool _isPaused = false;
        private static EngineTimer _timer = new EngineTimer();
        private static List<DateTime> _debugTimers = new List<DateTime>();

        private static ComputerInfo _computerInfo;
        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;

        //Continually scans for and processes new input devices.
        //TODO: allow disabling
        private static InputAwaiter _inputAwaiter;

        //Queue of what pawns should be possessed next for each player index when they either first join the game, or have their controlled pawn set to null.
        private static Dictionary<PlayerIndex, Queue<IPawn>> _possessionQueues = new Dictionary<PlayerIndex, Queue<IPawn>>();

        //internal static List<PhysicsDriver> _queuedCollisions = new List<PhysicsDriver>();
        private static PrivateFontCollection _fontCollection = new PrivateFontCollection();
        public static int MainThreadID;

        public static AbstractRenderer Renderer
        {
            get
            {
                if (MainThreadID != Thread.CurrentThread.ManagedThreadId)
                    throw new Exception("Cannot make render calls off the main thread. Invoke the method containing the calls with RenderPanel.CapturedPanel beforehand.");
                return _renderer;
            }
            internal set => _renderer = value;
        }
        public static AbstractAudioManager Audio
        {
            get => _audioManager;
            set => _audioManager = value;
        }

        public static float RenderDelta => (float)_timer.RenderTime;
        public static float UpdateDelta => (float)_timer.UpdateTime;
        public static double RenderPeriod => _timer.RenderPeriod;
        public static double UpdatePeriod => _timer.UpdatePeriod;

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
        /// How fast/slow the game moves.
        /// Greater than 1.0 for speed up, less than 1.0 for slow down.
        /// </summary>
        public static double TimeDilation
        {
            get => _timer.TimeDilation;
            set => _timer.TimeDilation = value;
        }
        
        /// <summary>
        /// The world that is currently being rendered and played in.
        /// </summary>
        public static World World
        {
            get => _currentWorld;
            set => SetCurrentWorld(value, true);
        }

        public static bool IsPaused => _isPaused;

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine performance accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo => _computerInfo;
        /// <summary>
        /// The library to render with.
        /// </summary>
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
        /// <summary>
        /// The library to play audio with.
        /// </summary>
        public static AudioLibrary AudioLibrary
        {
            get => _audioLibrary;
            set
            {
                _audioLibrary = value;
                switch (_audioLibrary)
                {
                    case AudioLibrary.OpenAL:
                        Audio = new ALAudioManager();
                        break;
                }
            }
        }
        /// <summary>
        /// The library to read input with.
        /// </summary>
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