using System;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene
{
    public class SubActorComponent<T> : TRSComponent where T : class, IActor
    {
        private T _actor;

        [TSerialize]
        public T Actor
        {
            get => _actor;
            set
            {
                var oldRoot = _actor?.RootComponent;
                if (_actor != null)
                    _actor.RootComponentChanged -= RootComponentChanged;
                _actor = value;
                var newRoot = _actor?.RootComponent;
                if (_actor != null)
                    _actor.RootComponentChanged += RootComponentChanged;
                RootComponentChanged(oldRoot, newRoot);
            }
        }
        private void RootComponentChanged(OriginRebasableComponent oldRoot, OriginRebasableComponent newRoot)
        {
            if (oldRoot != null && oldRoot.ParentSocket == this)
                oldRoot.ParentSocket = null;
            if (newRoot != null)
                newRoot.ParentSocket = this;
        }
    }
}
