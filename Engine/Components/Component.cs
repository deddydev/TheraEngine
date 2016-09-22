using CustomEngine.World;

namespace CustomEngine.Components
{
    public abstract class Component
    {
        private Actor _owner;
        public Actor Owner { get { return _owner; } set { _owner = value; } }

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() { }
        public virtual void Render() { }
    }
}
