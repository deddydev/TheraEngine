using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors
{
    public abstract class Component : FileObject
    {
        /// <summary>
        /// Determines if this component was constructed by code and cannot be removed.
        /// </summary>
        public bool Locked => _locked;
        public virtual IActor OwningActor
        {
            get => _owner;
            set => _owner = value;
        }

        private IActor _owner;
        public bool _locked = true;

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() { }
    }
}
