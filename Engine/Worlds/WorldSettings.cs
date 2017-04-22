﻿using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Audio;
using CustomEngine.Rendering.HUD;

namespace CustomEngine.Worlds
{
    public delegate void GravityChange(Vec3 oldGravity);
    public delegate void GameModeChange(GameMode oldMode);
    public delegate void TimeMultiplierChange(float oldMult);
    public class WorldSettings : FileObject
    {
        public GravityChange GravityChanged;
        public GameModeChange GameModeChanged;
        public TimeMultiplierChange TimeMultiplierChanged;

        public void OnGravityChanged(Vec3 oldGravity) => GravityChanged?.Invoke(oldGravity);
        public void OnGameModeChanged(GameMode oldMode) => GameModeChanged?.Invoke(oldMode);
        public void OnTimeMultiplierChanged(float oldMult) => TimeMultiplierChanged?.Invoke(oldMult);

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
        public GameMode GameMode
        {
            get => _gameMode;
            set
            {
                GameMode oldMode = _gameMode;
                _gameMode = value;
                OnGameModeChanged(oldMode);
            }
        }
        public float TimeDilation
        {
            get => _timeSpeed;
            set
            {
                float oldTimeSpeed = _timeSpeed;
                _timeSpeed = value;
                OnTimeMultiplierChanged(oldTimeSpeed);
            }
        }
        [Serialize("Gravity")]
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        [Serialize("GameMode")]
        private GameMode _gameMode;
        [Serialize("TimeDilation")]
        private float _timeSpeed = 1.0f;
        public BoundingBox OriginRebaseBounds
            => _originRebaseBounds;
        public BoundingBox Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }
        public WorldState State
        {
            get => _state;
            set => _state = value;
        }
        public SoundFile AmbientSound
        {
            get => _ambientSound;
            set => _ambientSound = value;
        }
        public List<Map> Maps
        {
            get => _maps;
            set => _maps = value;
        }
        public AudioSourceParameters AmbientParams
        {
            get => _ambientParams;
            set => _ambientParams = value;
        }
        public HudManager DefaultHud
        {
            get => _defaultHud;
            set => _defaultHud = value;
        }

        [Serialize("DefaultHud")]
        private HudManager _defaultHud;
        [Serialize("Bounds")]
        private BoundingBox _bounds = BoundingBox.FromMinMax(-500.0f, 500.0f);
        [Serialize("OriginRebaseBounds")]
        private BoundingBox _originRebaseBounds = BoundingBox.FromMinMax(float.MinValue, float.MaxValue);
        [Serialize("Maps")]
        private List<Map> _maps = new List<Map>();
        [Serialize("State")]
        private WorldState _state;
        [Serialize("AmbientSound")]
        private SoundFile _ambientSound;
        [Serialize("AmbientParams")]
        private AudioSourceParameters _ambientParams = new AudioSourceParameters()
        {
            SourceRelative = new UsableValue<bool>(true, false, true),
            Gain = new UsableValue<float>(0.6f, 1.0f, true),
            Loop = new UsableValue<bool>(true, false, true),
        };

        public List<Material> CollectDefaultMaterials()
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
        public WorldSettings(string name, WorldState state, params Map[] maps)
        {
            _maps = maps.ToList();
            _originRebaseBounds = _bounds;
            _name = name;
            _state = state;
        }
        public WorldSettings(string name, WorldState state)
        {
            _originRebaseBounds = _bounds;
            _maps = new List<Map>();
            _name = name;
            _state = state;
        }
        public WorldSettings(string name)
        {
            _originRebaseBounds = _bounds;
            _maps = new List<Map>();
            _name = name;
            _state = new WorldState();
        }

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new BoundingBox(distance);
        }
        public override void Read(VoidPtr address, VoidPtr strings)
        {
            //foreach (Map m in _maps)
            //    if (m.Settings.VisibleByDefault)
            //        m.Load();
            //_state._activeMaterials = new HashSet<Material>(CollectDefaultMaterials());
        }

        public static WorldSettings FromXML(string filePath)
        {
            return FromXML<WorldSettings>(filePath);
        }
    }
}
