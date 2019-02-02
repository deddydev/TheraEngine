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
        public TWorld OwningWorld { get; private set; }

        protected bool _visibleByDefault;
        protected List<IActor> _actors = new List<IActor>();
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
            _actors = actors == null ? new List<IActor>() : actors.ToList();
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
        public List<IActor> Actors
        {
            get => _actors;
            set => _actors = value;
        }
        public virtual void EndPlay()
        {
            foreach (IActor actor in Actors)
            {
                OwningWorld.DespawnActor(actor);
                actor.MapAttachment = null;
            }
            OwningWorld = null;
        }
        public virtual void BeginPlay(TWorld world)
        {
            OwningWorld = world;
            foreach (IActor actor in Actors)
            {
                actor.MapAttachment = this;
                OwningWorld.SpawnActor(actor, SpawnPosition);
            }
        }
    }
}
