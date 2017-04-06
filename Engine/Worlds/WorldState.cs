﻿using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace CustomEngine.Worlds
{
    public delegate void GravityChange(Vec3 oldGravity);
    public delegate void GameModeChange(GameMode oldMode);
    public delegate void TimeMultiplierChange(float oldMult);
    public class WorldState : FileObject
    {
        public GravityChange GravityChanged;
        public GameModeChange GameModeChanged;
        public TimeMultiplierChange TimeMultiplierChanged;

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

        public List<Map> SpawnedMaps => _spawnedMaps;
        public MonitoredList<IActor> SpawnedActors => _spawnedActors;

        public void OnGravityChanged(Vec3 oldGravity) => GravityChanged?.Invoke(oldGravity);
        public void OnGameModeChanged(GameMode oldMode) => GameModeChanged?.Invoke(oldMode);
        public void OnTimeMultiplierChanged(float oldMult) => TimeMultiplierChanged?.Invoke(oldMult);

        [Serialize("SpawnedMaps")]
        private List<Map> _spawnedMaps = new List<Map>();
        [Serialize("SpawnedActors")]
        private MonitoredList<IActor> _spawnedActors = new MonitoredList<IActor>();
        [Serialize("Gravity")]
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        [Serialize("GameMode")]
        private GameMode _gameMode;
        [Serialize("TimeDilation")]
        private float _timeSpeed = 1.0f;

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public unsafe struct Header
        //{
        //    public const int Size = 4;

        //    public BVec3 _gravity;
        //    public float _timeDilation;
        //    public bint _visibleMapCount;
        //    public bint _visibleActorCount;
        //    public GameMode.Header _gameMode;
            
        //    public bint* VisibleMapIndices => (bint*)(Address + Size);
        //    public bint* VisibleActorIndices => (bint*)(Address + Size);

        //    public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        //}
    }
}
