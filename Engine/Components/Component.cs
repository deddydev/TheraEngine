using CustomEngine.World;
using CustomEngine.System;

namespace CustomEngine.Components
{
    public abstract class Component : ObjectBase
    {
        private Actor _owner;
        public Actor Owner { get { return _owner; } set { _owner = value; } }

        private bool _isSpawned;
        public bool IsSpawned { get { return _isSpawned; } }

        public virtual void OnSpawned() { _isSpawned = true; }
        public virtual void OnDespawned() { _isSpawned = false; }
        public virtual void RenderTick(double deltaTime) { }
    }
}
