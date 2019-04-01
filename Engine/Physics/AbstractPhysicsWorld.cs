using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.ContactTesting;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsWorld : IDisposable
    {
        public virtual Vec3 Gravity { get; set; } = new Vec3(0.0f, -9.81f, 0.0f);
        public bool AllowIndividualAabbUpdates { get; set; } = true;

        public abstract bool DrawConstraints { get; set; }
        public abstract bool DrawConstraintLimits { get; set; }
        public abstract bool DrawCollisionAABBs { get; set; }

        /// <summary>
        /// Renders all debug objects. Only to be called in the render pass.
        /// </summary>
        public abstract void DrawDebugWorld();
        /// <summary>
        /// Moves the physics simulation forward by the specified amount of seconds.
        /// </summary>
        /// <param name="delta">How many seconds to add.</param>
        public abstract void StepSimulation(float delta);
        /// <summary>
        /// Adds a collision object to the physics world.
        /// </summary>
        /// <param name="collision">The collision object to add.</param>
        public abstract void AddCollisionObject(TCollisionObject collision);
        /// <summary>
        /// Removes a collision object from the physics world.
        /// </summary>
        /// <param name="collision">The collision object to remove.</param>
        public abstract void RemoveCollisionObject(TCollisionObject collision);
        /// <summary>
        /// Add a simulation constraint to the physics world.
        /// </summary>
        /// <param name="constraint">The constraint to add.</param>
        public abstract void AddConstraint(TConstraint constraint);
        /// <summary>
        /// Removes a simulation constraint from the physics world.
        /// </summary>
        /// <param name="constraint">The constraint to remove.</param>
        public abstract void RemoveConstraint(TConstraint constraint);
        /// <summary>
        /// Shoots a ray using a start and end point and determines collisions with the physics world.
        /// </summary>
        /// <param name="result">Contains parameters and also the results of the trace once this method returns.</param>
        /// <returns>True if any hits occurred.</returns>
        public abstract bool RayTrace(RayTrace result);
        /// <summary>
        /// Moves a shape through the physics world using a start and end transformations and determines collisions.
        /// </summary>
        /// <param name="result">Contains parameters and also the results of the trace once this method returns.</param>
        /// <returns>True if any hits occurred.</returns>
        public abstract bool ShapeTrace(ShapeTrace result);
        public abstract bool ContactTest(ContactTest result);
        /// <summary>
        /// Recalculates the AABBs of all collision objects in the physics world.
        /// </summary>
        public abstract void UpdateAabbs();
        /// <summary>
        /// Recalculates a specific collision object's AABB.
        /// </summary>
        /// <param name="collision">The object whose AABB to recalculate.</param>
        public void UpdateSingleAabb(TCollisionObject collision)
        {
            if (AllowIndividualAabbUpdates)
                OnUpdateSingleAabb(collision);
        }

        protected abstract void OnUpdateSingleAabb(TCollisionObject collision);
        public abstract void Dispose();
    }
}
