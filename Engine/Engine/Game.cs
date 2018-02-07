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
    public class Game : TFileObject
    {
        public Game() { }
        
        protected LocalizedStringTable _localizedStringTable;
        protected VariableStringTable _variableStringTable;
        
        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        [TSerialize]
        public GlobalFileRef<World> TransitionWorldRef { get; set; } = new GlobalFileRef<World>("TransitionWorld");
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        [TSerialize]
        public GlobalFileRef<World> OpeningWorldRef { get; set; } = new GlobalFileRef<World>("OpeningWorld");
        [Category("Engine")]
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<EngineSettings> EngineSettingsRef { get; set; } = new EngineSettings();
        [Category("Engine")]
        [TSerialize]
        [Browsable(false)]
        public GlobalFileRef<UserSettings> UserSettingsRef { get; set; } = new UserSettings();
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
    public class GameState : TFileObject
    {
        private GlobalFileRef<World> _worldRef = new GlobalFileRef<World>();
        private GlobalFileRef<BaseGameMode> _gameModeRef = new GlobalFileRef<BaseGameMode>();

        public World World
        {
            get => _worldRef.File;
            set => _worldRef.File = value;
        }
        public GlobalFileRef<World> WorldRef
        {
            get => _worldRef;
            set => _worldRef = value ?? new GlobalFileRef<World>();
        }
        public BaseGameMode GameMode
        {
            get => _gameModeRef.File;
            set => _gameModeRef.File = value;
        }
        public GlobalFileRef<BaseGameMode> GameModeRef
        {
            get => _gameModeRef;
            set => _gameModeRef = value ?? new GlobalFileRef<BaseGameMode>();
        }
    }
}