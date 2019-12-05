using Extensions;
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
    [TFileExt("comp")]
    public abstract class Component : TFileObject, IComponent
    {
        /// <summary>
        /// Determines if this component was constructed by code and cannot be removed.
        /// </summary>
        [Browsable(false)]
        public bool Locked { get; } = true;

        [Browsable(false)]
        public virtual IActor OwningActor { get; set; }

        [Browsable(false)]
        public bool IsSpawned => OwningActor?.IsSpawned ?? false;

        /// <summary>
        /// Called when this component is spawned.
        /// This base method starts any attached animations.
        /// </summary>
        public virtual void OnSpawned()
        {
            Animations?.ForEach(anim =>
            {
                if (anim.BeginOnSpawn)
                    anim.Start();
            });
        }
        public virtual void OnDespawned() 
        {
            Animations?.ForEach(anim =>
            {
                if (anim.State != Animation.EAnimationState.Stopped)
                    anim.Start();
            });
        }
    }
}
