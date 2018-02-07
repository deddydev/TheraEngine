using System;
using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Physics
{
    [Flags]
    public enum TCollisionGroup : ushort
    {
        All             = 0xFFFF,
        None            = 0x0000,
        Default         = 0x0001,
        Pawns           = 0x0002,
        Characters      = 0x0004,
        Vehicles        = 0x0008,
        StaticWorld     = 0x0010,
        DynamicWorld    = 0x0020,
        PhysicsObjects  = 0x0040,
        Interactables   = 0x0080,
        Projectiles     = 0x0100,
    }
    public interface ICollidable
    {
        Matrix4 WorldMatrix { get; set; }
    }
    public interface IRigidCollidable : ICollidable
    {
        TRigidBody RigidBodyCollision { get; }
    }
    public interface ISoftCollidable : ICollidable
    {
        TSoftBody SoftBodyCollision { get; }
    }
    public delegate void DelMatrixUpdate(Matrix4 transform);
    public delegate void DelCollision(TCollisionObject me, TCollisionObject other, TCollisionInfo info);
    [FileExt("coll")]
    [FileDef("Collision Object", "Defines a collision object used by the physics engine for collision simulation.")]
    public abstract class TCollisionObject : TFileObject
    {
        public event DelMatrixUpdate TransformChanged;
        protected internal void OnTransformChanged(Matrix4 worldTransform)
        {
            TransformChanged?.Invoke(worldTransform);
        }
        public event DelCollision Collision;
        protected internal void OnCollided(TCollisionObject other, TCollisionInfo info)
        {
            Collision?.Invoke(this, other, info);
        }

        protected TCollisionObject(ICollidable owner, TCollisionShape shape)
        {
            Owner = owner;
        }

        public ICollidable Owner { get; set; }

        //[PhysicsSupport(PhysicsLibrary.Bullet)]
        //public abstract int UniqueID { get; }
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
        public virtual ushort CollidesWith { get; set; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public virtual ushort CollisionGroup { get; set; }

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

        public abstract void Activate();
        public abstract void Activate(bool forceActivation);
        public abstract bool CheckCollideWith(TCollisionObject collisionObject);
        public abstract void ForceActivationState(EBodyActivationState newState);
        public abstract void GetWorldTransform(out Matrix4 transform);
        [Flags]
        public enum EAnisotropicFrictionFlags
        {
            Disabled = 0,
            Linear = 1,
            Rolling = 2
        }
        public abstract bool HasAnisotropicFriction(EAnisotropicFrictionFlags frictionMode);
        public abstract bool HasAnisotropicFriction();
        public abstract void SetAnisotropicFriction(Vec3 anisotropicFriction);
        public abstract void SetAnisotropicFriction(Vec3 anisotropicFriction, EAnisotropicFrictionFlags frictionMode);
        public abstract void SetIgnoreCollisionCheck(TCollisionObject collisionObject, bool ignoreCollisionCheck);
    }
}
