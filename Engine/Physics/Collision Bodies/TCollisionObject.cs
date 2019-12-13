using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;

namespace TheraEngine.Physics
{
    [Flags]
    public enum ETheraCollisionGroup : ushort
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
        Camera          = 0x0200,
        Volumes         = 0x0400,
        Foliage         = 0x0800,
        Aux1            = 0x1000,
        Aux2            = 0x2000,
        Aux3            = 0x4000,
        Aux4            = 0x8000,
    }
    public interface ICollidable
    {
        Matrix4 CollidableWorldMatrix { get; set; }
    }
    public interface IGenericCollidable : ICollidable
    {
        TCollisionObject CollisionObject { get; }
    }
    public interface IGhostCollidable : ICollidable
    {
        //TGhostCollisionObject GhostCollision { get; }
    }
    public interface IRigidBodyCollidable : ICollidable
    {
        TRigidBody RigidBodyCollision { get; }
    }
    public interface ISoftBodyCollidable : ICollidable
    {
        TSoftBody SoftBodyCollision { get; }
    }
    public delegate void DelMatrixUpdate(Matrix4 transform);
    public delegate void DelCollision(TCollisionObject @this, TCollisionObject other, TContactInfo info, bool thisIsA);

    [TFileExt("coll")]
    [TFileDef("Collision Object", "Defines a collision object used by the physics engine for collision simulation.")]
    public abstract class TCollisionObject : TFileObject, IDisposable
    {
        public event DelMatrixUpdate TransformChanged;
        protected internal void OnTransformChanged(Matrix4 worldTransform)
            => TransformChanged?.Invoke(worldTransform);
        
        public event DelCollision Collided, Overlapped;
        protected internal void OnCollided(TCollisionObject other, TContactInfo info, bool thisIsA)
            => Collided?.Invoke(this, other, info, thisIsA);
        protected internal void OnOverlapped(TCollisionObject other, TContactInfo info, bool thisIsA)
            => Overlapped?.Invoke(this, other, info, thisIsA);
        
        protected TCollisionObject() { }

        public ICollidable Owner { get; internal set; }

        //[PhysicsSupport(PhysicsLibrary.Bullet)]
        //public abstract int UniqueID { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract int IslandTag { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool IsActive { get; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Matrix4 WorldTransform { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Matrix4 InterpolationWorldTransform { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 InterpolationLinearVelocity { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 InterpolationAngularVelocity { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float HitFraction { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float Friction { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float DeactivationTime { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float ContactProcessingThreshold { get; set; }
        
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public virtual TCollisionShape CollisionShape { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool HasContactResponse { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool IsStatic { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool IsKinematic { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool CustomMaterialCallback { get; set; }
        
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float CcdSweptSphereRadius { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float CcdSquareMotionThreshold { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float CcdMotionThreshold { get; set; }
        
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AnisotropicFriction { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract EBodyActivationState ActivationState { get; set; }
        
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool MergesSimulationIslands { get; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float RollingFriction { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float Restitution { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public virtual ushort CollidesWith { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public virtual ushort CollisionGroup { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AabbMin { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AabbMax { get; set; }

        public void Spawn(IWorld world)
            => world.PhysicsWorld3D.AddCollisionObject(this);
        public void Despawn(IWorld world)
            => world.PhysicsWorld3D.RemoveCollisionObject(this);

        protected ushort _previousCollidesWith = 0xFFFF;
        private bool _collisionEnabled;
        public bool CollisionEnabled
        {
            get => _collisionEnabled;
            set
            {
                HasContactResponse = _collisionEnabled = value;

                //if (_collisionEnabled)
                //    CollidesWith = _previousCollidesWith;
                //else
                //{
                //    _previousCollidesWith = CollidesWith;
                //    CollidesWith = 0;
                //}
            }
        }

        private bool _sleepingEnabled;
        public bool SleepingEnabled
        {
            get => _sleepingEnabled;
            set
            {
                _sleepingEnabled = value;
                if (_sleepingEnabled)
                {
                    if (ActivationState == EBodyActivationState.DisableSleep)
                        ActivationState = EBodyActivationState.WantsSleep;
                }
                else
                {
                    if (ActivationState != EBodyActivationState.DisableSimulation)
                        ActivationState = EBodyActivationState.DisableSleep;
                }
            }
        }

        private bool _simulatingPhysics = false;
        public bool SimulatingPhysics
        {
            get => _simulatingPhysics;
            set
            {
                _simulatingPhysics = value;
                if (!_simulatingPhysics)
                    StopSimulation();
                else
                    StartSimulation();
            }
        }

        protected virtual void StopSimulation()
        {
            IsStatic = true;
            ActivationState = EBodyActivationState.DisableSimulation;
        }
        protected virtual void StartSimulation()
        {
            IsStatic = false;
            WorldTransform = Owner?.CollidableWorldMatrix ?? Matrix4.Identity;

            if (_sleepingEnabled)
            {
                if (ActivationState == EBodyActivationState.DisableSleep)
                    ActivationState = EBodyActivationState.Active;
            }
            else
            {
                if (ActivationState != EBodyActivationState.DisableSimulation)
                    ActivationState = EBodyActivationState.DisableSleep;
            }
        }

        public abstract void Activate();
        public abstract void Activate(bool forceActivation);
        public abstract bool CheckCollideWith(TCollisionObject collisionObject);
        public abstract void ForceActivationState(EBodyActivationState newState);
        public abstract void GetWorldTransform(out Matrix4 transform);

        [Flags]
        public enum EAnisotropicFrictionFlags
        {
            Disabled    = 0b00,
            Linear      = 0b01,
            Rolling     = 0b10,
        }

        public abstract bool HasAnisotropicFriction(EAnisotropicFrictionFlags frictionMode);
        public abstract bool HasAnisotropicFriction();
        public abstract void SetAnisotropicFriction(Vec3 anisotropicFriction);
        public abstract void SetAnisotropicFriction(Vec3 anisotropicFriction, EAnisotropicFrictionFlags frictionMode);
        public abstract void SetIgnoreCollisionCheck(TCollisionObject collisionObject, bool ignoreCollisionCheck);

        public virtual void Dispose()
        {
            CollisionShape?.Dispose();
        }
    }
}
