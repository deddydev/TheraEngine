using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Animation.Cutscenes;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds
{
    public delegate void GravityChange(WorldSettings settings, Vec3 oldGravity);
    public delegate void GameModeChange(WorldSettings settings, BaseGameMode oldMode);
    public delegate void TimeMultiplierChange(WorldSettings settings, float oldMult);
    [TFileExt("wset")]
    [TFileDef("World Settings")]
    public class WorldSettings : TFileObject
    {
        public event GravityChange GravityChanged;
        public event GameModeChange GameModeOverrideChanged;
        public event TimeMultiplierChange TimeMultiplierChanged;
        public event Action<WorldSettings> EnableOriginRebasingChanged;

        public void OnGravityChanged(Vec3 oldGravity) => GravityChanged?.Invoke(this, oldGravity);
        public void OnGameModeOverrideChanged(BaseGameMode oldMode) => GameModeOverrideChanged?.Invoke(this, oldMode);
        public void OnTimeMultiplierChanged(float oldMult) => TimeMultiplierChanged?.Invoke(this, oldMult);
        public void OnEnableOriginRebasingChanged() => EnableOriginRebasingChanged?.Invoke(this);

        public TWorld OwningWorld { get; internal set; }

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
        
        [TSerialize(nameof(Bounds))]
        private BoundingBox _bounds = BoundingBox.FromMinMax(-1000.0f, 1000.0f);

        [TSerialize(nameof(OriginRebaseRadius))]
        private float _originRebaseRadius = 500.0f;

        [TSerialize(nameof(Maps))]
        private EventList<LocalFileRef<Map>> _maps = new EventList<LocalFileRef<Map>>();
        
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
        public float OriginRebaseRadius
        {
            get => _originRebaseRadius;
            set => _originRebaseRadius = value;
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
        [Category("World")]
        public EventList<LocalFileRef<Map>> Maps
        {
            get => _maps;
            set
            {
                if (_maps != null)
                {
                    _maps.PostAnythingAdded -= _maps_PostAnythingAdded;
                    _maps.PostAnythingRemoved -= _maps_PostAnythingRemoved;
                }
                _maps = value ?? new EventList<LocalFileRef<Map>>();
                _maps.PostAnythingAdded += _maps_PostAnythingAdded;
                _maps.PostAnythingRemoved += _maps_PostAnythingRemoved;
                foreach (var map in _maps)
                    _maps_PostAnythingAdded(map);
            }
        }

        private void _maps_PostAnythingRemoved(LocalFileRef<Map> item)
        {
            item.UnregisterLoadEvent(MapLoaded);
            item.UnregisterUnloadEvent(MapUnloaded);
        }
        private void _maps_PostAnythingAdded(LocalFileRef<Map> item)
        {
            item.RegisterLoadEvent(MapLoaded);
            item.RegisterUnloadEvent(MapUnloaded);
        }
        private void MapUnloaded(Map map)
        {
            map.OwningWorld = OwningWorld;
        }
        private void MapLoaded(Map map)
        {
            if (map.OwningWorld == OwningWorld)
                map.OwningWorld = null;
        }
        
        public List<TMaterial> CollectDefaultMaterials()
        {
            foreach (Map m in _maps)
            {
                if (m.VisibleByDefault)
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
            _name = name;
            _originRebaseRadius = TMath.Min(
                _bounds.Maximum.X, _bounds.Maximum.Y, _bounds.Maximum.Z,
                _bounds.Minimum.X, _bounds.Minimum.Y, _bounds.Minimum.Z);

            Maps = new EventList<LocalFileRef<Map>>(maps.Select(x => new LocalFileRef<Map>(x)));
        }
    }
}
