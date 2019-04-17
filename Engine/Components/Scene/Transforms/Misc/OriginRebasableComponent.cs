using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Transforms
{
    public interface IOriginRebasableComponent : ISceneComponent
    {
        event Action<Vec3> OriginRebased;
        void RebaseOrigin(Vec3 newOrigin);
    }
    /// <summary>
    /// Base class for actor root components. 
    /// Ensures that the actor's root transform can be rebased to a different location.
    /// </summary>
    public abstract class OriginRebasableComponent : SceneComponent
    {
        public event Action<Vec3> OriginRebased;

        public OriginRebasableComponent() : base() { }

        /// <summary>
        /// This is to handle translating the root component of an actor when the world's origin is changed.
        /// </summary>
        /// <param name="newOrigin">The translation of the new origin relative to the current origin. 
        /// Subtract this value from a world translation to correct it.</param>
        public void RebaseOrigin(Vec3 newOrigin)
        {
            OnOriginRebased(newOrigin);
            OriginRebased?.Invoke(newOrigin);
        }
        /// <summary>
        /// This is to handle translating the root component of an actor when the world's origin is changed.
        /// </summary>
        /// <param name="newOrigin">The translation of the new origin relative to the current origin. 
        /// Subtract this value from a world translation to correct it.</param>
        protected internal abstract void OnOriginRebased(Vec3 newOrigin);
    }
}
