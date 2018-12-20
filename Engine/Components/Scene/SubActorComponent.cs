using System;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Core.Files;

namespace TheraEngine.Components.Scene
{
    public class SubActorComponent<T> : SceneComponent where T : class, IActor
    {
        private LocalFileRef<T> _actor;

        [TSerialize]
        public LocalFileRef<T> Actor
        {
            get => _actor;
            set
            {
                if (_actor != null)
                {
                    _actor.UnregisterLoadEvent(ActorLoaded);
                    _actor.UnregisterUnloadEvent(ActorUnloaded);
                }
                _actor = value;
                if (_actor != null)
                {
                    _actor.RegisterLoadEvent(ActorLoaded);
                    _actor.RegisterUnloadEvent(ActorUnloaded);
                }
            }
        }
        private void ActorLoaded(T actor)
        {
            actor.RootComponent.ParentSocket = this;
            if (IsSpawned)
                Actor?.File?.Spawned(OwningWorld);
        }
        private void ActorUnloaded(T actor)
        {
            actor.RootComponent.ParentSocket = this;
            if (IsSpawned)
                Actor?.File?.Spawned(OwningWorld);
        }
        public override void OnSpawned()
        {
            Actor?.File?.Spawned(OwningWorld);
        }
        public override void OnDespawned()
        {
            Actor?.File?.Despawned();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
