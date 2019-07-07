using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Animation.Cutscenes;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;

namespace TheraEngine.Worlds
{
    public interface IWorldSettings : IFileObject
    {
        event GravityChange GravityChanged;
        event GameModeChange GameModeOverrideChanged;
        event TimeMultiplierChange TimeMultiplierChanged;
        event Action<WorldSettings> EnableOriginRebasingChanged;

        void OnGravityChanged(Vec3 oldGravity);
        void OnGameModeOverrideChanged(IGameMode oldMode);
        void OnTimeMultiplierChanged(float oldMult);
        void OnEnableOriginRebasingChanged();

        IWorld OwningWorld { get; set; }

        Vec3 Gravity { get; set; }
        GlobalFileRef<IGameMode> DefaultGameModeRef { get; set; }
        float TimeDilation { get; set; }
        bool TwoDimensional { get; set; }
        bool EnableOriginRebasing { get; set; }
        [Category("World Origin Rebasing")]
        [TSerialize]
        float OriginRebaseRadius { get; set; }
        [TSerialize]
        [Category("Editor Traits")]
        bool PreviewOctrees { get; set; }
        [TSerialize]
        [Category("Editor Traits")]
        bool PreviewQuadtrees { get; set; }
        bool PreviewPhysics { get; set; }

        string NewActorTargetMapName { get; set; }
        BoundingBox Bounds { get; set; }
        string CutsceneToPlayOnBeginPlay { get; set; }
        EventDictionary<string, Cutscene> Cutscenes { get; set; }
        EventDictionary<string, LocalFileRef<IMap>> Maps { get; set; }
        IMap FindOrCreateMap(string name);
    }

    public delegate void GravityChange(WorldSettings settings, Vec3 oldGravity);
    public delegate void GameModeChange(WorldSettings settings, IGameMode oldMode);
    public delegate void TimeMultiplierChange(WorldSettings settings, float oldMult);

    [Serializable]
    [TFileExt("wset")]
    [TFileDef("World Settings")]
    public class WorldSettings : TFileObject, IWorldSettings
    {
        public event GravityChange GravityChanged;
        public event GameModeChange GameModeOverrideChanged;
        public event TimeMultiplierChange TimeMultiplierChanged;
        public event Action<WorldSettings> EnableOriginRebasingChanged;

        public void OnGravityChanged(Vec3 oldGravity) => GravityChanged?.Invoke(this, oldGravity);
        public void OnGameModeOverrideChanged(IGameMode oldMode) => GameModeOverrideChanged?.Invoke(this, oldMode);
        public void OnTimeMultiplierChanged(float oldMult) => TimeMultiplierChanged?.Invoke(this, oldMult);
        public void OnEnableOriginRebasingChanged() => EnableOriginRebasingChanged?.Invoke(this);

        [Browsable(false)]
        public IWorld OwningWorld { get; set; }

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
        /// <summary>
        /// Overrides the default game mode specified by the game.
        /// </summary>
        [Category("Gameplay")]
        public GlobalFileRef<IGameMode> DefaultGameModeRef
        {
            get => _gameModeOverrideRef;
            set
            {
                IGameMode oldMode = _gameModeOverrideRef.File;
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

        [TSerialize(nameof(DefaultGameModeRef))]
        private GlobalFileRef<IGameMode> _gameModeOverrideRef;

        [TSerialize(nameof(TimeDilation))]
        private float _timeSpeed = 1.0f;

        [TSerialize(nameof(Cutscenes))]
        private EventDictionary<string, Cutscene> _cutscenes;

        [TSerialize(nameof(Maps))]
        private EventDictionary<string, LocalFileRef<IMap>> _maps;

        [TSerialize]
        [Category("World")]
        [DisplayName("2D")]
        public bool TwoDimensional { get; set; } = false;

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
        [TSerialize]
        public float OriginRebaseRadius { get; set; } = 500.0f;
        [TSerialize]
        [Category("Editor Traits")]
        public bool PreviewOctrees { get; set; } = false;
        [TSerialize]
        [Category("Editor Traits")]
        public bool PreviewQuadtrees { get; set; } = false;
        [TSerialize]
        [Category("Editor Traits")]
        public bool PreviewPhysics { get; set; } = false;

        private const string NewActorTargetMapNameDefault = "DefaultMap";
        private string _newActorTargetMapName = NewActorTargetMapNameDefault;
        [TSerialize]
        [Category("Editor Traits")]
        public string NewActorTargetMapName
        {
            get => _newActorTargetMapName ?? NewActorTargetMapNameDefault;
            set => _newActorTargetMapName = string.IsNullOrWhiteSpace(value) ? NewActorTargetMapNameDefault : value;
        }

        [Category("World")]
        [TSerialize]
        public BoundingBox Bounds { get; set; } = BoundingBox.FromMinMax(-1000.0f, 1000.0f);
        [TSerialize]
        [Category("World")]
        public string CutsceneToPlayOnBeginPlay { get; set; }
        [TSerialize]
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

        [Category("World")]
        public EventDictionary<string, LocalFileRef<IMap>> Maps
        {
            get => _maps;
            set
            {
                if (_maps != null)
                {
                    _maps.Added -= _maps_Added;
                    _maps.Removed -= _maps_Removed;
                    _maps.Set -= _maps_Set;
                }

                _maps = value ?? new EventDictionary<string, LocalFileRef<IMap>>();

                _maps.Added += _maps_Added;
                _maps.Removed += _maps_Removed;
                _maps.Set += _maps_Set;

                foreach (var mapRef in _maps)
                    _maps_Added(mapRef.Key, mapRef.Value);
            }
        }

        private void _maps_Set(string key, LocalFileRef<IMap> oldValue, LocalFileRef<IMap> newValue)
        {
            _maps_Removed(key, oldValue);
            _maps_Added(key, newValue);
        }
        private void _maps_Removed(string key, LocalFileRef<IMap> value)
        {
            if (value == null)
                return;
            value.UnregisterLoadEvent(MapLoaded);
            value.UnregisterUnloadEvent(MapUnloaded);
        }
        private void _maps_Added(string key, LocalFileRef<IMap> value)
        {
            if (value == null)
                return;
            value.RegisterLoadEvent(MapLoaded);
            value.RegisterUnloadEvent(MapUnloaded);
        }

        public IMap FindOrCreateMap(string name)
        {
            IMap map;
            if (Maps != null)
            {
                if (Maps.ContainsKey(name))
                    map = Maps[name].File;
                else
                {
                    map = new Map();
                    Maps.Add(name, new LocalFileRef<IMap>(map));
                }
            }
            else
            {
                map = new Map();
                Maps = new EventDictionary<string, LocalFileRef<IMap>> { { name, new LocalFileRef<IMap>(map) } };
            }
            return map;
        }
        
        private void MapUnloaded(IMap map)
        {
            if (map.OwningWorld == OwningWorld)
                map.OwningWorld = null;
        }
        private void MapLoaded(IMap map)
        {
            map.OwningWorld = OwningWorld;
        }

        public WorldSettings()
        {
            Maps = new EventDictionary<string, LocalFileRef<IMap>>();
            Cutscenes = new EventDictionary<string, Cutscene>();
        }
        public WorldSettings(string name, params Map[] maps)
        {
            _name = name;
            OriginRebaseRadius = TMath.Min(
                Bounds.Maximum.X, Bounds.Maximum.Y, Bounds.Maximum.Z,
                Bounds.Minimum.X, Bounds.Minimum.Y, Bounds.Minimum.Z);

            Maps = new EventDictionary<string, LocalFileRef<IMap>>(maps.ToDictionary(x => x.Name, x => new LocalFileRef<IMap>(x)));
            Cutscenes = new EventDictionary<string, Cutscene>();
        }
    }
}
