using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        [PostChanged("OnGravityChanged")]
        public Vec3 Gravity { get { return _gravity; } set { _gravity = value; } }
        [PostChanged("OnGameModeChanged")]
        public GameMode GameMode { get { return _gameMode; } set { _gameMode = value; } }
        [PostChanged("OnTimeMultiplierChanged")]
        public float TimeDilation { get { return _timeSpeed; } set { _timeSpeed = value; } }
        public List<Map> SpawnedMaps { get { return _spawnedMaps; } }
        public List<Actor> SpawnedActors { get { return _spawnedActors; } }

        public void OnGravityChanged(Vec3 oldGravity) { GravityChanged?.Invoke(oldGravity); }
        public void OnGameModeChanged(GameMode oldMode) { GameModeChanged?.Invoke(oldMode); }
        public void OnTimeMultiplierChanged(float oldMult) { TimeMultiplierChanged?.Invoke(oldMult); }

        private List<Map> _spawnedMaps = new List<Map>();
        private List<Actor> _spawnedActors = new List<Actor>();
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        private GameMode _gameMode;
        private float _timeSpeed = 1.0f;
        public HashSet<Material> _activeMaterials;

        public static string GetTag() { return "WSTA"; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WorldStateHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
