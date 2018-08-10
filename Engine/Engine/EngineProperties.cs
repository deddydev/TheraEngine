﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Audio;
using TheraEngine.Core;
using TheraEngine.Editor;
using TheraEngine.Files;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Input.Devices.DirectX;
using TheraEngine.Input.Devices.OpenTK;
using TheraEngine.Networking;
using TheraEngine.Physics;
using TheraEngine.Physics.Bullet;
using TheraEngine.Physics.Jitter;
using TheraEngine.Rendering;
using TheraEngine.Timers;
using TheraEngine.Worlds;

namespace TheraEngine
{
    public delegate void DelTick(float delta);
    public enum ETickGroup
    {
        PrePhysics = 0,
        DuringPhysics = 15,
        PostPhysics = 30,
    }
    public enum ETickOrder
    {
        Timers = 0, //Call timing events
        Input = 3, //Call input events
        Animation = 6, //Update model animation positions
        Logic = 9, //Gameplay calculations
        Scene = 12, //Update scene
    }
    public static partial class Engine
    {
        public const RenderLibrary DefaultRenderLibrary = RenderLibrary.OpenGL;
        public const AudioLibrary DefaultAudioLibrary = AudioLibrary.OpenAL;
        public const InputLibrary DefaultInputLibrary = InputLibrary.OpenTK;
        public const PhysicsLibrary DefaultPhysicsLibrary = PhysicsLibrary.Bullet;

        public static string StartupPath = Application.StartupPath + Path.DirectorySeparatorChar;
        public static string ContentFolderAbs = StartupPath + ContentFolderRel;
        public static string ContentFolderRel = "Content" + Path.DirectorySeparatorChar;
        public static string ConfigFolderAbs = StartupPath + ConfigFolderRel;
        public static string ConfigFolderRel = "Config" + Path.DirectorySeparatorChar;
        public static string EngineSettingsPathAbs = ConfigFolderAbs + "Engine.xset";
        public static string EngineSettingsPathRel = ConfigFolderRel + "Engine.xset";
        public static string UserSettingsPathAbs = ConfigFolderAbs + "User.xset";
        public static string UserSettingsPathRel = ConfigFolderRel + "User.xset";

        public static NetworkConnection Network { get; set; }
        public static Server ServerConnection => Network as Server;
        public static Client ClientConnection => Network as Client;

        /// <summary>
        /// Event for when the engine is paused or unpaused and by which player.
        /// </summary>
        public static event Action<bool, LocalPlayerIndex> PauseChanged;
        /// <summary>
        /// Event for sending debug console output text.
        /// </summary>
        public static event Action<string> DebugOutput;
        /// <summary>
        /// Event fired before the current world is changed.
        /// </summary>
        public static event Action PreWorldChanged;
        /// <summary>
        /// Event fired after the current world is changed.
        /// </summary>
        public static event Action PostWorldChanged;

        public static BaseGameMode ActiveGameMode => Game?.State.GameModeRef?.File;

        /// <summary>
        /// Instances of files that are loaded only once and are accessable by all global references to that file.
        /// </summary>
        public static ConcurrentDictionary<string, IFileObject> GlobalFileInstances { get; } = new ConcurrentDictionary<string, IFileObject>();
        /// <summary>
        /// Instances of files that are loaded locally in a class. A single file may be loaded independently in multiple local contexts.
        /// </summary>
        public static ConcurrentDictionary<string, List<IFileObject>> LocalFileInstances { get; } = new ConcurrentDictionary<string, List<IFileObject>>();
        /// <summary>
        /// Controllers for all players that are local to this client.
        /// </summary>
        public static List<LocalPlayerController> LocalPlayers { get; } = new List<LocalPlayerController>();
        
        public static List<AIController> ActiveAI = new List<AIController>();

        public static Lazy<EngineSettings> DefaultEngineSettings { get; set; } = new Lazy<EngineSettings>(() => new EngineSettings());

        /// <summary>
        /// The scene containing actors of the world the engine is currently hosting.
        /// </summary>
        public static BaseScene Scene => World?.Scene;
        /// <summary>
        /// Information necessary to run a game.
        /// </summary>
        public static Game Game { get; private set; }
        /// <summary>
        /// The settings for the engine, specified by the game.
        /// </summary>
        public static EngineSettings Settings => Game?.EngineSettingsRef?.File ?? DefaultEngineSettings.Value;

        internal static int RenderThreadId;
        public static bool IsInRenderThread() => Thread.CurrentThread.ManagedThreadId == RenderThreadId;
        
        /// <summary>
        /// The settings for the engine, specified by the user.
        /// </summary>
        public static UserSettings UserSettings => Game?.UserSettingsRef;
        
        /// <summary>
        /// The index of the currently ticking list of functions (group + order + pause)
        /// </summary>
        private static int _currentTickList = -1;

        private static ConcurrentDictionary<World, Vec3> RebaseWorldsQueue1 = new ConcurrentDictionary<World, Vec3>();
        private static ConcurrentDictionary<World, Vec3> RebaseWorldsQueue2 = new ConcurrentDictionary<World, Vec3>();
        public static void QueueRebaseOrigin(World world, Vec3 point)
        {
            RebaseWorldsQueue2.AddOrUpdate(world, t => point, (t, t2) => point);
        }

        /// <summary>
        /// Queue for adding to or removing from the currently ticking list
        /// </summary>
        private static ConcurrentQueue<Tuple<bool, DelTick>> _tickListQueue = new ConcurrentQueue<Tuple<bool, DelTick>>();
        public static List<DelTick>[] _tickLists;
        private static RenderLibrary _renderLibrary = DefaultRenderLibrary;
        private static AudioLibrary _audioLibrary = DefaultAudioLibrary;
        private static InputLibrary _inputLibrary = DefaultInputLibrary;
        private static PhysicsLibrary _physicsLibrary = DefaultPhysicsLibrary;
        private static EngineTimer _timer = new EngineTimer();
        private static List<DateTime> _debugTimers = new List<DateTime>();

        //Continually scans for and processes new input devices.
        //TODO: allow disabling
        private static InputAwaiter _inputAwaiter;

        //Queue of what pawns should be possessed next for each player index when they either first join the game, or have their controlled pawn set to null.
        private static Dictionary<LocalPlayerIndex, Queue<IPawn>> _possessionQueues = new Dictionary<LocalPlayerIndex, Queue<IPawn>>();

        //internal static List<PhysicsDriver> _queuedCollisions = new List<PhysicsDriver>();
        private static Dictionary<string, int> _fontIndexMatching = new Dictionary<string, int>();
        private static PrivateFontCollection _fontCollection = new PrivateFontCollection();

        public static int MainThreadID;

        /// <summary>
        /// The world that is currently being rendered and played in.
        /// </summary>
        public static World World { get; private set; } = null;
        public static bool IsPaused { get; private set; } = false;

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine performance accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo => _computerInfo.Value;
        private static Lazy<ComputerInfo> _computerInfo = new Lazy<ComputerInfo>(() => ComputerInfo.Analyze());

#if EDITOR
        public static EngineEditorState EditorState = new EngineEditorState();
#endif

        #region Timing

        public static float RenderDelta => _timer.RenderTime;
        public static float UpdateDelta => _timer.UpdateTime;
        public static float RenderPeriod => _timer.RenderPeriod;
        public static float UpdatePeriod => _timer.UpdatePeriod;

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static float TargetRenderFreq
        {
            get => _timer.TargetRenderFrequency;
            set
            {
                _timer.TargetRenderFrequency = value;
            }
        }
        /// <summary>
        /// Frames per second that the game will try to update at.
        /// </summary>
        public static float TargetUpdateFreq
        {
            get => _timer.TargetUpdateFrequency;
            set => _timer.TargetUpdateFrequency = value;
        }

        /// <summary>
        /// How fast/slow the game moves.
        /// Greater than 1.0 for speed up, less than 1.0 for slow down.
        /// </summary>
        public static float TimeDilation
        {
            get => _timer.TimeDilation;
            set
            {
                _timer.TimeDilation = value;
            }
        }
        
        #endregion

        #region Libraries
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
                        _audioManager = new ALAudioManager();
                        break;
                }
            }
        }
        /// <summary>
        /// The library to play audio with.
        /// </summary>
        public static PhysicsLibrary PhysicsLibrary
        {
            get => _physicsLibrary;
            set
            {
                _physicsLibrary = value;
                switch (_physicsLibrary)
                {
                    case PhysicsLibrary.Bullet:
                        _physicsInterface = new BulletPhysicsInterface();
                        break;
                    case PhysicsLibrary.Jitter:
                        _physicsInterface = new JitterPhysicsInterface();
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
        #endregion

        #region Library Abstraction
        /// <summary>
        /// Provides an abstraction layer for managing any supported render library.
        /// </summary>
        public static AbstractRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                    throw new InvalidOperationException("No render library set.");
                //if (MainThreadID != Thread.CurrentThread.ManagedThreadId)
                //    throw new Exception("Cannot make render calls off the main thread. Invoke the method containing the calls with RenderPanel.CapturedPanel beforehand.");
                return _renderer;
            }
            internal set => _renderer = value;
        }
        /// <summary>
        /// Provides an abstraction layer for managing any supported physics engine.
        /// </summary>
        public static AbstractPhysicsInterface Physics
        {
            get
            {
                if (_physicsInterface == null)
                    throw new InvalidOperationException("No physics library set.");
                return _physicsInterface;
            }
        }
        /// <summary>
        /// Provides an abstraction layer for managing any supported audio library.
        /// </summary>
        public static AbstractAudioManager Audio
        {
            get
            {
                if (_audioManager == null)
                    throw new InvalidOperationException("No audio library set.");
                return _audioManager;
            }
        }

        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;
        private static AbstractPhysicsInterface _physicsInterface;
        #endregion
    }
}