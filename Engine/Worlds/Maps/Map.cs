using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using Extensions;

namespace TheraEngine.Worlds
{
    public interface IMap : IFileObject
    {
        IWorld OwningWorld { get; set; }
        bool VisibleByDefault { get; set; }
        Vec3 SpawnPosition { get; set; }
        IEventList<IActor> Actors { get; set; }

        void BeginPlay(IWorld world);
        void EndPlay();
    }
    [TFileExt("map")]
    [TFileDef("Map")]
    public class Map : TFileObject, IMap
    {
        protected bool _visibleByDefault;
        protected IEventList<IActor> _actors = new EventList<IActor>();
        protected Vec3 _spawnPosition;

        [Browsable(true)]
        public override string Name { get => base.Name; set => base.Name = value; }

        public Map() : this(null) { }
        public Map(IEnumerable<IActor> actors) : this(true, Vec3.Zero, actors) { }
        public Map(params IActor[] actors) : this(true, Vec3.Zero, actors) { }
        public Map(bool visibleAtSpawn, Vec3 spawnOrigin, params IActor[] actors)
            : this(visibleAtSpawn, spawnOrigin, actors as IEnumerable<IActor>) { }
        public Map(bool visibleAtSpawn, Vec3 spawnOrigin, IEnumerable<IActor> actors)
        {
            _visibleByDefault = visibleAtSpawn;
            _spawnPosition = spawnOrigin;

            Actors = actors == null ? new EventList<IActor>(false, false) : new EventList<IActor>(actors.ToList(), false, false);
        }

        [Browsable(false)]
        public IWorld OwningWorld { get; set; }

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
        public IEventList<IActor> Actors
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

        private void _actors_PostAnythingAdded(IActor item)
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
        private void _actors_PostAnythingRemoved(IActor item)
        {
            if (item != null && item.MapAttachment == this)
                item.MapAttachment = null;
        }

        public virtual void EndPlay()
        {
            foreach (IActor actor in Actors)
                OwningWorld.DespawnActor(actor);
            OwningWorld = null;
        }
        public virtual void BeginPlay(IWorld world)
        {
            OwningWorld = world;
            foreach (IActor actor in Actors)
                OwningWorld.SpawnActor(actor, SpawnPosition);
        }
    }
}
