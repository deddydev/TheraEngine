using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

namespace CustomEngine.Worlds
{
    public delegate void GravityChange(Vec3 oldGravity);
    public delegate void GameModeChange(GameMode oldMode);
    public delegate void TimeMultiplierChange(float oldMult);
    public class WorldState : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.WorldState; } }

        public GravityChange GravityChanged;
        public GameModeChange GameModeChanged;
        public TimeMultiplierChange TimeMultiplierChanged;
        
        public Vec3 Gravity
        {
            get { return _gravity; }
            set
            {
                Vec3 oldGravity = _gravity;
                _gravity = value;
                OnGravityChanged(oldGravity);
            }
        }
        public GameMode GameMode
        {
            get { return _gameMode; }
            set
            {
                GameMode oldMode = _gameMode;
                _gameMode = value;
                OnGameModeChanged(oldMode);
            }
        }
        public float TimeDilation
        {
            get { return _timeSpeed; }
            set
            {
                float oldTimeSpeed = _timeSpeed;
                _timeSpeed = value;
                OnTimeMultiplierChanged(oldTimeSpeed);
            }
        }
        public List<Map> SpawnedMaps { get { return _spawnedMaps; } }
        public List<Actor> SpawnedActors { get { return _spawnedActors; } }

        public void OnGravityChanged(Vec3 oldGravity) { GravityChanged?.Invoke(oldGravity); }
        public void OnGameModeChanged(GameMode oldMode) { GameModeChanged?.Invoke(oldMode); }
        public void OnTimeMultiplierChanged(float oldMult) { TimeMultiplierChanged?.Invoke(oldMult); }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        private List<Map> _spawnedMaps = new List<Map>();
        private List<Actor> _spawnedActors = new List<Actor>();
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        private GameMode _gameMode;
        private float _timeSpeed = 1.0f;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WorldStateHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
