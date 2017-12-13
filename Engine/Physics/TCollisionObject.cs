using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics
{
    public interface ICollidable
    {
        TCollisionObject CollisionObject { get; }
        Matrix4 WorldMatrix { get; }
    }
    public abstract class TCollisionObject : TObject
    {
        public event MatrixUpdate TransformChanged;
        protected internal void OnTransformChanged(Matrix4 worldTransform)
        {
            TransformChanged?.Invoke(worldTransform);
        }

        public TCollisionObject(TCollisionShape shape)
        {
            CollisionShape = shape;
        }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract int IslandTag { get; set; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool IsActive { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Matrix4 WorldTransform { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Matrix4 InterpolationWorldTransform { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 InterpolationLinearVelocity { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 InterpolationAngularVelocity { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float HitFraction { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float Friction { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float DeactivationTime { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float ContactProcessingThreshold { get; set; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public virtual TCollisionShape CollisionShape { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool HasContactResponse { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool IsStatic { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool IsKinematic { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool CustomMaterialCallback { get; set; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float CcdSweptSphereRadius { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float CcdSquareMotionThreshold { get; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float CcdMotionThreshold { get; set; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 AnisotropicFriction { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract EBodyActivationState ActivationState { get; set; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool MergesSimulationIslands { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float RollingFriction { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float Restitution { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract ushort CollidesWith { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract ushort CollisionGroup { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 AabbMin { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 AabbMax { get; set; }

        public void Spawn()
        {
            Engine.World.PhysicsWorld.AddCollisionObject(this);
        }
        public void Despawn()
        {
            Engine.World.PhysicsWorld.RemoveCollisionObject(this);
        }
    }
}
