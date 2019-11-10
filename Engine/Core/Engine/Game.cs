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

        public LocalizedStringTable _localizedStringTable;

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

        private List<GlobalFileRef<World>> _worldCollection;
        public List<GlobalFileRef<World>> WorldCollection
        {
            get => _worldCollection;
            set => _worldCollection = value ?? new List<GlobalFileRef<World>>();
        }

        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        public GlobalFileRef<World> TransitionWorldRef
        {
            get => _transitionWorldRef;
            set => _transitionWorldRef = value;
        }
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        public GlobalFileRef<World> OpeningWorldRef
        {
            get => _openingWorldRef;
            set => _openingWorldRef = value;
        }
        [Category("Game")]
        public GlobalFileRef<BaseGameMode> DefaultGameModeRef
        {
            get => _gameModeRef;
            set => _gameModeRef = value;
        }
        [Category("Engine")]
        public GlobalFileRef<UserSettings> UserSettingsRef
        {
            get => _userSettingsRef;
            set => _userSettingsRef = value;
        }
        [Category("Engine")]
        public GlobalFileRef<EngineSettings> EngineSettingsOverrideRef
        {
            get => _engineSettingsRef;
            set => _engineSettingsRef = value;
        }

        [Category("About")]
        [TSerialize]
        public string Description { get; set; }

        [Category("About")]
        [TSerialize]
        public string Copyright { get; set; }

        [Category("About")]
        [TSerialize]
        public string Credits { get; set; }

        [Category("About")]
        [TSerialize]
        public string IconPath { get; set; }

        [Category("Viewports")]
        [TSerialize]
        public Viewport.ETwoPlayerPreference TwoPlayerPref { get; set; } = Viewport.ETwoPlayerPreference.SplitHorizontally;

        [Category("Viewports")]
        [TSerialize]
        public Viewport.EThreePlayerPreference ThreePlayerPref { get; set; } = Viewport.EThreePlayerPreference.PreferFirstPlayer;

        [TSerialize]
        [Category("Text")]
        public LocalizedStringTable LocalizedStringTable
        {
            get => _localizedStringTable;
            set => _localizedStringTable = value;
        }

        public virtual GameState State { get; set; } = new GameState();

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