using AppDomainToolkit;
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
using System.Threading.Tasks;
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

        //TODO: Use cached instance, maintain lifetime, destory on app domain destroy, remake before reloading caches

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
                SettingsSourcesChanged();
            }
        }


#if EDITOR
        private static Lazy<EngineEditorState> EditorStateLazy { get; set; } = new Lazy<EngineEditorState>(() =>
        {
            return new EngineEditorState();
            //var state = Instance.EditorState;
            //AppDomainHelper.Sponsor(state);
            //return state;
        }, LazyThreadSafetyMode.ExecutionAndPublication);
        public static EngineEditorState EditorState => EditorStateLazy.Value;
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

        private static readonly Lazy<EngineSettings> _defaultEngineSettings = new Lazy<EngineSettings>(() => new EngineSettings(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static NetworkConnection Network { get; set; }
        public static Server ServerConnection => Network as Server;
        public static Client ClientConnection => Network as Client;
        public static EngineSingleton Singleton { get; set; }

        public static LocalFileRef<EngineSettings> DefaultEngineSettingsOverrideRef
        {
            get => _defaultEngineSettingsOverrideRef;
            set
            {
                if (_defaultEngineSettingsOverrideRef == value)
                    return;

                if (_defaultEngineSettingsOverrideRef != null)
                    _defaultEngineSettingsOverrideRef.Loaded -= _defaultEngineSettingsOverrideRef_Loaded;

                _defaultEngineSettingsOverrideRef = value;

                if (_defaultEngineSettingsOverrideRef != null)
                    _defaultEngineSettingsOverrideRef.Loaded += _defaultEngineSettingsOverrideRef_Loaded;

                SettingsSourcesChanged();
            }
        }

        private static void _defaultEngineSettingsOverrideRef_Loaded(EngineSettings obj)
            => SettingsSourcesChanged();

        private static LocalFileRef<EngineSettings> _defaultEngineSettingsOverrideRef
            = new LocalFileRef<EngineSettings>(Path.Combine(Application.StartupPath, "EngineConfig.xset"))
            {
                AllowDynamicConstruction = true,
                CreateFileIfNonExistent = true
            };

        private static bool SettingsLoading { get; set; } = false;
        public static EngineSettings Settings 
        {
            get
            {
                if (SettingsLoading)
                    return _defaultEngineSettings.Value;

                if (_settings is null)
                {
                    SettingsLoading = true;
                    _settings = LinkSettings(GetBestSettings());
                    SettingsLoading = false;
                }

                return _settings;
            }
            private set => _settings = value;
        }

        public static void SettingsSourcesChanged()
        {
            SettingsLoading = true;
            UnlinkSettings();
            _settings = LinkSettings(GetBestSettings());
            SettingsLoading = false;
        }

        private static EngineSettings LinkSettings(EngineSettings settings)
        {
            if (settings is null)
                return null;

            settings.SingleThreadedChanged += Settings_SingleThreadedChanged;
            settings.FramesPerSecondChanged += Settings_FramesPerSecondChanged;
            settings.UpdatePerSecondChanged += Settings_UpdatePerSecondChanged;

            Settings_SingleThreadedChanged(settings);
            Settings_FramesPerSecondChanged(settings);
            Settings_UpdatePerSecondChanged(settings);

            return settings;
        }

        private static void UnlinkSettings()
        {
            if (_settings is null)
                return;

            var value = _settings;
            value.SingleThreadedChanged -= Settings_SingleThreadedChanged;
            value.FramesPerSecondChanged -= Settings_FramesPerSecondChanged;
            value.UpdatePerSecondChanged -= Settings_UpdatePerSecondChanged;
        }

        /// <summary>
        /// Returns settings from the game, from the engine override settings, or the default hardcoded settings.
        /// </summary>
        /// <returns></returns>
        public static EngineSettings GetBestSettings()
        {
            //string domainName = AppDomain.CurrentDomain.FriendlyName;

            var gameSettings = Game?.EngineSettingsOverrideRef?.File;
            if (gameSettings != null)
            {
                //Console.WriteLine($"[{domainName}] Loading game settings.");
                return gameSettings;
            }

            var defaultOverrideSettings = DefaultEngineSettingsOverrideRef?.File;
            if (defaultOverrideSettings != null)
            {
                //Console.WriteLine($"[{domainName}] Loading default override settings.");
                return defaultOverrideSettings;
            }

            //Console.WriteLine($"[{domainName}] Loading default settings.");
            return _defaultEngineSettings.Value;
        }

        /// <summary>
        /// Returns settings from the game, from the engine override settings, or the default hardcoded settings.
        /// </summary>
        /// <returns></returns>
        public static async Task<EngineSettings> GetBestSettingsAsync()
        {
            //string domainName = AppDomain.CurrentDomain.FriendlyName;

            var ref1 = Game?.EngineSettingsOverrideRef;
            if (ref1 != null)
            {
                var settings = await ref1.GetInstanceAsync();
                if (settings != null)
                {
                    //Console.WriteLine($"[{domainName}] Loading game settings.");
                    return settings;
                }
            }

            var ref2 = DefaultEngineSettingsOverrideRef;
            if (ref2 != null)
            {
                var settings = await ref2.GetInstanceAsync();
                if (settings != null)
                {
                    //Console.WriteLine($"[{domainName}] Loading default override settings.");
                    return settings;
                }
            }

            //Console.WriteLine($"[{domainName}] Loading default settings.");
            return _defaultEngineSettings.Value;
        }

        /// <summary>
        /// The settings for the engine, specified by the user.
        /// </summary>
        public static UserSettings UserSettings => Game?.UserSettingsRef?.File;

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine performance accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo { get; } = ComputerInfo.Analyze();

        public static AbstractAudioManager _audioManager;
        public static AbstractPhysicsInterface _physicsInterface;
        private static EngineSettings _settings;

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
        public partial class InternalEnginePersistentSingleton : MarshalByRefObject
        {
            private void SafePrintLine(string str)
            {
                Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}]" + str);
            }

            private void StopProxy(EngineDomainProxy proxy)
            {
                if (proxy is null)
                    return;

                DomainProxyDestroying?.Invoke(proxy);
                proxy.Stop();
                proxy.Stopped -= DomainProxy_Stopped;

                SafePrintLine($"DomainProxy stopped for accessing {proxy.Domain.FriendlyName}.");
            }
            private void StartProxy(EngineDomainProxy proxy, string gamePath, bool isUIDomain)
            {
                proxy.Start(gamePath, isUIDomain);
                proxy.Stopped += DomainProxy_Stopped;
                DomainProxyCreated?.Invoke(proxy);

                SafePrintLine($"DomainProxy started for accessing {(isUIDomain ? "this domain" : proxy.Domain.FriendlyName)}.");
            }

            public void SetDomainProxy<T>(
                AppDomain domain,
                AppDomainContext<AssemblyTargetLoader, PathBasedAssemblyResolver> prevDomain,
                string gamePath)
                where T : EngineDomainProxy, new()
            {
                SafePrintLine($"Generating new domain proxy of type {typeof(T).GetFriendlyName()} for domain {domain.FriendlyName}");

                bool isUIDomain = domain == AppDomain.CurrentDomain;
                EngineDomainProxy newProxy;
                if (isUIDomain)
                    newProxy = new T();
                else
                {
                    var proxy = domain.CreateInstanceAndUnwrap<T>();
                    AppDomainHelper.Sponsor(proxy);
                    newProxy = proxy;
                }

                EngineDomainProxy oldProxy = DomainProxy;
                DomainProxy = newProxy;

                StopProxy(oldProxy);
                DestroyDomain(prevDomain);
                StartProxy(newProxy, gamePath, isUIDomain);
            }

            private void DestroyDomain(AppDomainContext<AssemblyTargetLoader, PathBasedAssemblyResolver> prevDomain)
            {
                if (prevDomain is null)
                    return;

                SafePrintLine($"Destroying game domain {prevDomain.Domain.FriendlyName}");

                AppDomainHelper.ResetAppDomainCache();
                SafePrintLine("Active domains before destroy: " + AppDomainHelper.AppDomainStringList);

                prevDomain.Dispose();

                AppDomainHelper.ResetAppDomainCache();
                SafePrintLine("Active domains after destroy: " + AppDomainHelper.AppDomainStringList);
            }

            private void DomainProxy_Stopped()
            {
                SafePrintLine($"DomainProxy stopped.");
            }

            //private Type TypeCreationFailed(string typeDeclaration)
            //    => DomainProxy.CreateType(typeDeclaration);

            public event Action<EngineDomainProxy> DomainProxyCreated;
            public event Action<EngineDomainProxy> DomainProxyDestroying;

            [Browsable(false)]
            public EngineDomainProxy DomainProxy { get; private set; } = null;

            //#if EDITOR
            //            public EngineEditorState EditorState { get; } = new EngineEditorState();
            //#endif
        }
    }
}