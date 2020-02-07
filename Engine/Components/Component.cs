using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Components
{
    public interface IComponent : IFileObject
    {
        IActor OwningActor { get; set; }

        bool IsSpawned { get; }
        void Spawn(IActor owner);
        void Despawn(IActor owner);
    }
    /// <summary>
    /// Components are plugged into actors to define customizable functionality.
    /// There are two types of components: <see cref="SceneComponent"/> and <see cref="LogicComponent"/>.
    /// </summary>
    [TFileExt("comp")]
    public abstract class Component : TFileObject, IComponent
    {
        [TSerialize(nameof(OwningActor))]
        private IActor _owningActor;

        [Browsable(false)]
        public virtual IActor OwningActor
        {
            get => _owningActor;
            set => Set(ref _owningActor, value);
        }

        [Browsable(false)]
        public bool IsSpawned => OwningActor?.IsSpawned ?? false;

        public void Spawn(IActor owner)
        {
            OwningActor = owner;
            OnSpawned();
        }
        public void Despawn(IActor owner)
        {
            OnDespawned();
            if (OwningActor == owner)
                OwningActor = null;
        }

        /// <summary>
        /// Called when this component is spawned.
        /// This base method starts any attached animations.
        /// </summary>
        public virtual void OnSpawned()
        {
            StartAllAnimations(true);
        }
        public virtual void OnDespawned()
        {
            StopAllAnimations();
        }
    }
}
