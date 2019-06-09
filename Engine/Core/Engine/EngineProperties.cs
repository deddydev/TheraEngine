using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Audio;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection;
using TheraEngine.Editor;
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

        private static readonly EngineTimer _timer = new EngineTimer();
        private static InputAwaiter _inputAwaiter;

        public static float RenderFrequency => _timer.RenderFrequency;
        public static float UpdateFrequency => _timer.UpdateFrequency;
        public static float RenderDelta => _timer.RenderTime;
        public static float UpdateDelta => _timer.UpdateTime;
        public static float RenderPeriod => _timer.RenderPeriod;
        public static float UpdatePeriod => _timer.UpdatePeriod;

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static float TargetFramesPerSecond
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
        public static float TargetUpdatesPerSecond
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

        #region Libraries
        /// <summary>
        /// The library to render with.
        /// </summary>
        public static ERenderLibrary RenderLibrary
        {
            get => Instance.RenderLibrary;
            internal set
            {
                Instance.RenderLibrary = value;
                RenderLibraryChanged();
            }
        }
        private static void RenderLibraryChanged()
        {
            Renderer = RenderContext.Captured?.GetRendererInstance();
            List<RenderContext> contexts = new List<RenderContext>(RenderContext.BoundContexts);
            foreach (RenderContext c in contexts)
                c.Control?.CreateContext();
        }
        /// <summary>
        /// The library to play audio with.
        /// </summary>
        public static EAudioLibrary AudioLibrary
        {
            get => Instance.AudioLibrary;
            internal set
            {
                Instance.AudioLibrary = value;
                RetrieveAudioManager();
            }
        }
        private static void RetrieveAudioManager()
        {
            switch (Instance.AudioLibrary)
            {
                case EAudioLibrary.OpenAL:
                    _audioManager = new ALAudioManager();
                    break;
            }
        }
        /// <summary>
        /// The library to play audio with.
        /// </summary>
        public static EPhysicsLibrary PhysicsLibrary
        {
            get => Instance.PhysicsLibrary;
            internal set
            {
                Instance.PhysicsLibrary = value;
                RetrievePhysicsInterface();
            }
        }

        private static void RetrievePhysicsInterface()
        {
            switch (Instance.PhysicsLibrary)
            {
                case EPhysicsLibrary.Bullet:
                    _physicsInterface = new BulletPhysicsInterface();
                    break;
                case EPhysicsLibrary.Jitter:
                    _physicsInterface = new JitterPhysicsInterface();
                    break;
            }
        }
        /// <summary>
        /// The library to read input with.
        /// </summary>
        public static EInputLibrary InputLibrary
        {
            get => Instance.InputLibrary;
            internal set
            {
                Instance.InputLibrary = value;
                InputLibraryChanged();
            }
        }
        private static void InputLibraryChanged()
        {
            //_inputAwaiter?.Dispose();
            switch (InputLibrary)
            {
                case EInputLibrary.OpenTK:
                    _inputAwaiter = new TKInputAwaiter(Instance.FoundInput);
                    break;
                case EInputLibrary.XInput:
                    _inputAwaiter = new DXInputAwaiter(Instance.FoundInput);
                    break;
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

        public static bool IsSingleThreaded => _timer.IsSingleThreaded;

        public static EngineDomainProxy DomainProxy
        {
            get => Instance.DomainProxy;
            internal set => Instance.DomainProxy = value;
        }

        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;
        private static AbstractPhysicsInterface _physicsInterface;
        #endregion

        public partial class InternalEnginePersistentSingleton : MarshalByRefObject
        {
            public InternalEnginePersistentSingleton()
            {
                TickLists = new List<DelTick>[45];
                for (int i = 0; i < TickLists.Length; ++i)
                    TickLists[i] = new List<DelTick>();
            }

            public ERenderLibrary RenderLibrary { get; set; } = DefaultRenderLibrary;
            public EAudioLibrary AudioLibrary { get; set; } = DefaultAudioLibrary;
            public EInputLibrary InputLibrary { get; set; } = DefaultInputLibrary;
            public EPhysicsLibrary PhysicsLibrary { get; set; } = DefaultPhysicsLibrary;
            
            private Lazy<EngineSettings> _defaultEngineSettings = new Lazy<EngineSettings>(() => new EngineSettings(), true);

            public NetworkConnection Network { get; set; }
            public Server ServerConnection => Network as Server;
            public Client ClientConnection => Network as Client;
            public EngineSingleton Singleton { get; set; }

            public GlobalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef { get; set; }
                = new GlobalFileRef<EngineSettings>(Path.Combine(Application.StartupPath, "EngineConfig.xset")) { AllowDynamicConstruction = true, CreateFileIfNonExistent = true };

            private TGame _game;
            public IScene Scene => World?.Scene;
            public TGame Game
            {
                get => _game;
                internal set
                {
                    if (_game == value)
                        return;

                    _game = value;
                }
            }

            public void GenerateProxy<T>(AppDomain domain, TGame game) where T : EngineDomainProxy, new()
            {
                string domainName = domain.FriendlyName;
                PrintLine("Generating proxy of type " + typeof(T).GetFriendlyName() + " in domain " + domainName);

                if (domain == AppDomain.CurrentDomain)
                {
                    DomainProxy = new T();
                }
                else
                {
                    DomainProxy = domain.CreateInstanceAndUnwrap<T>();
                    var lease = DomainProxy.InitializeLifetimeService() as ILease;
                    lease.Register(DomainProxy.SponsorRef);
                }

                DomainProxy.Run(game);

                //dynamic dynProxy = proxy;
                //string info = dynProxy.GetVersionInfo();

                //Type type = typeof(ProjectDomainProxy);
                //string info3 = type.Assembly.CodeBase;
                //string info4 = dynProxy.GetType().Assembly.CodeBase;

                //Engine.PrintLine(info);
                //Engine.PrintLine(info3);
                //Engine.PrintLine(info4);

                //Type.TypeCreationFailed = TypeCreationFailed;
            }

            private Type TypeCreationFailed(string typeDeclaration)
                => DomainProxy.CreateType(typeDeclaration);

            [Browsable(false)]
            public EngineDomainProxy DomainProxy { get; set; }

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

            public ConcurrentDictionary<IWorld, Vec3> RebaseWorldsProcessing = new ConcurrentDictionary<IWorld, Vec3>();
            public ConcurrentDictionary<IWorld, Vec3> RebaseWorldsQueue = new ConcurrentDictionary<IWorld, Vec3>();

            /// <summary>
            /// The index of the currently ticking list of functions (group + order + pause)
            /// </summary>
            public int CurrentTickList = -1;
            /// <summary>
            /// Queue for adding to or removing from the currently ticking list
            /// </summary>
            public ConcurrentQueue<Tuple<bool, DelTick>> TickListQueue = new ConcurrentQueue<Tuple<bool, DelTick>>();
            public List<DelTick>[] TickLists;

            public List<DateTime> _debugTimers = new List<DateTime>();

            public Dictionary<string, int> _fontIndexMatching = new Dictionary<string, int>();
            public PrivateFontCollection _fontCollection = new PrivateFontCollection();

            /// <summary>
            /// The world that is currently being rendered and played in.
            /// </summary>
            public IWorld World { get; internal set; } = null;
            public bool IsPaused { get; internal set; } = false;

            /// <summary>
            /// Class containing this computer's specs. Use to adjust engine performance accordingly.
            /// </summary>
            public ComputerInfo ComputerInfo { get; } = ComputerInfo.Analyze();

#if EDITOR
            public EngineEditorState EditorState { get; } = new EngineEditorState();
#endif
        }
    }
}