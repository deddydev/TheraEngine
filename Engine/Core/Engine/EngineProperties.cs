using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Audio;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Editor;
using TheraEngine.Core.Files;
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
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;
using System.Drawing;

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
    /// <summary>
    /// Class to inherit from in order to store custom persistent information.
    /// </summary>
    [TFileDef("Engine Singleton", "")]
    [TFileExt("singleton")]
    public class EngineSingleton : TFileObject
    {
        internal EngineSingleton() { }
    }
    public static partial class Engine
    {
        public const ERenderLibrary DefaultRenderLibrary = ERenderLibrary.OpenGL;
        public const EAudioLibrary DefaultAudioLibrary = EAudioLibrary.OpenAL;
        public const EInputLibrary DefaultInputLibrary = EInputLibrary.OpenTK;
        public const EPhysicsLibrary DefaultPhysicsLibrary = EPhysicsLibrary.Bullet;

        public static readonly string StartupPath = Application.StartupPath + Path.DirectorySeparatorChar;
        public static readonly string ContentFolderAbs = StartupPath + ContentFolderRel;
        public static readonly string ContentFolderRel = "Content" + Path.DirectorySeparatorChar;
        public static readonly string ConfigFolderAbs = StartupPath + ConfigFolderRel;
        public static readonly string ConfigFolderRel = "Config" + Path.DirectorySeparatorChar;
        public static readonly string EngineSettingsPathAbs = ConfigFolderAbs + "Engine.xset";
        public static readonly string EngineSettingsPathRel = ConfigFolderRel + "Engine.xset";
        public static readonly string UserSettingsPathAbs = ConfigFolderAbs + "User.xset";
        public static readonly string UserSettingsPathRel = ConfigFolderRel + "User.xset";

        /// <summary>
        /// Event for when the engine is paused or unpaused and by which player.
        /// </summary>
        public static event Action<bool, ELocalPlayerIndex> PauseChanged;
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

        #region Singleton

        public static InternalEnginePersistentSingleton Instance => Singleton<InternalEnginePersistentSingleton>.Instance;

        public static NetworkConnection Network
        {
            get => Instance.Network;
            set => Instance.Network = value;
        }
        public static Server ServerConnection => Instance.ServerConnection;
        public static Client ClientConnection => Instance.ClientConnection;
        public static EngineSingleton Singleton
        {
            get => Instance.Singleton;
            set => Instance.Singleton = value;
        }
        /// <summary>
        /// The world that is currently being rendered and played in.
        /// </summary>
        public static IWorld World
        {
            get => Instance.World;
            private set => Instance.World = value;
        }
        /// <summary>
        /// The scene containing actors of the world the engine is currently hosting.
        /// </summary>
        public static IScene Scene => Instance.Scene;
        /// <summary>
        /// Information necessary to run a game.
        /// </summary>
        public static TGame Game
        {
            get => Instance.Game;
            private set => Instance.Game = value;
        }
        public static GlobalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef
        {
            get => Instance.DefaultEngineSettingsOverrideRef;
            set => Instance.DefaultEngineSettingsOverrideRef = value;
        }
        /// <summary>
        /// The settings for the engine, specified by the game.
        /// </summary>
        public static EngineSettings Settings => Instance.Settings;
        /// <summary>
        /// The settings for the engine, specified by the user.
        /// </summary>
        public static UserSettings UserSettings => Instance.UserSettings;

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine performance accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo => Instance.ComputerInfo;

#if EDITOR
        public static EngineEditorState EditorState => Instance.EditorState;
#endif

        public static bool IsPaused
        {
            get => Instance.IsPaused;
            private set => Instance.IsPaused = value;
        }

        public static void SetCurrentWorld(IWorld world)
            => Instance.SetCurrentWorld(world);

        public static void QueueRebaseOrigin(IWorld world, Vec3 point)
            => Instance.QueueRebaseOrigin(world, point);

        #region Timing

        public static float RenderFrequency => Instance.Timer.RenderFrequency;
        public static float UpdateFrequency => Instance.Timer.UpdateFrequency;
        public static float RenderDelta => Instance.Timer.RenderTime;
        public static float UpdateDelta => Instance.Timer.UpdateTime;
        public static float RenderPeriod => Instance.Timer.RenderPeriod;
        public static float UpdatePeriod => Instance.Timer.UpdatePeriod;

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static float TargetFramesPerSecond
        {
            get => Instance.Timer.TargetRenderFrequency;
            set
            {
                Instance.Timer.TargetRenderFrequency = value;
            }
        }
        /// <summary>
        /// Frames per second that the game will try to update at.
        /// </summary>
        public static float TargetUpdatesPerSecond
        {
            get => Instance.Timer.TargetUpdateFrequency;
            set => Instance.Timer.TargetUpdateFrequency = value;
        }

        /// <summary>
        /// How fast/slow the game moves.
        /// Greater than 1.0 for speed up, less than 1.0 for slow down.
        /// </summary>
        public static float TimeDilation
        {
            get => Instance.Timer.TimeDilation;
            set
            {
                Instance.Timer.TimeDilation = value;
            }
        }

        #endregion

        #endregion

        internal static int MainThreadID;
        internal static int RenderThreadId;
        public static bool IsInMainThread() => Thread.CurrentThread.ManagedThreadId == MainThreadID;
        public static bool IsInRenderThread() => Thread.CurrentThread.ManagedThreadId == RenderThreadId;

        public static bool Assert(bool condition, string message, bool throwException = true)
        {
            if (!condition)
            {
                if (throwException)
                    throw new Exception(message);
                else
                    LogWarning(message);
                return false;
            }
            return true;
        }

        public static Font MakeFont(string fontName, float size, FontStyle style)
            => Instance.MakeFont(fontName, size, style);

        private static ERenderLibrary _renderLibrary = DefaultRenderLibrary;
        private static EAudioLibrary _audioLibrary = DefaultAudioLibrary;
        private static EInputLibrary _inputLibrary = DefaultInputLibrary;
        private static EPhysicsLibrary _physicsLibrary = DefaultPhysicsLibrary;

        #region Libraries
        /// <summary>
        /// The library to render with.
        /// </summary>
        public static ERenderLibrary RenderLibrary
        {
            get => _renderLibrary;
            internal set
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
        public static EAudioLibrary AudioLibrary
        {
            get => _audioLibrary;
            internal set
            {
                _audioLibrary = value;
                switch (_audioLibrary)
                {
                    case EAudioLibrary.OpenAL:
                        _audioManager = new ALAudioManager();
                        break;
                }
            }
        }
        /// <summary>
        /// The library to play audio with.
        /// </summary>
        public static EPhysicsLibrary PhysicsLibrary
        {
            get => _physicsLibrary;
            internal set
            {
                _physicsLibrary = value;
                switch (_physicsLibrary)
                {
                    case EPhysicsLibrary.Bullet:
                        _physicsInterface = new BulletPhysicsInterface();
                        break;
                    case EPhysicsLibrary.Jitter:
                        _physicsInterface = new JitterPhysicsInterface();
                        break;
                }
            }
        }
        /// <summary>
        /// The library to read input with.
        /// </summary>
        public static EInputLibrary InputLibrary
        {
            get => _inputLibrary;
            internal set
            {
                _inputLibrary = value;

                Instance.InputLibraryChanged(_inputLibrary);
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

        public static bool IsSingleThreaded => Instance.Timer.IsSingleThreaded;

        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;
        private static AbstractPhysicsInterface _physicsInterface;
        #endregion

        public partial class InternalEnginePersistentSingleton : MarshalByRefObject
        {
            public InternalEnginePersistentSingleton()
            {
                Timer = new EngineTimer();
                Timer.UpdateFrame += EngineTick;
                Timer.SwapBuffers += SwapBuffers;

                TickLists = new List<DelTick>[45];
                for (int i = 0; i < TickLists.Length; ++i)
                    TickLists[i] = new List<DelTick>();
            }

            private Lazy<EngineSettings> _defaultEngineSettings = new Lazy<EngineSettings>(() => new EngineSettings(), true);

            public NetworkConnection Network { get; set; }
            public Server ServerConnection => Network as Server;
            public Client ClientConnection => Network as Client;
            public EngineSingleton Singleton { get; set; }

            public GlobalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef { get; set; }
                = new GlobalFileRef<EngineSettings>(Path.Combine(Application.StartupPath, "EngineConfig.xset")) { AllowDynamicConstruction = true, CreateFileIfNonExistent = true };

            public IScene Scene => World?.Scene;
            public TGame Game { get; internal set; }

            /// <summary>
            /// The settings for the engine, specified by the game.
            /// </summary>
            public EngineSettings Settings =>
                Game?.EngineSettingsOverrideRef?.File ?? //Game overrides engine settings?
                DefaultEngineSettingsOverrideRef.File ?? //User overrides engine settings?
                _defaultEngineSettings.Value; //Fall back to truly default engine settings

            /// <summary>
            /// The settings for the engine, specified by the user.
            /// </summary>
            public UserSettings UserSettings => Game?.UserSettingsRef?.File;

            private ConcurrentDictionary<IWorld, Vec3> RebaseWorldsProcessing = new ConcurrentDictionary<IWorld, Vec3>();
            private ConcurrentDictionary<IWorld, Vec3> RebaseWorldsQueue = new ConcurrentDictionary<IWorld, Vec3>();

            /// <summary>
            /// The index of the currently ticking list of functions (group + order + pause)
            /// </summary>
            internal int CurrentTickList = -1;
            /// <summary>
            /// Queue for adding to or removing from the currently ticking list
            /// </summary>
            internal ConcurrentQueue<Tuple<bool, DelTick>> TickListQueue = new ConcurrentQueue<Tuple<bool, DelTick>>();
            internal List<DelTick>[] TickLists;
            
            private ERenderLibrary _renderLibrary = DefaultRenderLibrary;
            private EAudioLibrary _audioLibrary = DefaultAudioLibrary;
            private EInputLibrary _inputLibrary = DefaultInputLibrary;
            private EPhysicsLibrary _physicsLibrary = DefaultPhysicsLibrary;

            internal EngineTimer Timer { get; } = new EngineTimer();

            private List<DateTime> _debugTimers = new List<DateTime>();
            private InputAwaiter _inputAwaiter;

            private static Dictionary<string, int> _fontIndexMatching = new Dictionary<string, int>();
            private static PrivateFontCollection _fontCollection = new PrivateFontCollection();

            /// <summary>
            /// The world that is currently being rendered and played in.
            /// </summary>
            public IWorld World { get; internal set; } = null;
            public bool IsPaused { get; internal set; } = false;

            /// <summary>
            /// Class containing this computer's specs. Use to adjust engine performance accordingly.
            /// </summary>
            public ComputerInfo ComputerInfo => _computerInfo.Value;
            private readonly Lazy<ComputerInfo> _computerInfo = new Lazy<ComputerInfo>(ComputerInfo.Analyze);

#if EDITOR
            public EngineEditorState EditorState { get; } = new EngineEditorState();
#endif
        }
    }
}