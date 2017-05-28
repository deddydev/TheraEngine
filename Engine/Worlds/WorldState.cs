using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomEngine.Worlds
{
    public class WorldState : FileObject
    {
        public List<Map> SpawnedMaps => _spawnedMaps;
        public MonitoredList<IActor> SpawnedActors => _spawnedActors;

        [Serialize("SpawnedMaps")]
        private List<Map> _spawnedMaps = new List<Map>();
        [Serialize("SpawnedActors")]
        private MonitoredList<IActor> _spawnedActors = new MonitoredList<IActor>();

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
