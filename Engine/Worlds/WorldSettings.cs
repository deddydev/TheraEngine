using TheraEngine.Files;
using TheraEngine.GameModes;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Audio;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Cutscenes;

namespace TheraEngine.Worlds
{
    public delegate void GravityChange(Vec3 oldGravity);
    public delegate void GameModeChange(BaseGameMode oldMode);
    public delegate void TimeMultiplierChange(float oldMult);
    [FileExt("wset")]
    [FileDef("World Settings")]
    public class WorldSettings : TFileObject
    {
        public GravityChange GravityChanged;
        public GameModeChange GameModeOverrideChanged;
        public TimeMultiplierChange TimeMultiplierChanged;

        public void OnGravityChanged(Vec3 oldGravity) => GravityChanged?.Invoke(oldGravity);
        public void OnGameModeOverrideChanged(BaseGameMode oldMode) => GameModeOverrideChanged?.Invoke(oldMode);
        public void OnTimeMultiplierChanged(float oldMult) => TimeMultiplierChanged?.Invoke(oldMult);

        //[TypeConverter(typeof(Vec3StringConverter))]
        [Category("World")]
        public Vec3 Gravity
        {
            get => _gravity;
            set
            {
                Vec3 oldGravity = _gravity;
                _gravity = value;
                OnGravityChanged(oldGravity);
            }
        }
        [Category("Gameplay")]
        /// <summary>
        /// Overrides the default game mode specified by the game.
        /// </summary>
        public GlobalFileRef<BaseGameMode> GameModeOverrideRef
        {
            get => _gameModeOverrideRef;
            set
            {
                BaseGameMode oldMode = _gameModeOverrideRef;
                _gameModeOverrideRef = value;
                OnGameModeOverrideChanged(oldMode);
            }
        }
        /// <summary>
        /// How fast the game moves. 
        /// A value of 2 will make the game 2x faster,
        /// while a value of 0.5 will make it 2x slower.
        /// </summary>
        [Description(
            "How fast the game moves. " +
            "A value of 2 will make the game 2x faster, " +
            "while a value of 0.5 will make it 2x slower.")]
        [Category("World")]
        public float TimeDilation
        {
            get => _timeSpeed;
            set
            {
                float oldTimeSpeed = _timeSpeed;
                _timeSpeed = value;
                OnTimeMultiplierChanged(oldTimeSpeed);
                Engine.TimeDilation = _timeSpeed;
            }
        }

        [TSerialize(nameof(Gravity))]
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);

        [TSerialize(nameof(GameModeOverrideRef))]
        private GlobalFileRef<BaseGameMode> _gameModeOverrideRef;

        [TSerialize(nameof(TimeDilation))]
        private float _timeSpeed = 1.0f;

        [TSerialize(nameof(Cutscenes))]
        private EventDictionary<string, Cutscene> _cutscenes;

        //[TSerialize(nameof(GlobalAmbient))]
        //private ColorF3 _globalAmbient;

        [TSerialize(nameof(DefaultHud))]
        private IUserInterface _defaultHud;

        [TSerialize(nameof(Bounds))]
        private BoundingBox _bounds = BoundingBox.FromMinMax(-1000.0f, 1000.0f);

        [TSerialize(nameof(OriginRebaseBounds))]
        private BoundingBox _originRebaseBounds = BoundingBox.FromMinMax(-500.0f, 500.0f);

        [TSerialize(nameof(Maps))]
        private List<LocalFileRef<Map>> _maps = new List<LocalFileRef<Map>>();

        [TSerialize(nameof(AmbientSound))]
        private SoundFile _ambientSound;

        [TSerialize(nameof(AmbientParams))]
        private AudioSourceParameters _ambientParams = new AudioSourceParameters()
        {
            SourceRelative = new UsableValue<bool>(true, false, true),
            Gain = new UsableValue<float>(0.6f, 1.0f, true),
            Loop = new UsableValue<bool>(true, false, true),
        };

        /// <summary>
        /// Determines if the origin of the world should be moved to keep the local players closest to it.
        /// This is useful for open-world games as the player will be moving far from the original origin
        /// and floating-point precision will become very poor at large distances.
        /// </summary>
        [Description("Determines if the origin of the world should be moved to keep the local players closest to it." +
            "This is useful for open-world games as the player will be moving far from the original origin" +
            " and floating-point precision will become very poor at large distances.")]
        [Category("World Origin Rebasing")]
        public bool EnableOriginRebasing { get; set; } = false;
        [Category("World Origin Rebasing")]
        public BoundingBox OriginRebaseBounds
        {
            get => _originRebaseBounds;
            set => _originRebaseBounds = value;
        }
        [Category("World")]
        public BoundingBox Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }
        [Category("World")]
        public EventDictionary<string, Cutscene> Cutscenes
        {
            get => _cutscenes;
            set
            {
                if (_cutscenes != null)
                {

                }
                _cutscenes = value;
                if (_cutscenes != null)
                {

                }
            }
        }
        [Category("Audio")]
        public SoundFile AmbientSound
        {
            get => _ambientSound;
            set => _ambientSound = value;
        }
        [Category("World")]
        public List<LocalFileRef<Map>> Maps
        {
            get => _maps;
            set => _maps = value;
        }
        [Browsable(false)]
        public AudioSourceParameters AmbientParams
        {
            get => _ambientParams;
            set => _ambientParams = value;
        }
        [Browsable(false)]
        public IUserInterface DefaultHud
        {
            get => _defaultHud;
            set => _defaultHud = value;
        }
        //[Category("Lighting")]
        //public ColorF3 GlobalAmbient
        //{
        //    get => _globalAmbient;
        //    set => _globalAmbient = value;
        //}
        public List<TMaterial> CollectDefaultMaterials()
        {
            foreach (Map m in _maps)
            {
                if (m.Settings.VisibleByDefault)
                {

                }
            }
            return null;
        }

        public WorldSettings()
        {

        }
        public WorldSettings(string name, params Map[] maps)
        {
            _maps = maps.Select(x => new LocalFileRef<Map>(x)).ToList();
            _originRebaseBounds = _bounds;
            _name = name;
        }
        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new BoundingBox(distance);
        }
    }
}
