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
                _actor?.UnregisterLoadEvent(ActorLoaded);
                _actor = value;
                _actor?.RegisterLoadEvent(ActorLoaded);
            }
        }
        private void ActorLoaded(T actor)
        {
            actor.RootComponent.ParentSocket = this; 
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
