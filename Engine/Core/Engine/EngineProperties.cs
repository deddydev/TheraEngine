using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.CompilerServices;
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

        //TODO: Use cached instance, maintain lifetime, destory on app domain destory, remake before reloading caches

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


#if EDITOR
        public static EngineEditorState EditorState
        {
            get
            {
                var state = Instance.EditorState;
                AppDomainHelper.Sponsor(state);
                return state;
            }
        }
#endif

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
            get => _renderLibrary;
            internal set
            {
                _renderLibrary = value;
                RenderLibraryChanged();
            }
        }
        private static ERenderLibrary _renderLibrary = DefaultRenderLibrary;
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
            get => _audioLibrary;
            internal set
            {
                _audioLibrary = value;
                RetrieveAudioManager();
            }
        }
        private static EAudioLibrary _audioLibrary = DefaultAudioLibrary;
        private static void RetrieveAudioManager()
        {
            switch (AudioLibrary)
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
            get => _physicsLibrary;
            internal set
            {
                _physicsLibrary = value;
                RetrievePhysicsInterface();
            }
        }
        private static EPhysicsLibrary _physicsLibrary = DefaultPhysicsLibrary;

        private static void RetrievePhysicsInterface()
        {
            switch (PhysicsLibrary)
            {
                default:
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
            get => _inputLibrary;
            internal set
            {
                _inputLibrary = value;
                InputLibraryChanged();
            }
        }
        private static EInputLibrary _inputLibrary = DefaultInputLibrary;

        public static InputAwaiter InputAwaiter { get; set; }

        private static void InputLibraryChanged()
        {
            if (!AppDomainHelper.IsGameDomain && !AppDomainHelper.IsGameDomainAlsoPrimary)
                return;

            switch (InputLibrary)
            {
                default:
                case EInputLibrary.OpenTK:
                    InputAwaiter = new TKInputAwaiter(FoundInput);
                    break;
                case EInputLibrary.XInput:
                    InputAwaiter = new DXInputAwaiter(FoundInput);
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
                    //string domain = AppDomain.CurrentDomain.FriendlyName;
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
                if (_physicsInterface is null)
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
                if (_audioManager is null)
                    throw new InvalidOperationException("No audio library set.");
                return _audioManager;
            }
        }

        public static bool IsSingleThreaded => Timer.IsSingleThreaded;

        public static EngineDomainProxy DomainProxy => Instance.DomainProxy;

        #endregion

        /// <summary>
        /// The world that is currently being rendered and played in.
        /// </summary>
        public static bool IsPaused { get; private set; } = false;

        /// <summary>
        /// Event fired before the current world is changed.
        /// </summary>
        public static event Action PreWorldChanged;
        /// <summary>
        /// Event fired after the current world is changed.
        /// </summary>
        public static event Action PostWorldChanged;

        public static ConcurrentDictionary<IWorld, Vec3> RebaseWorldsProcessing = new ConcurrentDictionary<IWorld, Vec3>();
        public static ConcurrentDictionary<IWorld, Vec3> RebaseWorldsQueue = new ConcurrentDictionary<IWorld, Vec3>();

        private static List<DateTime> _debugTimers = new List<DateTime>();

        private static Stack<Viewport> CurrentlyRenderingViewports { get; } = new Stack<Viewport>();
        public static void PushRenderingViewport(Viewport viewport) => CurrentlyRenderingViewports.Push(viewport);
        public static void PopRenderingViewport() => CurrentlyRenderingViewports.Pop();

        public static DelBeginOperation BeginOperation;
        public static DelEndOperation EndOperation;

        public static event Action GotFocus;
        public static event Action LostFocus;

        //public RenderContext CapturedRenderContext { get; set; }

        /// <summary>
        /// Event for when the engine is paused or unpaused and by which player.
        /// </summary>
        public static event Action<bool, ELocalPlayerIndex> PauseChanged;
        /// <summary>
        /// Event for sending debug console output text.
        /// </summary>
        public static event Action<string> DebugOutput;

        private static Lazy<EngineSettings> _defaultEngineSettings = new Lazy<EngineSettings>(() => new EngineSettings(), true);

        public static NetworkConnection Network { get; set; }
        public static Server ServerConnection => Network as Server;
        public static Client ClientConnection => Network as Client;
        public static EngineSingleton Singleton { get; set; }

        public static GlobalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef { get; set; }
            = new GlobalFileRef<EngineSettings>(Path.Combine(Application.StartupPath, "EngineConfig.xset")) { AllowDynamicConstruction = true, CreateFileIfNonExistent = true };

        private static EngineSettings _cachedSettings;
        public static EngineSettings Settings
        {
            get
            {
                if (_cachedSettings is null)
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

        public static void SettingsSourcesChanged(bool cacheBestSettingsNow = false)
        {
            _cachedSettings = null;
            if (cacheBestSettingsNow)
                Settings = GetBestSettings();
        }
        /// <summary>
        /// Returns settings from the game, from the engine override settings, or the default hardcoded settings.
        /// </summary>
        /// <returns></returns>
        public static EngineSettings GetBestSettings() =>
            Game?.EngineSettingsOverrideRef?.File ?? //Game overrides engine settings?
            DefaultEngineSettingsOverrideRef.File ?? //User overrides engine settings?
            _defaultEngineSettings.Value; //Fall back to truly default engine settings

        /// <summary>
        /// The settings for the engine, specified by the user.
        /// </summary>
        public static UserSettings UserSettings => Game?.UserSettingsRef?.File;

        public static Dictionary<string, int> _fontIndexMatching = new Dictionary<string, int>();
        public static PrivateFontCollection _fontCollection = new PrivateFontCollection();

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine performance accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo { get; } = ComputerInfo.Analyze();

        public static AbstractAudioManager _audioManager;
        public static AbstractPhysicsInterface _physicsInterface;

        public static Viewport CurrentlyRenderingViewport
        {
            get
            {
                if (CurrentlyRenderingViewports.Count > 0)
                    return CurrentlyRenderingViewports.Peek();
                return null;
            }
        }

        /// <summary>
        /// This singleton represents the engine for all AppDomains, 
        /// residing in the UI domain and marshalled to all others.
        /// Statics are not persistent across AppDomains so the static engine members
        /// are completely separate entities per AppDomain, which is why this singleton exists.
        /// </summary>
        public partial class InternalEnginePersistentSingleton: MarshalByRefObject
        {
            public void DestroyDomainProxy()
            {
                if (DomainProxy != null)
                {
                    DomainProxy.Stop();
                    DomainProxy.Stopped -= DomainProxy_Stopped;
                    DomainProxyUnset?.Invoke(DomainProxy);
                    DomainProxy = null;
                }
            }
            public void SetDomainProxy<T>(AppDomain domain, string gamePath) where T : EngineDomainProxy, new()
            {
                Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] DOMAIN PROXY: {domain.FriendlyName} {gamePath}");

                string domainName = domain.FriendlyName;
                //PrintLine($"Generating engine proxy of type {typeof(T).GetFriendlyName()} for domain {domainName}");

                bool isUIDomain = domain == AppDomain.CurrentDomain;
                EngineDomainProxy domainProxy;
                if (isUIDomain)
                    domainProxy = new T();
                else
                {
                    var proxy = domain.CreateInstanceAndUnwrap<T>();
                    AppDomainHelper.Sponsor(proxy);
                    domainProxy = proxy;
                }

                domainProxy.Stopped += DomainProxy_Stopped;

                DomainProxy = domainProxy;
                DomainProxy.Start(gamePath, isUIDomain);
                DomainProxySet?.Invoke(DomainProxy);

                PrintLine($"DomainProxy started for accessing {(isUIDomain ? "this domain" : domainProxy.Domain.FriendlyName)}.");
            }

            private void DomainProxy_Stopped()
            {
                PrintLine($"DomainProxy stopped.");
            }

            //private Type TypeCreationFailed(string typeDeclaration)
            //    => DomainProxy.CreateType(typeDeclaration);

            public event Action<EngineDomainProxy> DomainProxySet;
            public event Action<EngineDomainProxy> DomainProxyUnset;

            [Browsable(false)]
            public EngineDomainProxy DomainProxy { get; private set; } = null;

            public EngineEditorState EditorState { get; } = new EngineEditorState();
        }
    }
}