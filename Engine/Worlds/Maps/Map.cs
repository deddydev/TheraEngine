using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds
{
    [TFileExt("map")]
    [TFileDef("Map")]
    public class Map : TFileObject
    {
        public World OwningWorld { get; internal set; }

        protected bool _visibleByDefault;
        protected EventList<BaseActor> _actors = new EventList<BaseActor>();
        protected Vec3 _spawnPosition;

        [Browsable(true)]
        public override string Name { get => base.Name; set => base.Name = value; }

        public Map() : this(null) { }
        public Map(IEnumerable<BaseActor> actors) : this(true, Vec3.Zero, actors) { }
        public Map(params BaseActor[] actors) : this(true, Vec3.Zero, actors) { }
        public Map(bool visibleAtSpawn, Vec3 spawnOrigin, params BaseActor[] actors)
            : this(visibleAtSpawn, spawnOrigin, actors as IEnumerable<BaseActor>) { }
        public Map(bool visibleAtSpawn, Vec3 spawnOrigin, IEnumerable<BaseActor> actors)
        {
            _visibleByDefault = visibleAtSpawn;
            _spawnPosition = spawnOrigin;

            Actors = actors == null ? new EventList<BaseActor>(false, false) : new EventList<BaseActor>(actors.ToList(), false, false);
        }
        
        [TSerialize]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
        [TSerialize]
        public Vec3 SpawnPosition
        {
            get => _spawnPosition;
            set => _spawnPosition = value;
        }

        [TSerialize]
        public EventList<BaseActor> Actors
        {
            get => _actors;
            set
            {
                if (_actors != null)
                {
                    _actors.PostAnythingAdded -= _actors_PostAnythingAdded;
                    _actors.PostAnythingRemoved -= _actors_PostAnythingRemoved;
                    _actors.ForEach(_actors_PostAnythingRemoved);
                }
                _actors = value;
                if (_actors != null)
                {
                    _actors.PostAnythingAdded += _actors_PostAnythingAdded;
                    _actors.PostAnythingRemoved += _actors_PostAnythingRemoved;
                    _actors.ForEach(_actors_PostAnythingAdded);
                }
            }
        }

        private void _actors_PostAnythingAdded(BaseActor item)
        {
            if (item == null)
                return;
            
            if (item.IsSpawned && !item.IsSpawnedIn(OwningWorld))
            {
                item.Despawn();
                item.MapAttachment = this;
                if (OwningWorld.IsPlaying)
                    OwningWorld.SpawnActor(item);
            }
            else
                item.MapAttachment = this;
        }
        private void _actors_PostAnythingRemoved(BaseActor item)
        {
            if (item != null && item.MapAttachment == this)
                item.MapAttachment = null;
        }

        public virtual void EndPlay()
        {
            foreach (BaseActor actor in Actors)
                OwningWorld.DespawnActor(actor);
            OwningWorld = null;
        }
        public virtual void BeginPlay(World world)
        {
            OwningWorld = world;
            foreach (BaseActor actor in Actors)
                OwningWorld.SpawnActor(actor, SpawnPosition);
        }
    }
}
