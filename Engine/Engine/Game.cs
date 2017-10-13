using TheraEngine.Files;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using TheraEngine.Rendering.Text;
using TheraEngine.GameModes;

namespace TheraEngine
{
    /// <summary>
    /// Contains all information needed to run any game using the engine.
    /// </summary>
    [FileClass("GINF", "Game Info")]
    public class Game : FileObject
    {
        public Game() { }
        
        protected LocalizedStringTable _localizedStringTable;
        protected VariableStringTable _variableStringTable;
        
        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> TransitionWorld { get; set; } = new SingleFileRef<World>("TransitionWorld.xworld");
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> OpeningWorld { get; set; } = new SingleFileRef<World>("OpeningWorld.xworld");
        [Category("Engine")]
        [Serialize]
        [Browsable(false)]
        public SingleFileRef<EngineSettings> EngineSettings { get; set; } = new EngineSettings();
        [Category("Engine")]
        [Serialize]
        [Browsable(false)]
        public SingleFileRef<UserSettings> UserSettings { get; set; } = new UserSettings();
        [Category("About")]
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description { get; set; }
        [Category("About")]
        [Serialize]
        public string Copyright { get; set; }
        [Category("About")]
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Credits { get; set; }
        [Category("About")]
        [Serialize]
        public string IconPath { get; set; }
        [Category("Viewports")]
        [Serialize]
        public Viewport.TwoPlayerPreference TwoPlayerPref { get; set; } = Viewport.TwoPlayerPreference.SplitHorizontally;
        [Category("Viewports")]
        [Serialize]
        public Viewport.ThreePlayerPreference ThreePlayerPref { get; set; } = Viewport.ThreePlayerPreference.PreferFirstPlayer;
        [Serialize]
        [Category("Text")]
        public LocalizedStringTable LocalizedStringTable
        {
            get => _localizedStringTable;
            set => _localizedStringTable = value;
        }
        [Serialize]
        [Category("Text")]
        public VariableStringTable VariableStringTable
        {
            get => _variableStringTable;
            set => _variableStringTable = value;
        }

        public virtual GameState State { get; set; } = new GameState();
    }

    /// <summary>
    /// Contains all information pertaining to the game's current state in the engine.
    /// </summary>
    [FileClass("GSTA", "Game State")]
    public class GameState : FileObject
    {
        private BaseGameMode _currentGameMode;
        public BaseGameMode ActiveGameMode
        {
            get => _currentGameMode;
            set
            {
                //_currentGameMode?.EndGameplay();
                _currentGameMode = value;
                //_currentGameMode?.BeginGameplay();
            }
        }
    }
}