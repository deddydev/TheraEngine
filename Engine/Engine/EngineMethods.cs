using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEngine.Files;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using TheraEngine.Actors;
using System.Threading.Tasks;

namespace TheraEngine
{
    public static partial class Engine
    {
        public static ColorF4 InvalidColor { get; } = Color.Magenta;

        #region Startup/Shutdown

        static Engine()
        {
            //Steamworks.SteamAPI.Init();

            _timer = new EngineTimer();
            _timer.UpdateFrame += Tick;

            LocalPlayers.PostAdded += ActivePlayers_Added;
            LocalPlayers.PostRemoved += ActivePlayers_Removed;

            _tickLists = new ThreadSafeList<DelTick>[45];
            for (int i = 0; i < _tickLists.Length; ++i)
                _tickLists[i] = new ThreadSafeList<DelTick>();
        }

        /// <summary>
        /// Call this in the program's main method run a game in the engine.
        /// Will create a render form,  initialize the engine, and start the game, so no other methods are needed.
        /// </summary>
        /// <param name="game">The game to play.</param>
        public static void Run(Game game)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RenderForm(game));
        }

        public static Shader LoadEngineShader(string fileName, ShaderMode mode)
        {
            return new Shader(mode, new TextFile(EngineShaderPath(fileName)));
        }
        public static string EngineShaderPath(string fileName)
        {
            return Path.Combine(Settings.ShadersFolder, fileName);
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
        public static void SetGame(Game game)
        {
            MainThreadID = Thread.CurrentThread.ManagedThreadId;
            if (_game != null)
            {

            }
            _game = game;
            if (_game != null)
            {

            }
        }

        private static IEnumerable<Type> _typeCache = null;
        public static IEnumerable<Type> FindTypes(Predicate<Type> matchPredicate, bool resetTypeCache = false)
        {
            if (_typeCache == null || resetTypeCache)
                _typeCache = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).SelectMany(x => x.GetExportedTypes());
            return _typeCache.Where(x => matchPredicate(x)).OrderBy(x => x.Name);
        }

        public static void SetGamePanel(BaseRenderPanel panel, bool registerTickNow = true)
        {
            BaseRenderPanel.WorldPanel = panel;
            if (registerTickNow)
                panel?.RegisterTick();
        }
        /// <summary>
        /// Initializes the engine to its beginning state.
        /// Call AFTER SetGame is called and all initial render panels are created and ready.
        /// </summary>
        public static async void Initialize(bool loadOpeningWorldGameMode = true)
        {
            RenderLibrary = _game.UserSettingsRef.File.RenderLibrary;
            AudioLibrary = _game.UserSettingsRef.File.AudioLibrary;
            InputLibrary = _game.UserSettingsRef.File.InputLibrary;
            PhysicsLibrary = _game.UserSettingsRef.File.PhysicsLibrary;

            //Set initial world (this would generally be a world for opening videos or the main menu)
            SetCurrentWorld(Game.OpeningWorldRef, true, loadOpeningWorldGameMode);

            TargetRenderFreq = Settings.CapFPS ? Settings.TargetFPS.ClampMin(1.0f) : 0.0f;
            TargetUpdateFreq = Settings.CapUPS ? Settings.TargetUPS.ClampMin(1.0f) : 0.0f;

            //Preload transition world now
            await Game.TransitionWorldRef.LoadNewInstanceAsync();
        }

        public static bool ShuttingDown { get; private set; }

        /// <summary>
        /// Call this to stop the engine and dispose of all allocated data.
        /// </summary>
        public static void ShutDown()
        {
            ShuttingDown = true;

            //Steamworks.SteamAPI.Shutdown();
            Stop();
            SetCurrentWorld(null, true, true);
            IEnumerable<FileObject> files = LocalFileInstances.SelectMany(x => x.Value);
            foreach (FileObject o in files)
                o?.Unload();
            files = GlobalFileInstances.Values;
            foreach (FileObject o in files)
                o?.Unload();
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
        public static void Run() => _timer.Run();
        /// <summary>
        /// HALTS update and render ticks. Not recommended for use as this literally halts all visuals and user input.
        /// </summary>
        public static void Stop() => _timer.Stop();
        /// <summary>
        /// Registers the given function to be called every update tick.
        /// </summary>
        public static void RegisterRenderTick(EventHandler<FrameEventArgs> func)
            => _timer.RenderFrame += func;
        /// <summary>
        /// Registers the given function to be called every render tick.
        /// </summary>
        public static void UnregisterRenderTick(EventHandler<FrameEventArgs> func)
            => _timer.RenderFrame -= func;
        /// <summary>
        /// Registers a method to execute in a specific order every update tick.
        /// </summary>
        /// <param name="group">The first grouping of when to tick: before, after, or during the physics tick update.</param>
        /// <param name="order">The order to execute the function within its group.</param>
        /// <param name="function">The function to execute per update tick.</param>
        /// <param name="pausedBehavior">If the function should even execute at all, depending on the pause state.</param>
        internal static void RegisterTick(ETickGroup group, ETickOrder order, DelTick function, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            if (function != null)
            {
                var list = GetTickList(group, order, pausedBehavior);
                int tickIndex = (int)group + (int)order + (int)pausedBehavior;
                if (_currentTickList == tickIndex)
                    _tickListQueue.Enqueue(new Tuple<bool, DelTick>(true, function));
                else
                    list.Add(function);
            }
        }
        /// <summary>
        /// Stops running a tick method that was previously registered with the same parameters.
        /// </summary>
        internal static void UnregisterTick(ETickGroup group, ETickOrder order, DelTick function, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            if (function != null)
            {
                var list = GetTickList(group, order, pausedBehavior);
                int tickIndex = (int)group + (int)order + (int)pausedBehavior;
                if (_currentTickList == tickIndex)
                    _tickListQueue.Enqueue(new Tuple<bool, DelTick>(false, function));
                else
                    list.Remove(function);
            }
        }
        /// <summary>
        /// Gets a list of items to tick (in no particular order) that were registered with the following parameters.
        /// </summary>
        private static ThreadSafeList<DelTick> GetTickList(ETickGroup group, ETickOrder order, EInputPauseType pausedBehavior)
            => _tickLists[(int)group + (int)order + (int)pausedBehavior];
        /// <summary>
        /// Ticks the before, during, and after physics groups. Also steps the physics simulation during the during physics tick group.
        /// Does not tick physics if paused.
        /// </summary>
        private static void Tick(object sender, FrameEventArgs e)
        {
            float delta = (float)(e.Time * TimeDilation);
            TickGroup(ETickGroup.PrePhysics, delta);
            //using (Task task = new Task(() => TickGroup(ETickGroup.DuringPhysics, delta)))
            //{
                //task.Start();
                if (!_isPaused && World != null)
                    World.StepSimulation(delta);
                //task.Wait();
            //}
            TickGroup(ETickGroup.PostPhysics, delta);
        }
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
        private static void TickList(int index, float delta)
        {
            ThreadSafeList<DelTick> currentList = _tickLists[_currentTickList = index];

            //Parallel.ForEach(currentList, currentFunc => currentFunc(delta));
            currentList.ForEach(x => x(delta));

            _currentTickList = -1;

            //Add or remove the list of methods that tried to register to or unregister from this group while it was ticking.
            while (!_tickListQueue.IsEmpty && _tickListQueue.TryDequeue(out Tuple<bool, DelTick> result))
            {
                if (result.Item1)
                    currentList.Add(result.Item2);
                else
                    currentList.Remove(result.Item2);
            }
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
        public static void TogglePause(LocalPlayerIndex toggler)
        {
            SetPaused(!_isPaused, toggler);
        }
        /// <summary>
        /// Sets the pause state regardless of what it is currently.
        /// </summary>
        /// <param name="wantsPause">The desired pause state.</param>
        /// <param name="toggler">The player that's pausing the game.</param>
        public static void SetPaused(bool wantsPause, LocalPlayerIndex toggler, bool force = false)
        {
            if (!force && wantsPause && ActiveGameMode.DisallowPausing)
                return;
            _isPaused = wantsPause;
            Paused?.Invoke(_isPaused, toggler);
            string.Format("Engine{0}paused.", _isPaused ? " " : " un").PrintLine();
        }
        #endregion

        #region Fonts
        /// <summary>
        /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
        /// </summary>
        public static void LoadCustomFont(string path) 
            => LoadCustomFont(path, Path.GetFileNameWithoutExtension(path));
        /// <summary>
        /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
        /// </summary>
        public static void LoadCustomFont(string path, string fontFamilyName)
        {
            if (!File.Exists(path))
                return;
            string ext = Path.GetExtension(path).ToLowerInvariant().Substring(1);
            if (!(ext.Equals("ttf") || ext.Equals("otf")))
                return;
            _fontIndexMatching[fontFamilyName] = _fontCollection.Families.Length;
            _fontCollection.AddFontFile(path);
        }
        /// <summary>
        /// Gets a custom font family using its name.
        /// </summary>
        /// <param name="fontFamilyName">The name of the font family.</param>
        public static FontFamily GetCustomFontFamily(string fontFamilyName)
            => _fontIndexMatching.ContainsKey(fontFamilyName) ? GetCustomFontFamily(_fontIndexMatching[fontFamilyName]) : null;
        /// <summary>
        /// Gets a custom font family using its load index.
        /// </summary>
        /// <param name="fontFamilyIndex">The index of the font, in the order it was loaded in.</param>
        public static FontFamily GetCustomFontFamily(int fontFamilyIndex)
            => _fontCollection.Families.IndexInRange(fontFamilyIndex) ? _fontCollection.Families[fontFamilyIndex] : null;
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
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void Print(string message, params object[] args)
        {
#if DEBUG || EDITOR
            if (args.Length != 0)
                message = string.Format(message, args);
            Debug.Write(message);
            OutputString += message;
            DebugOutput?.Invoke(message);
#endif
        }
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void PrintLine(string message, params object[] args)
            => Print(message + Environment.NewLine, args);
        
        public static void LogWarning(string message, params object[] args)
        {
#if DEBUG || EDITOR
            //Format and print message
            message = message ?? "<No Message>";
            if (args != null && args.Length > 0)
                message = string.Format(message, args);

            //Format and print stack trace
            string stackTrace = Environment.StackTrace;
            string atStr = "   at ";

            //Most 3 recent calls don't matter
            int at4th = stackTrace.FindOccurrence(0, 3, atStr);
            if (at4th > 0)
                stackTrace = stackTrace.Substring(at4th);

            //Everything before wndProc is almost always irrelevant
            int wndProc = stackTrace.IndexOf("WndProc(Message& m)");
            if (wndProc > 0)
            {
                int at = stackTrace.FindFirstReverse(wndProc, atStr);
                if (at > 0)
                    stackTrace = stackTrace.Substring(0, at);
            }
            
            message += Environment.NewLine + stackTrace;
            PrintLine("[{1}] {0}", message, DateTime.Now);
#endif
        }

        #endregion

        #region Game Modes
        /// <summary>
        /// Retrieves the current world's overridden game mode or the game's game mode if not overriden.
        /// </summary>
        public static BaseGameMode GetGameMode()
            => World?.Settings?.GameModeOverrideRef?.File ?? Game.DefaultGameMode;
        public static void SetActiveGameMode(BaseGameMode mode, bool beginGameplay = true)
        {
            if (Game != null)
            {
                ActiveGameMode?.EndGameplay();
                Game.State.GameMode = mode;
                if (beginGameplay)
                    ActiveGameMode?.BeginGameplay();
            }
        }
        #endregion

        public static bool RayTrace(RayTrace result) => World?.PhysicsWorld.RayTrace(result) ?? false;
        public static bool ShapeTrace(ShapeTrace result) => World?.PhysicsWorld.ShapeTrace(result) ?? false;
        
        private static void ActivePlayers_Removed(LocalPlayerController item)
        {
            //ActiveGameMode?.HandleLocalPlayerLeft(item);

            //TODO: remove controller from the server
        }

        private static void ActivePlayers_Added(LocalPlayerController item)
        {
            //ActiveGameMode?.HandleLocalPlayerJoined(item);

            //TODO: create controller on the server
        }

        internal static void ResetLocalPlayerControllers()
        {
            foreach (LocalPlayerController controller in LocalPlayers)
                controller.UnlinkControlledPawn();
        }
        internal static void DestroyLocalPlayerControllers()
        {
            foreach (LocalPlayerController controller in LocalPlayers)
                controller.Destroy();
            LocalPlayers.Clear();
        }

        //public static BaseGameMode ActiveGameMode { get; set; }
        //public static T ActiveGameModeAs<T>() where T : BaseGameMode
        //    => ActiveGameMode as T;

        /// <summary>
        /// Enqueues a pawn to be possessed by the given local player as soon as its current controlled pawn is set to null.
        /// </summary>
        /// <param name="pawn">The pawn to possess.</param>
        /// <param name="possessor">The controller to possess the pawn.</param>
        public static void QueuePossession(IPawn pawn, LocalPlayerIndex possessor)
        {
            int index = (int)possessor;
            if (index < LocalPlayers.Count)
                LocalPlayers[index].EnqueuePosession(pawn);
            else if (_possessionQueues.ContainsKey(possessor))
                _possessionQueues[possessor].Enqueue(pawn);
            else
            {
                Queue<IPawn> queue = new Queue<IPawn>();
                queue.Enqueue(pawn);
                _possessionQueues.Add(possessor, queue);
            }
        }

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

        internal static bool AddLocalFileInstance<T>(string path, T file) where T : FileObject
        {
            if (string.IsNullOrEmpty(path) || file == null)
                return false;

            LocalFileInstances.AddOrUpdate(path, new List<FileObject>() { file }, (key, oldValue) =>
            {
                oldValue.Add(file);
                return oldValue;
            });

            file.OnLoaded();
            return true;
        }
        internal static bool AddGlobalFileInstance<T>(string path, T file) where T : FileObject
        {
            if (string.IsNullOrEmpty(path) || file == null)
                return false;

            GlobalFileInstances.AddOrUpdate(path, file, (key, oldValue) => file);

            file.OnLoaded();
            return true;
        }

        /// <summary>
        /// Retrieves the world viewport with the same index.
        /// </summary>
        public static Viewport GetViewport(LocalPlayerIndex index)
            => BaseRenderPanel.WorldPanel?.GetViewport(index);

        /// <summary>
        /// Tells the engine to play in a new world.
        /// </summary>
        /// <param name="world">The world to play in.</param>
        /// <param name="unloadPrevious">Whether or not the engine should deallocate all resources utilized by the current world before loading the new one.</param>
        public static void SetCurrentWorld(World world, bool unloadPrevious = true, bool loadWorldGameMode = true)
        {
            if (_currentWorld == world)
                return;

            PreWorldChanged?.Invoke();

            //bool wasRunning = _timer.IsRunning;
            World previous = World;

            ActiveGameMode?.EndGameplay();
            World?.EndPlay();

            //Stop();

            _currentWorld = world;
            if (World != null)
            {
                //if (!deferBeginPlay)
                    World.BeginPlay();
            }
            //else
            //    Scene.Clear(new BoundingBox(0.5f, Vec3.Zero));

            if (loadWorldGameMode && Game != null)
                Game.State.GameModeRef = World?.GetGameMode();

            ActiveGameMode?.BeginGameplay();

            PostWorldChanged?.Invoke();

            //if (wasRunning)
            //    Run();

            if (unloadPrevious)
                previous?.Unload();
        }

        /// <summary>
        /// Called when the input awaiter discovers a new input device.
        /// </summary>
        /// <param name="device">The device that was found.</param>
        internal static void FoundInput(InputDevice device)
        {
            if (device is BaseKeyboard || device is BaseMouse)
            {
                if (LocalPlayers.Count == 0)
                {
                    LocalPlayerIndex index = LocalPlayerIndex.One;
                    if (_possessionQueues.ContainsKey(index))
                    {
                        //Transfer possession queue to the controller itself
                        ActiveGameMode?.CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(index);
                    }
                    else
                        ActiveGameMode?.CreateLocalController(index);
                }
                else
                    LocalPlayers[0].Input.UpdateDevices();
            }
            else
            {
                if (device.Index >= LocalPlayers.Count)
                {
                    LocalPlayerIndex index = (LocalPlayerIndex)LocalPlayers.Count;
                    if (_possessionQueues.ContainsKey(index))
                    {
                        //Transfer possession queue to the controller itself
                        ActiveGameMode?.CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(index);
                    }
                    else
                        ActiveGameMode?.CreateLocalController(index);
                }
                else
                    LocalPlayers[device.Index].Input.UpdateDevices();
            }
        }
    }
}
