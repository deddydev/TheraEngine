using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene
{
    public class SubActorComponent<T> : TRSComponent where T : BaseActor
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
                    _actor.RootComponentChanged -= RootComponentChanged;
                _actor = value;
                if (_actor != null)
                    _actor.RootComponentChanged += RootComponentChanged;
                RootComponentChanged(_actor, oldRoot);
            }
        }
        protected void RootComponentChanged(BaseActor actor, OriginRebasableComponent oldRoot)
        {
            if (oldRoot != null && oldRoot.ParentSocket == this)
                oldRoot.ParentSocket = null;
            if (actor.RootComponentGeneric != null)
                actor.RootComponentGeneric.ParentSocket = this;
        }
    }
}
