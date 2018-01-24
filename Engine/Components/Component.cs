using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Files;

namespace TheraEngine.Components
{
    [FileExt("comp")]
    public abstract class Component : FileObject
    {
        /// <summary>
        /// Determines if this component was constructed by code and cannot be removed.
        /// </summary>
        [Browsable(false)]
        public bool Locked => _locked;
        [Browsable(false)]
        public virtual IActor OwningActor
        {
            get => _owner;
            set => _owner = value;
        }
        [Browsable(false)]
        public bool IsSpawned => OwningActor?.IsSpawned ?? false;

        private IActor _owner;
        public bool _locked = true;

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() { }
    }
}
