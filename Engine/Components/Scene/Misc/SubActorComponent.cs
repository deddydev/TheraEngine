using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene
{
    public interface ISubActorComponent : ITRSComponent
    {
        BaseActor Actor { get; }
    }
    public class SubActorComponent<T> : TRSComponent, ISubActorComponent where T : BaseActor
    {
        private T _actor;

        [TSerialize]
        public T Actor
        {
            get => _actor;
            set
            {
                OriginRebasableComponent oldRoot = _actor?.RootComponentGeneric;
                if (_actor != null)
                {
                    _actor.RootComponentChanged -= RootComponentChanged;
                    var root = _actor.RootComponentGeneric;
                    if (root != null && root.ParentSocket == this)
                        root.ParentSocket = null;
                    if (IsSpawned)
                        _actor.Despawned();
                }
                _actor = value;
                if (_actor != null)
                {
                    _actor.RootComponentChanged += RootComponentChanged;
                    var root = _actor.RootComponentGeneric;
                    if (root != null)
                        root.ParentSocket = this;
                    if (IsSpawned)
                        _actor.Spawned(OwningWorld);
                }
            }
        }
        protected void RootComponentChanged(BaseActor actor, OriginRebasableComponent oldRoot)
        {
            if (oldRoot != null && oldRoot.ParentSocket == this)
            {
                if (actor.IsSpawned)
                    oldRoot.OnDespawned();
                oldRoot.ParentSocket = null;
            }
            if (actor.RootComponentGeneric != null)
            {
                actor.RootComponentGeneric.ParentSocket = this;
                if (actor.IsSpawned)
                    actor.RootComponentGeneric.OnSpawned();
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

        BaseActor ISubActorComponent.Actor => Actor;
    }
}
