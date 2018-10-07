using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Core.Files;

namespace TheraEngine.Components
{
    public interface IComponent : IFileObject
    {
        bool Locked { get; }
        IActor OwningActor { get; set; }
        bool IsSpawned { get; }
        void OnSpawned();
        void OnDespawned();
    }
    /// <summary>
    /// Components are plugged into actors to define customizable functionality.
    /// There are two types of components: <see cref="SceneComponent"/> and <see cref="LogicComponent"/>.
    /// </summary>
    [FileExt("comp")]
    public abstract class Component : TFileObject
    {
        private IActor _owner;
        public bool _locked = true;

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

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() { }
    }
}
