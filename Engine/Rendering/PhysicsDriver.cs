using BulletSharp;
using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;
using BulletSharp.SoftBody;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace CustomEngine.Rendering
{
    [Flags]
    public enum CustomCollisionGroup : ushort
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
    public delegate void MatrixUpdate(Matrix4 worldMatrix);
    public delegate void SimulationUpdate(bool isSimulating);
    public interface IPhysicsDrivable
    {
        PhysicsDriver PhysicsDriver { get; }
        Matrix4 WorldMatrix { get; }
    }
    public class PhysicsConstructionInfo
    {
        public bool CollisionEnabled = true;
        public bool SimulatePhysics = true;
        public CustomCollisionGroup Group = CustomCollisionGroup.Default;
        public CustomCollisionGroup CollidesWith = CustomCollisionGroup.All;
        
        public float Mass = 1.0f;
        public MotionState MotionState = null;
        public CollisionShape CollisionShape = null;
        public Vec3 LocalInertia = Vec3.Zero;

        public bool AdditionalDamping = false;
        public float AdditionalDampingFactor = 0.005f;
        public float AdditionalLinearDampingThresholdSqr = 0.01f;
        public float AngularDamping = 0.0f;
        public float AngularSleepingThreshold = 1.0f;
        public float Friction = 0.5f;
        public float LinearDamping = 0.0f;
        public float AdditionalAngularDampingThresholdSqr = 0.01f;
        public float Restitution = 0.0f;
        public float RollingFriction = 0.0f;
        public Matrix4 InitialWorldTransform = Matrix4.Identity;
        public float LinearSleepingThreshold = 0.8f;
        public float AdditionalAngularDampingFactor = 0.01f;
    }
    public delegate void PhysicsOverlap(IPhysicsDrivable other, ManifoldPoint point);
    public class PhysicsDriver : FileObject
    {
        public PhysicsOverlap BeginOverlap, EndOverlap, OnHit;
        public event MatrixUpdate TransformChanged;
        public event SimulationUpdate SimulationStateChanged;

        public PhysicsDriver(IPhysicsDrivable owner, PhysicsConstructionInfo info)
        {
            _owner = owner;
            _collisionEnabled = info.CollisionEnabled;
            _simulatingPhysics = info.SimulatePhysics;
            _group = info.Group;
            _collidesWith = info.CollidesWith;

            RigidBodyConstructionInfo bodyInfo = new RigidBodyConstructionInfo(info.Mass, info.MotionState, info.CollisionShape, info.LocalInertia)
            {
                AdditionalDamping = info.AdditionalDamping,
                AdditionalDampingFactor = info.AdditionalDampingFactor,
                AdditionalLinearDampingThresholdSqr = info.AdditionalLinearDampingThresholdSqr,
                AngularDamping = info.AngularDamping,
                AngularSleepingThreshold = info.AngularSleepingThreshold,
                Friction = info.Friction,
                LinearDamping = info.LinearDamping,
                AdditionalAngularDampingThresholdSqr = info.AdditionalAngularDampingThresholdSqr,
                Restitution = info.Restitution,
                RollingFriction = info.RollingFriction,
                StartWorldTransform = info.InitialWorldTransform,
                LinearSleepingThreshold = info.LinearSleepingThreshold,
                AdditionalAngularDampingFactor = info.AdditionalAngularDampingFactor,
            };
            UpdateBody(new RigidBody(bodyInfo));
        }

        public PhysicsDriver(IPhysicsDrivable owner, PhysicsConstructionInfo info, MatrixUpdate func)
            : this(owner, info)
            => TransformChanged += func;
        public PhysicsDriver(IPhysicsDrivable owner, PhysicsConstructionInfo info, MatrixUpdate mtxFunc, SimulationUpdate simFunc)
            : this(owner, info, mtxFunc)
            => SimulationStateChanged += simFunc;
        
        IPhysicsDrivable _owner;
        private Vec3 _linearFactor = Vec3.One, _angularFactor = Vec3.One;
        private bool _collisionEnabled, _simulatingPhysics;
        private CustomCollisionGroup _group, _collidesWith;
        public Vec3 _prevVelocity, _velocity, _acceleration;
        public Matrix4 _prevWorldMatrix;
        private RigidBody _collision;
        
        public Vec3 PreviousPosition => _prevWorldMatrix.GetPoint();
        public Vec3 Position => _owner.WorldMatrix.GetPoint();
        public Vec3 PreviousVelocity => _prevVelocity;
        public Vec3 Velocity
        {
            get => _velocity;
            set
            {
                _prevVelocity = _velocity;
                _collision.LinearVelocity = _velocity = value;
            }
        }
        /// <summary>
        /// Returns the instantaneous speed of this object right now.
        /// Uses a fast approximation to avoid using a slow square root operation.
        /// </summary>
        public float GetSpeedFast()
            => Velocity.LengthFast;
        /// <summary>
        /// Returns the instantaneous speed of this object right now.
        /// Uses square root for exact value; slower than GetSpeedFast.
        /// </summary>
        public float GetSpeed()
            => Velocity.Length;
        /// <summary>
        /// Returns the instantaneous speed of this object on the last tick.
        /// Uses a fast approximation to avoid using a slow square root operation.
        /// </summary>
        public float GetPrevSpeedFast()
            => PreviousVelocity.LengthFast;
        /// <summary>
        /// Returns the instantaneous speed of this object on the last tick.
        /// Uses square root for exact value; slower than GetPrevSpeedFast.
        /// </summary>
        public float GetPrevSpeed()
            => PreviousVelocity.Length;
        /// <summary>
        /// Returns the acceleration of this object from the last tick to the current one.
        /// </summary>
        public Vec3 GetAcceleration()
            => _acceleration;

        public bool SimulatingPhysics
        {
            get => _simulatingPhysics;
            set
            {
                if (_simulatingPhysics == value)
                    return;
                _simulatingPhysics = value;
                if (_collision != null)
                {
                    if (!_simulatingPhysics)
                    {
                        _collision.LinearFactor = new Vector3(0.0f);
                        _collision.AngularFactor = new Vector3(0.0f);
                        _collision.ForceActivationState(ActivationState.DisableSimulation);
                        SimulationStateChanged?.Invoke(false);
                        UnregisterTick();
                    }
                    else
                    {
                        _collision.LinearFactor = _linearFactor;
                        _collision.AngularFactor = _angularFactor;
                        SetPhysicsTransform(_owner.WorldMatrix);
                        _collision.ForceActivationState(ActivationState.ActiveTag);
                        SimulationStateChanged?.Invoke(true);
                        RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
                    }
                }
            }
        }
        public bool CollisionEnabled
        {
            get => _collisionEnabled;
            set
            {
                if (_collisionEnabled == value)
                    return;
                _collisionEnabled = value;

                if (_collisionEnabled)
                    _collision.CollisionFlags &= ~CollisionFlags.NoContactResponse;
                else
                    _collision.CollisionFlags |= CollisionFlags.NoContactResponse;

                if (_collision != null && _collision.IsInWorld)
                    _collision.BroadphaseProxy.CollisionFilterMask = (CollisionFilterGroups)(short)(_collisionEnabled ? _collidesWith : CustomCollisionGroup.None);
            }
        }
        public RigidBody CollisionObject => _collision;
        public CustomCollisionGroup CollisionGroup
        {
            get => _group;
            set
            {
                if (_group == value)
                    return;
                _group = value;
                if (_collision != null && _collision.IsInWorld)
                {
                    _collision.BroadphaseProxy.CollisionFilterGroup = (CollisionFilterGroups)(short)_group;
                    //Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                    //Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, (short)(_collisionEnabled ? _collidesWith : CustomCollisionGroup.None));
                }
            }
        }
        public CustomCollisionGroup CollidesWith
        {
            get => _collidesWith;
            set
            {
                if (_collidesWith == value)
                    return;
                _collidesWith = value;
                if (_collision != null && _collision.IsInWorld)
                {
                    _collision.BroadphaseProxy.CollisionFilterMask = (CollisionFilterGroups)(short)(_collisionEnabled ? _collidesWith : CustomCollisionGroup.None);
                    //Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                    //Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, (short)(_collisionEnabled ? _collidesWith : CustomCollisionGroup.None));
                }
            }
        }
        public IPhysicsDrivable Owner
        {
            get => _owner;
            set => _owner = value;
        }
        public Vec3 LinearFactor
        {
            get => _linearFactor;
            set => _linearFactor = value;
        }
        public Vec3 AngularFactor
        {
            get => _angularFactor;
            set => _angularFactor = value;
        }
        public void OnSpawned()
        {
            //if (Engine.World == null)
            //    Engine.QueueCollisionSpawn(this);
            //else
                Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);

            if (_simulatingPhysics)
                RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
        }
        public void OnDespawned()
        {

        }
        public void UpdateBody(RigidBody body)
        {
            if (_collision == body)
                return;
            if (_collision != null)
            {
                if (_collision.IsInWorld && Engine.World != null)
                    Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                _collision.UserObject = null;
            }
            _collision = body;
            if (_collision != null)
            {
                if (_collisionEnabled)
                    _collision.CollisionFlags &= ~CollisionFlags.NoContactResponse;
                else
                    _collision.CollisionFlags |= CollisionFlags.NoContactResponse;

                _collision.CollisionFlags |= CollisionFlags.CustomMaterialCallback;

                _collision.UserObject = this;
                if (!_simulatingPhysics)
                {
                    _collision.LinearFactor = new Vector3(0.0f);
                    _collision.AngularFactor = new Vector3(0.0f);
                    //The body is not simulating but can be moved by the user and bullet will calculate velocity
                    //_collision.CollisionFlags |= CollisionFlags.KinematicObject;
                    //_collision.CollisionFlags |= CollisionFlags.StaticObject;
                    _collision.ForceActivationState(ActivationState.DisableSimulation);
                    UnregisterTick();
                }
                else
                {
                    _collision.LinearFactor = _linearFactor;
                    _collision.AngularFactor = _angularFactor;
                    //_collision.CollisionFlags &= ~CollisionFlags.KinematicObject;
                    //_collision.CollisionFlags &= ~CollisionFlags.StaticObject;
                    //_collision.ForceActivationState(ActivationState.IslandSleeping);
                    RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
                }
            }
        }
        internal void AddToWorld()
        {
            Engine.World.PhysicsScene.AddRigidBody(
                _collision, 
                (short)_group,
                _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);
        }
        internal virtual void SetPhysicsTransform(Matrix4 newTransform)
        {
            _collision.WorldTransform = newTransform;
            Engine.World?.PhysicsScene.UpdateAabbs();
        }
        protected internal override void Tick(float delta)
        {
            _collision.GetWorldTransform(out Matrix transform);
            //_prevWorldMatrix = _worldMatrix;
            //_prevVelocity = _velocity;
            //_velocity = (Position - PreviousPosition) / delta;
            //_acceleration = (_velocity - _prevVelocity) / delta;
            TransformChanged?.Invoke(transform);
        }
    }
}
