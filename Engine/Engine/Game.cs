using TheraEngine;
using TheraEngine.Files;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine
{
    /// <summary>
    /// Contains all information needed to run any game using the engine.
    /// </summary>
    [FileClass("TGAME", "Game Info")]
    public class Game : FileObject
    {
        public Game()
        {
            UserSettings = new UserSettings();
            EngineSettings = new EngineSettings();
            OpeningWorld = new SingleFileRef<World>("OpeningWorld.xworld");
            TransitionWorld = new SingleFileRef<World>("TransitionWorld.xworld");
        }

        private SingleFileRef<EngineSettings> _engineSettings;
        private SingleFileRef<UserSettings> _userSettings;
        private SingleFileRef<World> _transitionWorld;
        private SingleFileRef<World> _openingWorld;
        private string _description;
        private string _copyright;
        private string _credits;
        private Viewport.TwoPlayerPreference _2PPref = Viewport.TwoPlayerPreference.SplitHorizontally;
        private Viewport.ThreePlayerPreference _3PPref = Viewport.ThreePlayerPreference.PreferFirstPlayer;
        private string _iconPath;
        
        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> TransitionWorld
        {
            get => _transitionWorld;
            set => _transitionWorld = value;
        }
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> OpeningWorld
        {
            get => _openingWorld;
            set => _openingWorld = value;
        }
        [Category("Engine")]
        [Serialize]
        [Browsable(false)]
        public EngineSettings EngineSettings
        {
            get => _engineSettings;
            set => _engineSettings = value;
        }
        [Category("Engine")]
        [Serialize]
        [Browsable(false)]
        public UserSettings UserSettings
        {
            get => _userSettings;
            set => _userSettings = value;
        }
        [Category("About")]
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description
        {
            get => _description;
            set => _description = value;
        }
        [Category("About")]
        [Serialize]
        public string Copyright
        {
            get => _copyright;
            set => _copyright = value;
        }
        [Category("About")]
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Credits
        {
            get => _credits;
            set => _credits = value;
        }
        [Category("Viewports")]
        [Serialize]
        public Viewport.TwoPlayerPreference TwoPlayerPref
        {
            get => _2PPref;
            set => _2PPref = value;
        }
        [Category("Viewports")]
        [Serialize]
        public Viewport.ThreePlayerPreference ThreePlayerPref
        {
            get => _3PPref;
            set => _3PPref = value;
        }
        public string IconPath { get => _iconPath; set => _iconPath = value; }
    }
}