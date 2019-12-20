using TheraEngine.Core.Files;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using TheraEngine.Rendering.Text;
using TheraEngine.GameModes;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace TheraEngine
{
    /// <summary>
    /// Contains all information needed to run any game using the engine.
    /// </summary>
    [TFileExt("tgame")]
    [TFileDef("Game Info", "Contains all information needed to run any game using the engine.")]
    public class TGame : TFileObject
    {
        public TGame() { }

        [TSerialize(nameof(TransitionWorldRef))]
        public GlobalFileRef<World> _transitionWorldRef
            = new GlobalFileRef<World>("TransitionWorld.xworld");

        [TSerialize(nameof(OpeningWorldRef))]
        public GlobalFileRef<World> _openingWorldRef
            = new GlobalFileRef<World>("OpeningWorld.xworld");

        [TSerialize(nameof(UserSettingsRef))]
        public GlobalFileRef<UserSettings> _userSettingsRef;

        [TSerialize(nameof(EngineSettingsOverrideRef))]
        public GlobalFileRef<EngineSettings> _engineSettingsRef;

        [TSerialize(nameof(DefaultGameModeRef))]
        public GlobalFileRef<BaseGameMode> _gameModeRef;

        private LocalizedStringTable _localizedStringTable;
        private List<GlobalFileRef<World>> _worldCollection;
        private string _description;
        private string _copyright;
        private string _credits;
        private string _iconPath;
        private Viewport.ETwoPlayerPreference _twoPlayerPref = Viewport.ETwoPlayerPreference.SplitHorizontally;
        private Viewport.EThreePlayerPreference _threePlayerPref = Viewport.EThreePlayerPreference.PreferFirstPlayer;
        private GameState _state = new GameState();

        public List<GlobalFileRef<World>> WorldCollection
        {
            get => _worldCollection;
            set => Set(ref _worldCollection, value ?? new List<GlobalFileRef<World>>());
        }

        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        public GlobalFileRef<World> TransitionWorldRef
        {
            get => _transitionWorldRef;
            set => Set(ref _transitionWorldRef, value);
        }
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        public GlobalFileRef<World> OpeningWorldRef
        {
            get => _openingWorldRef;
            set => Set(ref _openingWorldRef, value);
        }
        [Category("Game")]
        public GlobalFileRef<BaseGameMode> DefaultGameModeRef
        {
            get => _gameModeRef;
            set => Set(ref _gameModeRef, value);
        }
        [Category("Engine")]
        public GlobalFileRef<UserSettings> UserSettingsRef
        {
            get => _userSettingsRef;
            set => Set(ref _userSettingsRef, value);
        }
        [Category("Engine")]
        public GlobalFileRef<EngineSettings> EngineSettingsOverrideRef
        {
            get => _engineSettingsRef;
            set => Set(ref _engineSettingsRef, value);
        }

        [Category("About")]
        [TSerialize]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        [Category("About")]
        [TSerialize]
        public string Copyright
        {
            get => _copyright;
            set => Set(ref _copyright, value);
        }

        [Category("About")]
        [TSerialize]
        public string Credits
        {
            get => _credits;
            set => Set(ref _credits, value);
        }

        [Category("About")]
        [TSerialize]
        public string IconPath
        {
            get => _iconPath;
            set => Set(ref _iconPath, value);
        }

        [Category("Viewports")]
        [TSerialize]
        public Viewport.ETwoPlayerPreference TwoPlayerPref
        {
            get => _twoPlayerPref;
            set => Set(ref _twoPlayerPref, value);
        }

        [Category("Viewports")]
        [TSerialize]
        public Viewport.EThreePlayerPreference ThreePlayerPref
        {
            get => _threePlayerPref;
            set => Set(ref _threePlayerPref, value);
        }

        [TSerialize]
        [Category("Text")]
        public LocalizedStringTable LocalizedStringTable
        {
            get => _localizedStringTable;
            set => Set(ref _localizedStringTable, value);
        }

        [TSerialize(Config = false, State = true)]
        [Category("Engine")]
        public virtual GameState State 
        {
            get => _state;
            set => Set(ref _state, value);
        }

        public Icon GetIcon()
        {
            if (!string.IsNullOrEmpty(IconPath) && File.Exists(IconPath))
                return new Icon(IconPath);
            return null;
        }
    }

    /// <summary>
    /// Contains all information pertaining to the game's current state in the engine.
    /// </summary>
    [Serializable]
    [TFileExt("state")]
    [TFileDef("Game State")]
    public class GameState : TFileObject
    {
        private GlobalFileRef<World> _worldRef = new GlobalFileRef<World>();
        private GlobalFileRef<BaseGameMode> _gameModeRef = new GlobalFileRef<BaseGameMode>();

        [Browsable(false)]
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
    }
}