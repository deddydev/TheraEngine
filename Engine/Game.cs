using TheraEngine;
using TheraEngine.Files;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using TheraEngine.Rendering;

namespace TheraEditor
{
    /// <summary>
    /// Contains all information needed to run any game using the engine.
    /// </summary>
    [FileClass("TGAME", "Game Info")]
    public class Game : FileObject
    {
        private SingleFileRef<EngineSettings> _engineSettings;
        private SingleFileRef<UserSettings> _userSettings;
        private string _description;
        private string _copyright;
        private string _credits;
        private Viewport.TwoPlayerPreference _2PPref = Viewport.TwoPlayerPreference.SplitHorizontally;
        private Viewport.ThreePlayerPreference _3PPref = Viewport.ThreePlayerPreference.PreferFirstPlayer;
        
        [Serialize]
        [Browsable(false)]
        public EngineSettings EngineSettings
        {
            get => _engineSettings;
            set => _engineSettings = value;
        }
        [Serialize]
        [Browsable(false)]
        public UserSettings UserSettings
        {
            get => _userSettings;
            set => _userSettings = value;
        }
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description
        {
            get => _description;
            set => _description = value;
        }
        [Serialize]
        public string Copyright
        {
            get => _copyright;
            set => _copyright = value;
        }
        [Serialize]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Credits
        {
            get => _credits;
            set => _credits = value;
        }
        public Viewport.TwoPlayerPreference TwoPlayerPref
        {
            get => _2PPref;
            set => _2PPref = value;
        }
        public Viewport.ThreePlayerPreference ThreePlayerPref
        {
            get => _3PPref;
            set => _3PPref = value;
        }
    }
}