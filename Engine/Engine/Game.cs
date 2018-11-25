using TheraEngine.Core.Files;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using TheraEngine.Rendering.Text;
using TheraEngine.GameModes;
using System;
using System.Drawing;
using System.IO;

namespace TheraEngine
{
    /// <summary>
    /// Contains all information needed to run any game using the engine.
    /// </summary>
    [TFileExt("tgame")]
    [TFileDef("Game Info", "Contains all information needed to run any game using the engine.")]
    public class TGame : TFileObject
    {
        public TGame()
        {
        }

        protected LocalizedStringTable _localizedStringTable;

        [TSerialize(nameof(TransitionWorldRef))]
        protected GlobalFileRef<TWorld> _transitionWorldRef
            = new GlobalFileRef<TWorld>("TransitionWorld.xworld");

        [TSerialize(nameof(OpeningWorldRef))]
        protected GlobalFileRef<TWorld> _openingWorldRef
            = new GlobalFileRef<TWorld>("OpeningWorld.xworld");

        [TSerialize(nameof(UserSettingsRef))]
        protected GlobalFileRef<UserSettings> _userSettingsRef;

        [TSerialize(nameof(EngineSettingsOverrideRef))]
        protected GlobalFileRef<EngineSettings> _engineSettingsRef;

        [TSerialize(nameof(DefaultGameModeRef))]
        protected GlobalFileRef<BaseGameMode> _gameModeRef;

        /// <summary>
        /// The world the engine uses as a loading screen.
        /// </summary>
        [Category("Game")]
        public GlobalFileRef<TWorld> TransitionWorldRef
        {
            get => _transitionWorldRef;
            set => _transitionWorldRef = value;
        }
        /// <summary>
        /// The world the game starts with.
        /// </summary>
        [Category("Game")]
        public GlobalFileRef<TWorld> OpeningWorldRef
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
    [TFileExt("state")]
    [TFileDef("Game State")]
    public class GameState : TFileObject
    {
        private GlobalFileRef<TWorld> _worldRef = new GlobalFileRef<TWorld>();
        private GlobalFileRef<BaseGameMode> _gameModeRef = new GlobalFileRef<BaseGameMode>();

        [Browsable(false)]
        public TWorld World
        {
            get => _worldRef.File;
            set => _worldRef.File = value;
        }
        [Browsable(false)]
        public BaseGameMode GameMode
        {
            get => _gameModeRef.File;
            set => _gameModeRef.File = value;
        }

        public GlobalFileRef<TWorld> WorldRef
        {
            get => _worldRef;
            set => _worldRef = value ?? new GlobalFileRef<TWorld>();
        }
        public GlobalFileRef<BaseGameMode> GameModeRef
        {
            get => _gameModeRef;
            set => _gameModeRef = value ?? new GlobalFileRef<BaseGameMode>();
        }
    }
}