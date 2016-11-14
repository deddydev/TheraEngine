using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using CustomEngine.Rendering;
using CustomEngine.Worlds;
using CustomEngine.Input;
using CustomEngine.Audio;
using CustomEngine.Input.Devices;
using CustomEngine.Input.Devices.OpenTK;
using CustomEngine.Input.Devices.DirectX;
using CustomEngine.Rendering.Models.Materials;
using System.Threading.Tasks;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors;

namespace CustomEngine
{
    public static partial class Engine
    {
        public static string StartupPath = Application.StartupPath;
        public static string ContentFolderRel = "\\Content";
        public static string ConfigFolderRel = "\\Config";
        public static string EngineSettingsPathRel = ConfigFolderRel + "\\EngineSettings.xml";
        public static string UserSettingsPathRel = ConfigFolderRel + "\\UserSettings.xml";

        private static World _transitionWorld = null;
        private static World _currentWorld = null;
        public static SingleFileRef<EngineSettings> _engineSettings = new SingleFileRef<EngineSettings>(EngineSettingsPathRel);
        public static SingleFileRef<UserSettings> _userSettings = new SingleFileRef<UserSettings>(UserSettingsPathRel);

        public static Dictionary<string, List<FileObject>> LoadedFiles = new Dictionary<string, List<FileObject>>();

        public static int PhysicsSubsteps = 10;
        private static ComputerInfo _computerInfo;
        public static List<World> LoadedWorlds = new List<World>();
        public static MonitoredList<LocalPlayerController> ActivePlayers = new MonitoredList<LocalPlayerController>();
        public static List<AIController> ActiveAI = new List<AIController>();
        private static GlobalTimer _timer = new GlobalTimer();
        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;
        private static RenderLibrary _renderLibrary;
        private static AudioLibrary _audioLibrary;
        private static InputLibrary _inputLibrary;
        private static List<float> _debugTimers = new List<float>();
        private static InputAwaiter _inputAwaiter;
        public static Dictionary<PlayerIndex, Queue<Pawn>> _possessionQueue = new Dictionary<PlayerIndex, Queue<Pawn>>();

        public static Viewport.TwoPlayerViewportPreference TwoPlayerPref = 
            Viewport.TwoPlayerViewportPreference.SplitHorizontally;
        public static Viewport.ThreePlayerViewportPreference ThreePlayerPref =
            Viewport.ThreePlayerViewportPreference.PreferFirstPlayer;

        public static Dictionary<ETickGroup, Dictionary<ETickOrder, List<ObjectBase>>> _tick = 
            new Dictionary<ETickGroup, Dictionary<ETickOrder, List<ObjectBase>>>();

        internal static AbstractRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }
        public static AbstractAudioManager AudioManager
        {
            get { return _audioManager; }
            internal set { _audioManager = value; }
        }

        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static double TargetRenderFreq
        {
            get { return _timer.TargetRenderFrequency; }
            set { _timer.TargetRenderFrequency = value; }
        }

        /// <summary>
        /// Frames per second that the game will try to update at.
        /// </summary>
        public static double TargetUpdateFreq
        {
            get { return _timer.TargetUpdateFrequency; }
            set { _timer.TargetUpdateFrequency = value; }
        }


        /// <summary>
        /// How fast/slow the game time looks
        /// </summary>
        public static double TimeDilation
        {
            get { return _timer.TimeDilation; }
            set { _timer.TimeDilation = value; }
        }

        [Default]
        public static World TransitionWorld
        {
            get { return _transitionWorld; }
            set { _transitionWorld = value; }
        }
        [State]
        public static World World
        {
            get { return _currentWorld; }
            set { SetCurrentWorld(value, true); }
        }

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine settings accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo { get { return _computerInfo; } }
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
            get { return _renderLibrary; }
            set
            {
                _renderLibrary = value;
                List<RenderContext> contexts = new List<RenderContext>(RenderContext.BoundContexts);
                foreach (RenderContext c in contexts)
                    c.Control?.SetRenderLibrary();
            }
        }
        public static AudioLibrary AudioLibrary
        {
            get { return _audioLibrary; }
            set { _audioLibrary = value; }
        }
        public static InputLibrary InputLibrary
        {
            get { return _inputLibrary; }
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
                //foreach (LocalPlayerController c in ActivePlayers)
                //    c.SetInputLibrary();
            }
        }
    }
}