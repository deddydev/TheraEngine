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
    [FileExt("game")]
    [FileDef("Game Info", "Contains all information needed to run any game using the engine.")]
    public class Game : FileObject
    {
        public Game() { }
        
        protected LocalizedStringTable _localizedStringTable;
        protected VariableStringTable _variableStringTable;
        
        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        [TSerialize]
        public GlobalFileRef<World> TransitionWorld { get; set; } = new GlobalFileRef<World>("TransitionWorld");
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        [TSerialize]
        public GlobalFileRef<World> OpeningWorld { get; set; } = new GlobalFileRef<World>("OpeningWorld");
        [Category("Engine")]
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<EngineSettings> EngineSettings { get; set; } = new EngineSettings();
        [Category("Engine")]
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<UserSettings> UserSettings { get; set; } = new UserSettings();
        [Category("About")]
        [TSerialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description { get; set; }
        [Category("About")]
        [TSerialize]
        public string Copyright { get; set; }
        [Category("About")]
        [TSerialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Credits { get; set; }
        [Category("About")]
        [TSerialize]
        public string IconPath { get; set; }
        [Category("Viewports")]
        [TSerialize]
        public Viewport.TwoPlayerPreference TwoPlayerPref { get; set; } = Viewport.TwoPlayerPreference.SplitHorizontally;
        [Category("Viewports")]
        [TSerialize]
        public Viewport.ThreePlayerPreference ThreePlayerPref { get; set; } = Viewport.ThreePlayerPreference.PreferFirstPlayer;
        [TSerialize]
        [Category("Text")]
        public LocalizedStringTable LocalizedStringTable
        {
            get => _localizedStringTable;
            set => _localizedStringTable = value;
        }
        [TSerialize]
        [Category("Text")]
        public VariableStringTable VariableStringTable
        {
            get => _variableStringTable;
            set => _variableStringTable = value;
        }

        public virtual GameState State { get; set; } = new GameState();

        public GlobalFileRef<BaseGameMode> DefaultGameMode { get; set; } = null;
    }

    /// <summary>
    /// Contains all information pertaining to the game's current state in the engine.
    /// </summary>
    [FileExt("state")]
    [FileDef("Game State")]
    public class GameState : FileObject
    {
        public GlobalFileRef<World> World { get; set; }
        public GlobalFileRef<BaseGameMode> GameMode { get; set; }
    }
}