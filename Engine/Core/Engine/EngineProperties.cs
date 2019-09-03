using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
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

        #region Singleton

        /// <summary>
        /// Gets the domain data key for this type. This property may be called from any AppDomain, and will return the same value regardless of AppDomain.
        /// </summary>
        private static string Name => "BA9A49C7E9364060AC4E4DDDBF465684." + typeof(InternalEnginePersistentSingleton).FullName;

        /// <summary>
        /// A local cache of the instance wrapper.
        /// </summary>
        private static readonly Lazy<Wrapper> LazyInstance = AppDomain.CurrentDomain.IsDefaultAppDomain() ? CreateOnDefaultAppDomain() : CreateOnOtherAppDomain();

        /// <summary>
        /// A local cache of the instance.
        /// </summary>
        private static readonly Lazy<InternalEnginePersistentSingleton> CachedLazyInstance = new Lazy<InternalEnginePersistentSingleton>(() => Instance);

        /// <summary>
        /// Returns a lazy that creates the instance (if necessary) and saves it in the domain data.
        /// This method must only be called from the default AppDomain.
        /// </summary>
        private static Lazy<Wrapper> CreateOnDefaultAppDomain()
        {
            return new Lazy<Wrapper>(() =>
            {
                var ret = new Wrapper { WrappedInstance = new InternalEnginePersistentSingleton() };
                AppDomain.CurrentDomain.SetData(Name, ret);
                return ret;
            });
        }

        /// <summary>
        /// Returns a lazy that calls into the default domain to create the instance and retrieves a proxy into the current domain.
        /// </summary>
        private static Lazy<Wrapper> CreateOnOtherAppDomain()
        {
            return new Lazy<Wrapper>(() =>
            {
                var defaultAppDomain = AppDomainHelper.DefaultAppDomain;
                if (defaultAppDomain.GetData(Name) is Wrapper ret)
                    return ret;
                defaultAppDomain.DoCallBack(CreateCallback);
                return (Wrapper)defaultAppDomain.GetData(Name);
            });
        }

        /// <summary>
        /// Ensures the instance is created (and saved in the domain data). 
        /// This method must only be called on the default AppDomain.
        /// </summary>
        private static void CreateCallback() { var _ = LazyInstance.Value; }

        /// <summary>
        /// Gets the process-wide instance. 
        /// If the current domain is not the default AppDomain, this property returns a new proxy to the actual instance.
        /// </summary>
        public static InternalEnginePersistentSingleton Instance => LazyInstance.Value.WrappedInstance;

        /// <summary>
        /// Gets the process-wide instance. 
        /// If the current domain is not the default AppDomain, this property returns a cached proxy to the actual instance.
        /// It is your responsibility to ensure that the cached proxy does not time out; if you don't know what this means, use <see cref="Instance"/> instead.
        /// </summary>
        public static InternalEnginePersistentSingleton CachedInstance => CachedLazyInstance.Value;

        private sealed class Wrapper : MarshalByRefObject
        {
            public override object InitializeLifetimeService() => null;
            public InternalEnginePersistentSingleton WrappedInstance { get; set; }
        }

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

        public static void GlobalUpdate() => Scene?.GlobalUpdate();
        public static void GlobalPreRender() => Scene?.GlobalPreRender();
        public static void GlobalSwap() => Scene?.GlobalSwap();

        private static IWorld _world;
        public static IWorld World
        {
            get => _world;
            set
            {
                _world = value;
            }
        }
        public static IScene Scene => World?.Scene;
        private static TGame _game;
        public static TGame Game
        {
            get => _game;
            internal set
            {
                if (_game == value)
                    return;

                _game = value;
            }
        }

        public static GlobalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef
        {
            get => Instance.DefaultEngineSettingsOverrideRef;
            set => Instance.DefaultEngineSettingsOverrideRef = value;
        }

        /// <summary>
        /// The settings for the engine, specified by the game.
        /// </summary>
        public static EngineSettings Settings
        {
            get => Instance.Settings;
            set => Instance.Settings = value;
        }
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

        public static readonly EngineTimer Timer = new EngineTimer();

        //public static EngineTimer Timer => Instance.Timer;
        public static float RenderFrequency => Timer.RenderFrequency;
        public static float UpdateFrequency => Timer.UpdateFrequency;
        public static float RenderDelta => Timer.RenderTime;
        public static float UpdateDelta => Timer.UpdateTime;
        public static float RenderPeriod => Timer.RenderPeriod;
        public static float UpdatePeriod => Timer.UpdatePeriod;

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static float TargetFramesPerSecond
        {
            get => Timer.TargetRenderFrequency;
            set => Timer.TargetRenderFrequency = value;
        }
        /// <summary>
        /// Frames per second that the game will try to update at.
        /// </summary>
        public static float TargetUpdatesPerSecond
        {
            get => Timer.TargetUpdateFrequency;
            set => Timer.TargetUpdateFrequency = value;
        }

        /// <summary>
        /// How fast/slow the game moves.
        /// Greater than 1.0 for speed up, less than 1.0 for slow down.
        /// </summary>
        public static float TimeDilation
        {
            get => Timer.TimeDilation;
            set
            {
                Timer.TimeDilation = value;
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
            List<RenderContext> contexts = new List<RenderContext>(RenderContext.BoundContexts);
            foreach (RenderContext c in contexts)
                c.RecreateSelf();
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
                    Instance._audioManager = new ALAudioManager();
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
                default:
                case EPhysicsLibrary.Bullet:
                    Instance._physicsInterface = new BulletPhysicsInterface();
                    break;
                case EPhysicsLibrary.Jitter:
                    Instance._physicsInterface = new JitterPhysicsInterface();
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
                default:
                case EInputLibrary.OpenTK:
                    Instance.InputAwaiter = new TKInputAwaiter(Instance.FoundInput);
                    break;
                case EInputLibrary.XInput:
                    Instance.InputAwaiter = new DXInputAwaiter(Instance.FoundInput);
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
                var ctx = RenderContext.Captured;
                if (ctx is null)
                {
                    string domain = AppDomain.CurrentDomain.FriendlyName;
                    throw new InvalidOperationException("No render library set.");
                }
                //if (MainThreadID != Thread.CurrentThread.ManagedThreadId)
                //    throw new Exception("Cannot make render calls off the main thread. Invoke the method containing the calls with RenderPanel.CapturedPanel beforehand.");
                return ctx.Renderer;
            }
        }
        /// <summary>
        /// Provides an abstraction layer for managing any supported physics engine.
        /// </summary>
        public static AbstractPhysicsInterface Physics
        {
            get
            {
                if (Instance._physicsInterface == null)
                    throw new InvalidOperationException("No physics library set.");
                return Instance._physicsInterface;
            }
        }
        /// <summary>
        /// Provides an abstraction layer for managing any supported audio library.
        /// </summary>
        public static AbstractAudioManager Audio
        {
            get
            {
                if (Instance._audioManager == null)
                    throw new InvalidOperationException("No audio library set.");
                return Instance._audioManager;
            }
        }

        public static bool IsSingleThreaded => Timer.IsSingleThreaded;

        public static EngineDomainProxy DomainProxy
        {
            get => Instance.DomainProxy;
            internal set => Instance.DomainProxy = value;
        }

        #endregion

        public partial class InternalEnginePersistentSingleton: MarshalByRefObject
        {
            public DelBeginOperation BeginOperation;
            public DelEndOperation EndOperation;

            public event Action GotFocus;
            public event Action LostFocus;

            //public RenderContext CapturedRenderContext { get; set; }

            /// <summary>
            /// Event for when the engine is paused or unpaused and by which player.
            /// </summary>
            public event Action<bool, ELocalPlayerIndex> PauseChanged;
            /// <summary>
            /// Event for sending debug console output text.
            /// </summary>
            public event Action<string> DebugOutput;
            /// <summary>
            /// Event fired before the current world is changed.
            /// </summary>
            public event Action PreWorldChanged;
            /// <summary>
            /// Event fired after the current world is changed.
            /// </summary>
            public event Action PostWorldChanged;
            
            public ConcurrentDictionary<Type, TypeProxy> TypeProxies { get; }
                = new ConcurrentDictionary<Type, TypeProxy>();

            public ERenderLibrary RenderLibrary { get; set; } = DefaultRenderLibrary;
            public EAudioLibrary AudioLibrary { get; set; } = DefaultAudioLibrary;
            public EInputLibrary InputLibrary { get; set; } = DefaultInputLibrary;
            public EPhysicsLibrary PhysicsLibrary { get; set; } = DefaultPhysicsLibrary;
            
            private Lazy<EngineSettings> _defaultEngineSettings = new Lazy<EngineSettings>(() => new EngineSettings(), true);

            private Stack<Viewport> CurrentlyRenderingViewports { get; } = new Stack<Viewport>();
            public NetworkConnection Network { get; set; }
            public Server ServerConnection => Network as Server;
            public Client ClientConnection => Network as Client;
            public EngineSingleton Singleton { get; set; }

            public InputAwaiter InputAwaiter { get; set; }

            public GlobalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef { get; set; }
                = new GlobalFileRef<EngineSettings>(Path.Combine(Application.StartupPath, "EngineConfig.xset")) { AllowDynamicConstruction = true, CreateFileIfNonExistent = true };

            public void SetDomainProxy<T>(AppDomain domain, string gamePath) where T : EngineDomainProxy, new()
            {
                string domainName = domain.FriendlyName;
                //PrintLine($"Generating engine proxy of type {typeof(T).GetFriendlyName()} for domain {domainName}");

                bool isUIDomain = domain == AppDomain.CurrentDomain;
                if (isUIDomain)
                {
                    DomainProxy = new T();
                }
                else
                {
                    var proxy = domain.CreateInstanceAndUnwrap<T>();
                    AppDomainHelper.Sponsor(proxy);
                    DomainProxy = proxy;
                }

                DomainProxy.Stopped += DomainProxy_Stopped;
                DomainProxy.Start(gamePath, isUIDomain);
            }

            private void DomainProxy_Stopped()
            {
                PrintLine($"Stopped domain proxy callback occurred in AppDomain {AppDomain.CurrentDomain.FriendlyName}");
            }

            //private Type TypeCreationFailed(string typeDeclaration)
            //    => DomainProxy.CreateType(typeDeclaration);

            public event Action<EngineDomainProxy> ProxySet;
            public event Action<EngineDomainProxy> ProxyUnset;
            
            private EngineDomainProxy _domainProxy = null;
            [Browsable(false)]
            public EngineDomainProxy DomainProxy
            {
                get => _domainProxy;
                set
                {
                    ProxyUnset?.Invoke(_domainProxy);
                    _domainProxy = value;
                    ProxySet?.Invoke(_domainProxy);
                }
            }

            private EngineSettings _cachedSettings;
            public EngineSettings Settings
            {
                get
                {
                    if (_cachedSettings == null)
                        Settings = GetBestSettings();
                    return _cachedSettings;
                }
                set
                {
                    bool singleThreaded = false;
                    bool capFPS = false;
                    bool capUPS = false;
                    float fps = 0.0f;
                    float ups = 0.0f;

                    if (_cachedSettings != null)
                    {
                        singleThreaded = _cachedSettings.SingleThreaded;
                        capFPS = _cachedSettings.CapFPS;
                        capUPS = _cachedSettings.CapUPS;
                        fps = _cachedSettings.TargetFPS;
                        ups = _cachedSettings.TargetUPS;

                        _cachedSettings.SingleThreadedChanged -= Settings_SingleThreadedChanged;
                        _cachedSettings.FramesPerSecondChanged -= Settings_FramesPerSecondChanged;
                        _cachedSettings.UpdatePerSecondChanged -= Settings_UpdatePerSecondChanged;
                    }
                    _cachedSettings = value;
                    if (_cachedSettings != null)
                    {
                        _cachedSettings.SingleThreadedChanged += Settings_SingleThreadedChanged;
                        _cachedSettings.FramesPerSecondChanged += Settings_FramesPerSecondChanged;
                        _cachedSettings.UpdatePerSecondChanged += Settings_UpdatePerSecondChanged;

                        if (_cachedSettings.SingleThreaded != singleThreaded)
                            Settings_SingleThreadedChanged();
                        if (_cachedSettings.CapFPS != capFPS || _cachedSettings.TargetFPS != fps)
                            Settings_FramesPerSecondChanged();
                        if (_cachedSettings.CapUPS != capUPS || _cachedSettings.TargetUPS != ups)
                            Settings_UpdatePerSecondChanged();
                    }
                }
            }

            public void SettingsSourcesChanged(bool cacheBestSettingsNow = false)
            {
                _cachedSettings = null;
                if (cacheBestSettingsNow)
                    Settings = GetBestSettings();
            }
            /// <summary>
            /// Returns settings from the game, from the engine override settings, or the default hardcoded settings.
            /// </summary>
            /// <returns></returns>
            public EngineSettings GetBestSettings() =>
                Game?.EngineSettingsOverrideRef?.File ?? //Game overrides engine settings?
                DefaultEngineSettingsOverrideRef.File ?? //User overrides engine settings?
                _defaultEngineSettings.Value; //Fall back to truly default engine settings

            public void PushRenderingViewport(Viewport viewport) => CurrentlyRenderingViewports.Push(viewport);
            public void PopRenderingViewport() => CurrentlyRenderingViewports.Pop();

            /// <summary>
            /// The settings for the engine, specified by the user.
            /// </summary>
            public UserSettings UserSettings => Game?.UserSettingsRef?.File;

            public ConcurrentDictionary<IWorld, Vec3> RebaseWorldsProcessing = new ConcurrentDictionary<IWorld, Vec3>();
            public ConcurrentDictionary<IWorld, Vec3> RebaseWorldsQueue = new ConcurrentDictionary<IWorld, Vec3>();

            public List<DateTime> _debugTimers = new List<DateTime>();

            public Dictionary<string, int> _fontIndexMatching = new Dictionary<string, int>();
            public PrivateFontCollection _fontCollection = new PrivateFontCollection();

            /// <summary>
            /// The world that is currently being rendered and played in.
            /// </summary>
            public bool IsPaused { get; internal set; } = false;

            /// <summary>
            /// Class containing this computer's specs. Use to adjust engine performance accordingly.
            /// </summary>
            public ComputerInfo ComputerInfo { get; } = ComputerInfo.Analyze();

            public AbstractAudioManager _audioManager;
            public AbstractPhysicsInterface _physicsInterface;

            public Viewport CurrentlyRenderingViewport
            {
                get
                {
                    if (CurrentlyRenderingViewports.Count > 0)
                        return CurrentlyRenderingViewports.Peek();
                    return null;
                }
            }

#if EDITOR
            public EngineEditorState EditorState { get; } = new EngineEditorState();
#endif
        }
    }
}