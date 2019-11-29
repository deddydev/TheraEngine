using Extensions;
using WindowsNativeInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.GameModes;
using TheraEngine.Input.Devices;
using TheraEngine.Physics.ContactTesting;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using Valve.VR;
using TheraEngine.Core.Reflection;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace TheraEngine
{
    public enum EOutputVerbosity
    {
        None,
        Minimal,
        Normal,
        Verbose,
    }
    public static partial class Engine
    {
        public static float CurrentFramesPerSecond => Timer.RenderFrequency;
        public static float CurrentUpdatesPerSecond => Timer.UpdateFrequency;

        public static ColorF4 InvalidColor { get; } = Color.Magenta;

        #region Startup/Shutdown
        private class EngineTraceListener : TraceListener
        {
            public override void WriteLine(string message)
                => Write(message + Environment.NewLine);
            public override void Write(string message)
            {
                OutputString += message;
                Instance.OnDebugOutput(message);
            }
        }
        public static bool FontsLoaded { get; private set; } = false;
        static Engine()
        {
            Debug.Listeners.Add(new EngineTraceListener());
            //PrintLine("Constructing static engine class.");

            //TODO: change tick order to int. 
            //Allocate tick order values as they are requested in a dictionary.
            TickLists = new List<DelTick>[45];
            for (int i = 0; i < TickLists.Length; ++i)
                TickLists[i] = new List<DelTick>();

            Timer.UpdateFrame += EngineTick;
            Timer.SwapBuffers += SwapBuffers;

            RenderLibraryChanged();
            RetrieveAudioManager();
            //InputLibraryChanged();
            RetrievePhysicsInterface();

            //LoadCustomFonts();
        }

        /// <summary>
        /// Call this in the program's main method run a game in the engine.
        /// Will create a render form, initialize the engine, and start the game, so no other methods are needed.
        /// </summary>
        /// <param name="game">The game to play.</param>
        public static void RunSingleInstanceOf(string gamePath)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RenderForm(gamePath));
        }

        /// <summary>
        /// Call this to shut down the engine, deallocate all resources, and close the application.
        /// </summary>
        public static void CloseApplication()
        {
            ShutDown();
            Application.Exit();
        }
        /// <summary>
        /// Sets the game information for all code to grab preferences from.
        /// Call this BEFORE ANYTHING ELSE!
        /// </summary>
        public static void SetGame(TGame game)
        {
            if (Game == game)
                return;

            MainThreadID = Thread.CurrentThread.ManagedThreadId;
            if (Game != null)
            {

            }
            Game = game;
            if (Game != null)
            {

            }
        }
        /// <summary>
        /// Helper to collect all types from all loaded assemblies that match the given predicate.
        /// </summary>
        /// <param name="matchPredicate">What determines if the type is a match or not.</param>
        /// <param name="resetTypeCache">If true, recollects all assembly types manually and re-caches them.</param>
        /// <returns>All types that match the predicate.</returns>
        //public static IEnumerable<Type> FindTypes(Predicate<Type> matchPredicate, params Assembly[] assemblies)
        //{
        //    //TODO: search all appdomains, return marshalbyrefobject list containing typeproxies
        //    IEnumerable<Assembly> search;

        //    if (assemblies is null || assemblies.Length == 0)
        //    {
        //        search = AppDomain.CurrentDomain.GetAssemblies();
        //        ////PrintLine("FindTypes; returning assemblies from domains:");
        //        //var domains = EnumAppDomains();
        //        //search = domains.SelectMany(x =>
        //        //{
        //        //    //PrintLine(x.FriendlyName);
        //        //    try
        //        //    {
        //        //        return x.GetAssemblies();
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        LogWarning($"Unable to load assemblies from {nameof(AppDomain)} {x.FriendlyName}");
        //        //        return new Assembly[0];
        //        //    }
        //        //});
        //    }
        //    else
        //        search = assemblies;

        //    search = search.Where(x => !x.IsDynamic);

        //    //if (includeEngineAssembly)
        //    //{
        //    //    Assembly engine = Assembly.GetExecutingAssembly();
        //    //    if (!search.Contains(engine))
        //    //        search = search.Append(engine);
        //    //}

        //    var allTypes = search.SelectMany(x => x.GetExportedTypes());

        //    return allTypes.Where(x => matchPredicate(x)).OrderBy(x => x.Name);
        //}
        
        public static void SetWorldPanel(RenderContext panel, bool registerTickNow = true)
        {
            RenderContext.WorldPanel = panel;
            //if (registerTickNow)
            //    panel?.RegisterTick();
        }
        /// <summary>
        /// Initializes the engine to its beginning state.
        /// Call AFTER SetGame is called and all initial render panels are created and ready.
        /// </summary>
        public static void Initialize()
        {
            var userSet = UserSettings;
            var engineSet = Settings;

            if (engineSet != null)
            {
                TargetFramesPerSecond = engineSet.CapFPS ? engineSet.TargetFPS.ClampMin(1.0f) : 0.0f;
                TargetUpdatesPerSecond = engineSet.CapUPS ? engineSet.TargetUPS.ClampMin(1.0f) : 0.0f;
            }
            else
            {
                TargetFramesPerSecond = 0.0f;
                TargetUpdatesPerSecond = 0.0f;
            }

            if (Game != null)
            {
                if (userSet != null)
                {
                    RenderLibrary = userSet.RenderLibrary;
                    AudioLibrary = userSet.AudioLibrary;
                    InputLibrary = userSet.InputLibrary;
                    PhysicsLibrary = userSet.PhysicsLibrary;
                }
                else
                {
                    RenderLibrary = DefaultRenderLibrary;
                    AudioLibrary = DefaultAudioLibrary;
                    InputLibrary = DefaultInputLibrary;
                    PhysicsLibrary = DefaultPhysicsLibrary;
                }

                //Set initial world (this would generally be a world for opening videos or the main menu)
                if (Game.OpeningWorldRef?.FileExists ?? false)
                    SetCurrentWorld(Game.OpeningWorldRef.File);

                //Preload transition world now
                //await Game.TransitionWorldRef.LoadNewInstanceAsync();
            }

            //SteamAPI.Init();
            //AppId_t appId = new AppId_t(408u);
            //SteamAPI.RestartAppIfNecessary(appId);
            //bool ret = GameServer.Init(0, 8766, 27015, 27016, EServerMode.eServerModeNoAuthentication, "1.0.0.0");
            ////SteamGameServer.ForceHeartbeat();
            ////var steamID = SteamUser.GetSteamID();
            //int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
            //for (int i = 0; i < friendCount; ++i)
            //{
            //    CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
            //    int r = SteamFriends.GetFriendMessage(friendSteamId, 0, out string data, 0, out EChatEntryType type);
            //    string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
            //    EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendSteamId);
            //    PrintLine(friendName + " is " + friendState.ToString().Substring(15));
            //}

            //InitializeVR();

            //VariableStringTable table = new VariableStringTable();
            //string result = table.AnalyzeString("Wow this is really cool at <currentTime:hh:mm:ss tt> for <localPlayerName:0>!", out (int index, int length, bool redraw)[] varLocs);
            //string result2 = table.SolveAnalyzedString(result, varLocs.Select(x => (x.index, x.length)).ToArray());
            //PrintLine(result2);
        }

        public static EVRInitError InitializeVR()
        {
            if (!OpenVR.IsRuntimeInstalled())
            {
                PrintLine("VR runtime not installed.");
                return EVRInitError.Init_InstallationNotFound;
            }
            if (!OpenVR.IsHmdPresent())
            {
                PrintLine("VR headset not found.");
                return EVRInitError.Init_HmdNotFound;
            }

            EVRInitError peError = EVRInitError.None;

            try
            {
                if (OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Scene) is null)
                    LogWarning(peError.ToString());
                else
                    PrintLine("VR system initialized successfully.");
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return peError;
        }

        public static bool ShuttingDown { get; private set; }

        /// <summary>
        /// Call this to stop the engine and dispose of all allocated data.
        /// </summary>
        public static void ShutDown()
        {
            ShuttingDown = true;

            //SteamAPI.Shutdown();
            //Stop();
            SetCurrentWorld(null);

            //IEnumerable<IFileObject>
            //files = LocalFileInstances.SelectMany(x => x.Value);
            //foreach (IFileObject o in files)
            //    o?.Unload();
            //files = GlobalFileInstances.Values;
            //foreach (IFileObject o in files)
            //    o?.Unload();

            var contexts = new List<RenderContext>(RenderContext.BoundContexts);
            foreach (RenderContext c in contexts)
                c?.Dispose();
            
            ShuttingDown = false;
        }
        #endregion

        #region Timing
        /// <summary>
        /// Starts deployment of update and render ticks.
        /// </summary>
        public static void Run()
        {
            EngineSettings settings = Settings;
            Timer.Run(settings?.SingleThreaded ?? false);
        }

        private static void Settings_UpdatePerSecondChanged()
        {
            EngineSettings settings = Settings;
            TargetUpdatesPerSecond = settings.CapUPS ? settings.TargetUPS.ClampMin(1.0f) : 0.0f;
        }
        private static void Settings_FramesPerSecondChanged()
        {
            EngineSettings settings = Settings;
            TargetFramesPerSecond = settings.CapFPS ? settings.TargetFPS.ClampMin(1.0f) : 0.0f;
        }
        private static void Settings_SingleThreadedChanged()
        {
            if (Timer.IsRunning)
                Timer.IsSingleThreaded = Settings.SingleThreaded;
        }

        /// <summary>
        /// The index of the currently ticking list of functions (group + order + pause)
        /// </summary>
        public static int CurrentTickList = -1;
        /// <summary>
        /// Queue for adding to or removing from the currently ticking list
        /// </summary>
        public static ConcurrentQueue<Tuple<bool, DelTick>> TickListQueue = new ConcurrentQueue<Tuple<bool, DelTick>>();
        public static List<DelTick>[] TickLists;

        /// <summary>
        /// HALTS update and render ticks. Not recommended for use as this literally halts all visuals and user input.
        /// </summary>
        public static void Stop()
        {
            EngineSettings settings = Settings;
            settings.SingleThreadedChanged -= Settings_SingleThreadedChanged;
            settings.FramesPerSecondChanged -= Settings_FramesPerSecondChanged;
            settings.UpdatePerSecondChanged -= Settings_UpdatePerSecondChanged;
            Timer.Stop();
        }

        /// <summary>
        /// Registers the given function to be called every update tick.
        /// </summary>
        public static void RegisterTick(
            EventHandler<FrameEventArgs> render,
            EventHandler<FrameEventArgs> update,
            Action swapBuffers)
        {
            if (render != null)
                Timer.RenderFrame += render;
            if (update != null)
                Update += update;
            if (swapBuffers != null)
                Timer.SwapBuffers += swapBuffers;
        }
        /// <summary>
        /// Registers the given function to be called every render tick.
        /// </summary>
        public static void UnregisterTick(
            EventHandler<FrameEventArgs> render,
            EventHandler<FrameEventArgs> update,
            Action swapBuffers)
        {
            if (render != null)
                Timer.RenderFrame -= render;
            if (update != null)
                Update -= update;
            if (swapBuffers != null)
                Timer.SwapBuffers -= swapBuffers;
        }
        /// <summary>
        /// Registers a method to execute in a specific order every update tick.
        /// </summary>
        /// <param name="group">The first grouping of when to tick: before, after, or during the physics tick update.</param>
        /// <param name="order">The order to execute the function within its group.</param>
        /// <param name="function">The function to execute per update tick.</param>
        /// <param name="pausedBehavior">If the function should even execute at all, depending on the pause state.</param>
        public static void RegisterTick(ETickGroup group, ETickOrder order, DelTick function, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            if (function is null)
                return;
            
            var list = GetTickList(group, order, pausedBehavior);
            if (list.Contains(function))
                return;

            int tickIndex = (int)group + (int)order + (int)pausedBehavior;
            if (CurrentTickList == tickIndex)
                TickListQueue.Enqueue(new Tuple<bool, DelTick>(true, function));
            else
                list.Add(function);
        }
        /// <summary>
        /// Stops running a tick method that was previously registered with the same parameters.
        /// </summary>
        public static void UnregisterTick(ETickGroup group, ETickOrder order, DelTick function, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            if (function is null)
                return;
            
            var list = GetTickList(group, order, pausedBehavior);
            int tickIndex = (int)group + (int)order + (int)pausedBehavior;
            if (CurrentTickList == tickIndex)
                TickListQueue.Enqueue(new Tuple<bool, DelTick>(false, function));
            else
                list.Remove(function);
        }
        /// <summary>
        /// Gets a list of items to tick (in no particular order) that were registered with the following parameters.
        /// </summary>
        private static List<DelTick> GetTickList(ETickGroup group, ETickOrder order, EInputPauseType pausedBehavior)
            => TickLists[(int)group + (int)order + (int)pausedBehavior];

        public static event EventHandler<FrameEventArgs> Update;

        /// <summary>
        /// Ticks the before, during, and after physics groups. Also steps the physics simulation during the during physics tick group.
        /// Does not tick physics if paused.
        /// </summary>
        private static void EngineTick(object sender, FrameEventArgs e)
        {
            ClearMarkers();
            //Network?.RecievePackets();
            
            float delta = e.Time * TimeDilation;

            TickGroup(ETickGroup.PrePhysics, delta);

            if (!IsPaused)
                World?.StepSimulation(delta);

            TickGroup(ETickGroup.PostPhysics, delta);

            OnUpdate(sender, e);

            //Network?.UpdatePacketQueue(e.Time);

            //SteamAPI.RunCallbacks();
            PrintMarkers();
        }

        private static Dictionary<int, List<string>> Sequences { get; } = new Dictionary<int, List<string>>();
        public static void SequenceMarker(int id, [CallerMemberName]string name = "")
        {
            if (Sequences.ContainsKey(id))
                Sequences[id].Add(name);
            else
                Sequences[id] = new List<string>() { name };
        }
        private static void PrintMarkers()
        {
            foreach (var kv in Sequences)
            {
                PrintLine($"Sequence {kv.Key}: {kv.Value.ToStringList(",")}");
            }
        }
        private static void ClearMarkers()
        {
            Sequences.Clear();
        }

        private static void OnUpdate(object sender, FrameEventArgs e) 
            => Update?.Invoke(sender, e);

        /// <summary>
        /// Ticks all lists of methods registered to this group.
        /// Goes through timers, input, animation, logic and scene ticks in that order,
        /// and executes methods based on whether the engine is paused or not (or regardless).
        /// </summary>
        private static void TickGroup(ETickGroup group, float delta)
        {
            int start = (int)group;
            for (int i = start; i < start + 15; i += 3)
            {
                //These need to be processed in order
                for (int j = 0; j < 3; ++j)
                //Parallel.For(0, 3, (int j) => 
                {
                    if (j == 0 || (j == 1 && !IsPaused) || (j == 2 && IsPaused))
                        TickList(i + j, delta);
                }
                //);
            }
        }
        /// <summary>
        /// Ticks the list of items at the given index (created by adding the tick group, order and pause type together).
        /// </summary>
        public static void TickList(int index, float delta)
        {
            List<DelTick> currentList = TickLists[CurrentTickList = index];

            //These can be processed in parallel, as they are in the same tick list and group
            Parallel.ForEach(currentList, currentFunc => currentFunc(delta));
            //currentList.ForEach(x => x(delta));

            //Add or remove the list of methods that tried to register to or unregister from this group while it was ticking.
            while (!TickListQueue.IsEmpty && TickListQueue.TryDequeue(out Tuple<bool, DelTick> result))
            {
                if (result.Item1)
                    currentList.Add(result.Item2);
                else
                    currentList.Remove(result.Item2);
            }

            CurrentTickList = -1;
        }
        /// <summary>
        /// Tells the engine to play in a new world.
        /// </summary>
        /// <param name="world">The world to play in.</param>
        /// <param name="unloadPrevious">Whether or not the engine should deallocate all resources utilized by the current world before loading the new one.</param>
        public static void SetCurrentWorld(IWorld world)
        {
            if (World == world)
                return;

            PreWorldChanged?.Invoke();

            World?.EndPlay();
            World = world;
            World?.BeginPlay();

            PostWorldChanged?.Invoke();
        }
        public static void QueueRebaseOrigin(IWorld world, Vec3 point)
        {
            if (!world.IsRebasingOrigin)
                RebaseWorldsQueue.AddOrUpdate(world, t => point, (t, t2) => point);
        }
        /// <summary>
        /// Called when the input awaiter discovers a new input device.
        /// </summary>
        /// <param name="device">The device that was found.</param>
        internal static void FoundInput(InputDevice device)
        {
            World?.CurrentGameMode?.FoundInput(device);
        }
        public static void SwapBuffers()
        {
            THelpers.Swap(ref RebaseWorldsProcessing, ref RebaseWorldsQueue);
            RebaseWorldsProcessing.ForEach(x => x.Key.RebaseOrigin(x.Value));
            RebaseWorldsProcessing.Clear();
        }
        /// <summary>
        /// Starts a quick timer to track the number of sceonds elapsed.
        /// Returns the id of the timer.
        /// </summary>
        public static int StartTimer()
        {
            int id = _debugTimers.Count;
            _debugTimers.Add(DateTime.Now);
            return id;
        }
        /// <summary>
        /// Ends the timer and returns the amount of time elapsed, in seconds.
        /// </summary>
        /// <param name="id">The id of the timer.</param>
        public static float EndTimer(int id)
        {
            float seconds = (float)(DateTime.Now - _debugTimers[id]).TotalSeconds;
            _debugTimers.RemoveAt(id);
            return seconds;
        }

        /// <summary>
        /// Toggles the pause state. If currently paused, will unpause. If currently unpaused, will pause.
        /// </summary>
        /// <param name="toggler">The player that's pausing the game.</param>
        public static void TogglePause(ELocalPlayerIndex toggler)
        {
            SetPaused(!IsPaused, toggler);
        }
        /// <summary>
        /// Sets the pause state regardless of what it is currently.
        /// </summary>
        /// <param name="wantsPause">The desired pause state.</param>
        /// <param name="toggler">The player that's pausing the game.</param>
        public static void SetPaused(bool wantsPause, ELocalPlayerIndex toggler, bool force = false)
        {
            if ((!force && wantsPause && World.CurrentGameMode.DisallowPausing) || IsPaused == wantsPause)
                return;

            IsPaused = wantsPause;
            Instance.OnPauseChanged(IsPaused, toggler);
            PrintLine("Engine{0}paused.", IsPaused ? " " : " un");
        }
        public static void Pause(ELocalPlayerIndex toggler, bool force = false)
            => SetPaused(true, toggler, force);
        public static void Unpause(ELocalPlayerIndex toggler, bool force = false)
            => SetPaused(false, toggler, force);
        #endregion

        #region Output
        //        public static void LogError(string message, params string[] args)
        //        {
        //#if DEBUG
        //            if (args.Length != 0)
        //                message = string.Format(message, args);
        //            throw new Exception(message);
        //#endif
        //        }
        public static string OutputString { get; private set; }
        public static bool DesignMode 
            => Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) >= 0;

        public static ConcurrentDictionary<string, DateTime> RecentMessageCache = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void PrintLine(string message, params object[] args)
            => PrintLine(EOutputVerbosity.Normal, message, args);
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void PrintLine(EOutputVerbosity verbosity, string message, params object[] args)
            => PrintLine(verbosity, true, message, args);
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void PrintLine(EOutputVerbosity verbosity, bool debugOnly, string message, params object[] args)
            => PrintLine(verbosity, debugOnly, false, false, false, 0, 0, message, args);
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void PrintLine(
            EOutputVerbosity verbosity,
            bool debugOnly,
            bool printDate,
            bool printAppDomain,
            bool printStackTrace,
            int stackTraceIgnoredLineCount,
            int stackTraceIncludedLineCount,
            string message,
            params object[] args)
        {
#if DEBUG || EDITOR

            if (verbosity > Settings.OutputVerbosity)
                return;

            if (args.Length > 0)
                message = string.Format(message, args);

            if (printStackTrace)
                message += Environment.NewLine + GetStackTrace(stackTraceIgnoredLineCount, stackTraceIncludedLineCount);

            DateTime now = DateTime.Now;

            double recentness = Settings.AllowedOutputRecentnessSeconds;
            if (recentness > 0.0)
            {
                List<string> removeKeys = new List<string>();
                RecentMessageCache.ForEach(x =>
                {
                    TimeSpan span = now - x.Value;
                    if (span.TotalSeconds >= recentness)
                        removeKeys.Add(x.Key);
                });
                removeKeys.ForEach(x => RecentMessageCache.TryRemove(x, out _));

                if (RecentMessageCache.ContainsKey(message))
                {
                    //Messages already cleaned above, just return here

                    //TimeSpan span = now - RecentMessages[message];
                    //if (span.TotalSeconds <= AllowedOutputRecentness)
                    return;
                }
                else
                    RecentMessageCache.TryAdd(message, now);
            }

            bool printDomain = printAppDomain || Settings.PrintAppDomainInOutput;

            if (printDate && printDomain)
                message = $"[{AppDomain.CurrentDomain.FriendlyName} {now}] " + message;
            else if (printDomain)
                message = $"[{AppDomain.CurrentDomain.FriendlyName}] " + message;
            else if (printDate)
                message = $"[{now}] " + message;

            if (debugOnly)
                Debug.Print(message);
            else
                Trace.WriteLine(message);
#endif
        }

        public static void LogException(Exception ex)
        {
#if DEBUG || EDITOR
            PrintLine(EOutputVerbosity.Verbose, false, ex.ToString());
#endif
        }
        public static void LogWarning(string message, int lineIgnoreCount = 0, int includedLineCount = 1)
        {
#if DEBUG || EDITOR
            PrintLine(EOutputVerbosity.Verbose, true, false, false, true, 4 + lineIgnoreCount, includedLineCount, message);
#endif
        }

        public static string GetStackTrace(int lineIgnoreCount = 3, int includedLineCount = -1, bool ignoreBeforeWndProc = true)
        {
            //Format and print stack trace
            string stackTrace = Environment.StackTrace;
            string atStr = "   at ";
            
            int at4th = stackTrace.FindOccurrence(0, lineIgnoreCount, atStr);
            if (at4th > 0)
                stackTrace = stackTrace.Substring(at4th);

            if (ignoreBeforeWndProc)
            {
                //Everything before wndProc is almost always irrelevant
                int wndProc = stackTrace.IndexOf("WndProc(Message& m)");
                if (wndProc > 0)
                {
                    int at = stackTrace.FindFirstReverse(wndProc, atStr);
                    if (at > 0)
                        stackTrace = stackTrace.Substring(0, at);
                }
            }

            if (includedLineCount >= 0)
            {
                int atXth = stackTrace.FindOccurrence(0, includedLineCount, atStr);
                if (atXth > 0)
                    stackTrace = stackTrace.Substring(0, atXth);
            }

            return stackTrace;
        }

        #endregion

        #region Game Modes
        /// <summary>
        /// Retrieves the current world's overridden game mode or the game's game mode if not overriden.
        /// </summary>
        public static IGameMode GetGameMode()
            => World?.Settings?.DefaultGameModeRef?.File ?? Game.DefaultGameModeRef?.File;
        #endregion

        /// <summary>
        /// Performs a ray trace in the current world referenced by the engine.
        /// To perform a ray trace in a specific world, use World.PhysicsWorld.RayTrace(ShapeTrace result); 
        /// </summary>
        public static bool RayTrace(RayTrace result, IWorld world)
            => (world ?? World)?.PhysicsWorld3D?.RayTrace(result) ?? false;
        /// <summary>
        /// Performs a shape trace in the current world referenced by the engine.
        /// To perform a shape trace in a specific world, use World.PhysicsWorld.ShapeTrace(ShapeTrace result); 
        /// </summary>
        public static bool ShapeTrace(ShapeTrace result, IWorld world) 
            => (world ?? World)?.PhysicsWorld3D?.ShapeTrace(result) ?? false;
        
        public static bool ContactTest(ContactTest result, IWorld world)
           => (world ?? World)?.PhysicsWorld3D?.ContactTest(result) ?? false;

        //public static BaseGameMode ActiveGameMode { get; set; }
        //public static T ActiveGameModeAs<T>() where T : BaseGameMode
        //    => ActiveGameMode as T;

        /// <summary>
        /// Retrieves the world viewport with the same index.
        /// </summary>
        //public static Viewport GetViewport(ELocalPlayerIndex index)
        //    => BaseRenderPanel.WorldPanel?.GetViewport(index);

        public delegate int DelBeginOperation(string operationMessage, string finishedMessage, out Progress<float> progress, out CancellationTokenSource cancel, TimeSpan? maxOperationTime = null);
        public delegate void DelEndOperation(int operationId);

        public static bool IsFocused { get; private set; } = true;
        public static Random Random { get; } = new Random();

        /// <summary>
        /// Returns true if any window has focus.
        /// </summary>
        private static bool CheckFocus()
        {
            var activatedHandle = NativeMethods.GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
                return false; //No window is currently activated

            int procId = Process.GetCurrentProcess().Id;
            NativeMethods.GetWindowThreadProcessId(activatedHandle, out int activeProcId);

            return activeProcId == procId;
        }
        public static void FocusChanged()
        {
            if (CheckFocus())
            {
                if (!IsFocused)
                {
                    IsFocused = true;
                    Instance.OnGotFocus();
                }
            }
            else
            {
                if (IsFocused)
                {
                    IsFocused = false;
                    Instance.OnLostFocus();
                }
            }
        }

        /// <summary>
        /// Static class for accessing engine files.
        /// </summary>
        public static class Files
        {
            public static string WorldPath(string fileName)
                => Path.Combine(Settings.WorldsFolder ?? string.Empty, fileName);
            public static async Task<World> LoadEngineWorldAsync(string fileName)
                => await TFileObject.LoadAsync<World>(WorldPath(fileName));

            //public enum EEngineShaderName
            //{
            //    Bezier_gs,
            //    Heightmap_gs,
            //    CubeMapSphereMap_fs,
            //    Outline2DUnlitForward_fs,
            //    StencilExplode_gs,
            //    VisualizeNBTForward_fs,
            //    VisualizeNBTForward_gs,
            //    VisualizeNormal_gs,

            //    HudFBO_fs,
            //    ParticleInstance_vs,
            //    ParticleInstance_fs,
            //    PointLightShadowDepth_fs,
            //    PointLightShadowDepth_gs,

            //    EditorPreviewIcon_fs, //Move to editor files
            //    MaterialEditorGraphBG_fs, //Move to editor files
            
            //    Common_ColoredDeferred_fs,
            //    Common_TexturedDeferred_fs,
            //    Common_TexturedNormalMapDeferred_fs,
            //    Common_UnlitAlphaTexturedForward_fs,
            //    Common_UnlitColoredForward_fs,
            //    Common_UnlitTexturedForward_fs,
            //}

            public static string ShaderPath(string fileName)
                => Path.Combine(Settings.ShadersFolder ?? string.Empty, fileName);
            public static GLSLScript Shader(string fileName, EGLSLType mode)
                => new GLSLScript(ShaderPath(fileName), mode);

            public static string FontPath(string fileName)
                => Path.Combine(Settings.FontsFolder ?? string.Empty, fileName);

            public static string ScriptPath(string fileName)
                => Path.Combine(Settings.ScriptsFolder ?? string.Empty, fileName);
            public static TextFile Script(string fileName)
                => Script<TextFile>(fileName);
            public static T Script<T>(string fileName) where T : TextFile, new()
            {
                T value = new T { FilePath = ScriptPath(fileName) };
                return value;
            }
            
            public static async Task<TextureFile2D> LoadEngineTexture2DAsync(string fileName)
                => await TFileObject.LoadAsync<TextureFile2D>(TexturePath(fileName));
            public static string TexturePath(string fileName)
                => Path.Combine(Settings.TexturesFolder ?? string.Empty, fileName);

            /// <summary>
            /// Makes path relative to the application exe and returns only the relative part of the path.
            /// </summary>
            //public static string ModifyPath(string path)
            //{
            //    if (string.IsNullOrEmpty(path))
            //        return path;
            //    string startupPath = Application.StartupPath;
            //    return Path.GetFullPath(path).MakePathRelativeTo(startupPath).Substring(startupPath.Length);
            //}

            //internal static bool AddLocalFileInstance<T>(string path, T file) where T : class, IFileObject
            //{
            //    if (string.IsNullOrEmpty(path) || file is null)
            //        return false;

            //    LocalFileInstances.AddOrUpdate(path, new List<IFileObject>() { file }, (key, oldValue) =>
            //    {
            //        oldValue.Add(file);
            //        return oldValue;
            //    });

            //    return true;
            //}
        }
        /// <summary>
        /// Interface for accessing inputs.
        /// </summary>
        public static class Input
        {
            public static InputInterface Get(int localPlayerIndex)
                => World.CurrentGameMode.LocalPlayers.IndexInRange(localPlayerIndex) ? World.CurrentGameMode.LocalPlayers[localPlayerIndex]?.Input : null;

            public static bool Key(int localPlayerIndex, EKey key, EButtonInputType type)
                => Get(localPlayerIndex)?.GetKeyState(key, type) ?? false;
            public static bool Button(int localPlayerIndex, EGamePadButton button, EButtonInputType type)
                => Get(localPlayerIndex)?.GetButtonState(button, type) ?? false;
            public static bool MouseButton(int localPlayerIndex, EMouseButton button, EButtonInputType type)
                => Get(localPlayerIndex)?.GetMouseButtonState(button, type) ?? false;
            public static bool AxisButton(int localPlayerIndex, EGamePadAxis axis, EButtonInputType type)
                => Get(localPlayerIndex)?.GetAxisState(axis, type) ?? false;

            public static float Axis(int localPlayerIndex, EGamePadAxis axis)
                => Get(localPlayerIndex)?.GetAxisValue(axis) ?? 0.0f;
            
            public static bool KeyReleased(int localPlayerIndex, EKey key)
                => Key(localPlayerIndex, key, EButtonInputType.Released);
            public static bool KeyPressed(int localPlayerIndex, EKey key)
                => Key(localPlayerIndex, key, EButtonInputType.Pressed);
            public static bool KeyHeld(int localPlayerIndex, EKey key)
                => Key(localPlayerIndex, key, EButtonInputType.Held);
            public static bool KeyDoublePressed(int localPlayerIndex, EKey key)
                => Key(localPlayerIndex, key, EButtonInputType.DoublePressed);

            public static bool ButtonReleased(int localPlayerIndex, EGamePadButton button)
                => Button(localPlayerIndex, button, EButtonInputType.Released);
            public static bool ButtonPressed(int localPlayerIndex, EGamePadButton button)
                => Button(localPlayerIndex, button, EButtonInputType.Pressed);
            public static bool ButtonHeld(int localPlayerIndex, EGamePadButton button)
                => Button(localPlayerIndex, button, EButtonInputType.Held);
            public static bool ButtonDoublePressed(int localPlayerIndex, EGamePadButton button)
                => Button(localPlayerIndex, button, EButtonInputType.DoublePressed);
            
            public static bool MouseButtonReleased(int localPlayerIndex, EMouseButton button)
                => MouseButton(localPlayerIndex, button, EButtonInputType.Released);
            public static bool MouseButtonPressed(int localPlayerIndex, EMouseButton button)
                => MouseButton(localPlayerIndex, button, EButtonInputType.Pressed);
            public static bool MouseButtonHeld(int localPlayerIndex, EMouseButton button)
                => MouseButton(localPlayerIndex, button, EButtonInputType.Held);
            public static bool MouseButtonDoublePressed(int localPlayerIndex, EMouseButton button)
                => MouseButton(localPlayerIndex, button, EButtonInputType.DoublePressed);
        }

        public partial class InternalEnginePersistentSingleton : MarshalByRefObject
        {
            public void OnGotFocus() => GotFocus?.Invoke();
            public void OnLostFocus() => LostFocus?.Invoke();

            public InternalEnginePersistentSingleton()
            {
                Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Constructing new engine singleton.");
            }

            public async Task<EngineSettings> GetSettingsAsync()
            {
                EngineSettings settings;
                if (Game?.EngineSettingsOverrideRef != null)
                {
                    settings = await Game.EngineSettingsOverrideRef.GetInstanceAsync();
                    if (settings != null)
                        return settings;
                }
                if (DefaultEngineSettingsOverrideRef != null)
                {
                    settings = await DefaultEngineSettingsOverrideRef.GetInstanceAsync();
                    if (settings != null)
                        return settings;
                }
                return _defaultEngineSettings.Value;
            }
            #region Fonts
            /// <summary>
            /// Creates a new font object given the font's name and parameters.
            /// </summary>
            /// <param name="fontName"></param>
            /// <param name="size"></param>
            /// <param name="style"></param>
            /// <returns></returns>
            public Font MakeFont(string fontName, float size, FontStyle style)
            {
                FontFamily family = GetCustomFontFamily(fontName);
                if (family is null)
                    return new Font("Segoe UI", size, style);
                return new Font(family, size, style);
            }
            /// <summary>
            /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
            /// </summary>
            public void LoadCustomFont(string path)
                => LoadCustomFont(path, Path.GetFileNameWithoutExtension(path));
            /// <summary>
            /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
            /// </summary>
            public void LoadCustomFont(string path, string fontFamilyName)
            {
                if (!File.Exists(path))
                    return;
                string ext = Path.GetExtension(path).ToLowerInvariant().Substring(1);
                if (!(ext.Equals("ttf") || ext.Equals("otf")))
                    return;
                _fontIndexMatching.Add(fontFamilyName.ToLowerInvariant(), _fontCollection.Families.Length);
                _fontCollection.AddFontFile(path);
            }
            /// <summary>
            /// Gets a custom font family using its name.
            /// </summary>
            /// <param name="fontFamilyName">The name of the font family.</param>
            public FontFamily GetCustomFontFamily(string fontFamilyName)
                => _fontIndexMatching.ContainsKey(fontFamilyName.ToLowerInvariant()) ? GetCustomFontFamily(_fontIndexMatching[fontFamilyName]) : null;
            /// <summary>
            /// Gets a custom font family using its load index.
            /// </summary>
            /// <param name="fontFamilyIndex">The index of the font, in the order it was loaded in.</param>
            public FontFamily GetCustomFontFamily(int fontFamilyIndex)
                => _fontCollection.Families.IndexInRange(fontFamilyIndex) ? _fontCollection.Families[fontFamilyIndex] : null;
            private async Task LoadCustomFonts()
            {
                //if (DesignMode)
                //    return;
                FontsLoaded = true;
                EngineSettings s = await GetSettingsAsync();
                string folder = Path.GetFullPath(s.FontsFolder);
                string[] ttf = Directory.GetFiles(folder, "*.ttf");
                string[] otf = Directory.GetFiles(folder, "*.otf");
                foreach (string path in ttf) LoadCustomFont(path);
                foreach (string path in otf) LoadCustomFont(path);
            }

            public void OnPauseChanged(bool isPaused, ELocalPlayerIndex toggler)
                => PauseChanged?.Invoke(isPaused, toggler);
            public void OnDebugOutput(string message)
            {
                if (AppDomainHelper.IsGameDomain && DebugOutput is null)
                    Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] {message} (This message was not recieved by the GUI)");

                DebugOutput?.Invoke(message);
            }

            #endregion
        }
    }
}
