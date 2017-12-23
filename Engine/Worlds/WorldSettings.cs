using TheraEngine.Files;
using TheraEngine.GameModes;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Audio;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Types.Pawns;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Worlds
{
    public delegate void GravityChange(Vec3 oldGravity);
    public delegate void GameModeChange(BaseGameMode oldMode);
    public delegate void TimeMultiplierChange(float oldMult);
    public class WorldSettings : FileObject
    {
        public GravityChange GravityChanged;
        public GameModeChange GameModeOverrideChanged;
        public TimeMultiplierChange TimeMultiplierChanged;

        public void OnGravityChanged(Vec3 oldGravity) => GravityChanged?.Invoke(oldGravity);
        public void OnGameModeOverrideChanged(BaseGameMode oldMode) => GameModeOverrideChanged?.Invoke(oldMode);
        public void OnTimeMultiplierChanged(float oldMult) => TimeMultiplierChanged?.Invoke(oldMult);

        //[TypeConverter(typeof(Vec3StringConverter))]
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
        /// <summary>
        /// Overrides the default game mode specified by the game.
        /// </summary>
        public GlobalFileRef<BaseGameMode> GameModeOverrideRef
        {
            get => _gameMode;
            set
            {
                BaseGameMode oldMode = _gameMode;
                _gameMode = value;
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
        [TSerialize("Gravity")]
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        [TSerialize("GameMode")]
        private GlobalFileRef<BaseGameMode> _gameMode;
        [TSerialize("TimeDilation")]
        private float _timeSpeed = 1.0f;
        
        public BoundingBox OriginRebaseBounds
        {
            get => _originRebaseBounds;
            set => _originRebaseBounds = value;
        }
        public BoundingBox Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }
        public SoundFile AmbientSound
        {
            get => _ambientSound;
            set => _ambientSound = value;
        }
        [Browsable(false)]
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
        public UIManager DefaultHud
        {
            get => _defaultHud;
            set => _defaultHud = value;
        }
        public ColorF3 GlobalAmbient
        {
            get => _globalAmbient;
            set => _globalAmbient = value;
        }

        [TSerialize("GlobalAmbient")]
        private ColorF3 _globalAmbient;
        [TSerialize("DefaultHud")]
        private UIManager _defaultHud;
        [TSerialize("Bounds")]
        private BoundingBox _bounds = BoundingBox.FromMinMax(-70.0f, 70.0f);
        [TSerialize("OriginRebaseBounds")]
        private BoundingBox _originRebaseBounds = BoundingBox.FromMinMax(float.MinValue, float.MaxValue);
        [TSerialize("Maps")]
        private List<LocalFileRef<Map>> _maps = new List<LocalFileRef<Map>>();
        [TSerialize("AmbientSound")]
        private SoundFile _ambientSound;
        [TSerialize("AmbientParams")]
        private AudioSourceParameters _ambientParams = new AudioSourceParameters()
        {
            SourceRelative = new UsableValue<bool>(true, false, true),
            Gain = new UsableValue<float>(0.6f, 1.0f, true),
            Loop = new UsableValue<bool>(true, false, true),
        };

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
