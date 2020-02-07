using TheraEngine.Actors;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene
{
    public interface ISubActorComponent : ITRSComponent
    {
        IActor Actor { get; }
    }
    public class SubActorComponent<T> : TRSComponent, ISubActorComponent where T : class, IActor
    {
        private T _actor;

        [TSerialize]
        public T Actor
        {
            get => _actor;
            set
            {
                IOriginRebasableComponent oldRoot = _actor?.RootComponent;
                if (_actor != null)
                {
                    _actor.RootComponentChanged -= RootComponentChanged;
                    var root = _actor.RootComponent;
                    if (root != null && root.ParentSocket == this)
                        root.ParentSocket = null;
                    if (IsSpawned)
                        _actor.Despawned();
                }
                _actor = value;
                if (_actor != null)
                {
                    _actor.RootComponentChanged += RootComponentChanged;
                    var root = _actor.RootComponent;
                    if (root != null)
                        root.ParentSocket = this;
                    if (IsSpawned)
                        _actor.Spawned(OwningWorld);
                }
            }
        }
        protected void RootComponentChanged(IActor actor, IOriginRebasableComponent oldRoot)
        {
            if (oldRoot != null && oldRoot.ParentSocket == this)
            {
                if (actor.IsSpawned)
                    oldRoot.Despawn(OwningActor);
                oldRoot.ParentSocket = null;
            }
            if (actor.RootComponent != null)
            {
                actor.RootComponent.ParentSocket = this;
                if (actor.IsSpawned)
                    actor.RootComponent.Spawn(OwningActor);
            }
        }
        public override void OnSpawned()
        {
            Actor?.Spawned(OwningWorld);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            Actor?.Despawned();
            base.OnDespawned();
        }

        IActor ISubActorComponent.Actor => Actor;
    }
}
