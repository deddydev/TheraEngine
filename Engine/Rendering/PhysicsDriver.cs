using BulletSharp;
using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TheraEngine.Rendering
{
    [Flags]
    public enum CustomCollisionGroup : short
    {
        All             = -1,
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
        public CustomCollisionGroup CollisionGroup = CustomCollisionGroup.Default;
        public CustomCollisionGroup CollidesWith = CustomCollisionGroup.All;

        private float _mass = 1.0f;
        public MotionState MotionState = null;
        private CollisionShape _collisionShape = null;
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

        public CollisionShape CollisionShape
        {
            get => _collisionShape;
            set
            {
                _collisionShape = value;
                if (CollisionShape != null)
                    LocalInertia = CollisionShape.CalculateLocalInertia(Mass);
            }
        }
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                if (CollisionShape != null)
                    LocalInertia = CollisionShape.CalculateLocalInertia(Mass);
            }
        }

        public RigidBodyConstructionInfo ForBullet()
        {
            return new RigidBodyConstructionInfo(Mass, MotionState, CollisionShape, LocalInertia)
            {
                AdditionalDamping = AdditionalDamping,
                AdditionalDampingFactor = AdditionalDampingFactor,
                AdditionalLinearDampingThresholdSqr = AdditionalLinearDampingThresholdSqr,
                AngularDamping = AngularDamping,
                AngularSleepingThreshold = AngularSleepingThreshold,
                Friction = Friction,
                LinearDamping = LinearDamping,
                AdditionalAngularDampingThresholdSqr = AdditionalAngularDampingThresholdSqr,
                Restitution = Restitution,
                RollingFriction = RollingFriction,
                StartWorldTransform = InitialWorldTransform,
                LinearSleepingThreshold = LinearSleepingThreshold,
                AdditionalAngularDampingFactor = AdditionalAngularDampingFactor,
            };
        }
    }
    public delegate void PhysicsEndContact(IPhysicsDrivable me, IPhysicsDrivable other);
    public delegate void PhysicsOverlap(IPhysicsDrivable me, IPhysicsDrivable other, ManifoldPoint point);
    public class PhysicsDriver : FileObject
    {
        public event PhysicsOverlap BeginOverlap, EndOverlap, OnHit;
        public event PhysicsEndContact OnContactEnded;
        public event MatrixUpdate TransformChanged;
        public event SimulationUpdate SimulationStateChanged;

        public PhysicsDriver(IPhysicsDrivable owner, PhysicsConstructionInfo info)
        {
            _owner = owner;
            _collisionEnabled = info.CollisionEnabled;
            _simulatingPhysics = info.SimulatePhysics;
            _group = info.CollisionGroup;
            _collidesWith = info.CollidesWith;
            using (var bulletInfo = info.ForBullet())
                CollisionObject = new RigidBody(bulletInfo);
        }

        public PhysicsDriver(IPhysicsDrivable owner, PhysicsConstructionInfo info, MatrixUpdate func)
            : this(owner, info)
            => TransformChanged += func;
        public PhysicsDriver(IPhysicsDrivable owner, PhysicsConstructionInfo info, MatrixUpdate mtxFunc, SimulationUpdate simFunc)
            : this(owner, info, mtxFunc)
            => SimulationStateChanged += simFunc;
        
        private Vec3
            _previousLinearFactor = Vec3.One,
            _previousAngularFactor = Vec3.One;
        bool _enableSleeping = true;

        [Serialize("CollisionEnabled")]
        private bool _collisionEnabled;
        [Serialize("SimulatingPhysics")]
        private bool _simulatingPhysics;
        private bool _isSpawned;
        [Serialize("CollisionGroup")]
        private CustomCollisionGroup _group;
        [Serialize("CollidesWith")]
        private CustomCollisionGroup _collidesWith;

        private IPhysicsDrivable _owner;
        private RigidBody _collision;
        private ThreadSafeList<PhysicsDriver> _overlapping = new ThreadSafeList<PhysicsDriver>();

        public ThreadSafeList<PhysicsDriver> Overlapping => _overlapping;
        public RigidBody CollisionObject
        {
            get => _collision;
            set
            {
                if (_collision == value)
                    return;
                bool wasInWorld = false;
                if (_collision != null)
                {
                    wasInWorld = _collision.IsInWorld;
                    if (wasInWorld && Engine.World != null)
                    {
                        Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                        if (_simulatingPhysics)
                            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
                    }
                    _collision.UserObject = null;
                }
                _collision = value;
                if (_collision != null)
                {
                    _collision.UserObject = this;

                    if (_collisionEnabled)
                        _collision.CollisionFlags &= ~CollisionFlags.NoContactResponse;
                    else
                        _collision.CollisionFlags |= CollisionFlags.NoContactResponse;

                    _collision.CollisionFlags |= CollisionFlags.CustomMaterialCallback;

                    if (!_simulatingPhysics)
                    {
                        _collision.LinearFactor = new Vector3(0.0f);
                        _collision.AngularFactor = new Vector3(0.0f);
                        _collision.ForceActivationState(ActivationState.DisableSimulation);
                    }
                    else
                    {
                        _collision.LinearFactor = _previousLinearFactor;
                        _collision.AngularFactor = _previousAngularFactor;
                        _collision.ForceActivationState(_enableSleeping ? ActivationState.ActiveTag : ActivationState.DisableDeactivation);
                    }

                    if (wasInWorld && _isSpawned && Engine.World != null)
                    {
                        Engine.World.PhysicsScene.AddRigidBody(_collision, (short)CollisionGroup, (short)CollidesWith);
                        if (_simulatingPhysics)
                            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
                    }
                }
            }
        }
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
                        if (_isSpawned)
                            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
                    }
                    else
                    {
                        _collision.LinearFactor = _previousLinearFactor;
                        _collision.AngularFactor = _previousAngularFactor;
                        SetPhysicsTransform(_owner.WorldMatrix);
                        _collision.ForceActivationState(_enableSleeping ? ActivationState.ActiveTag : ActivationState.DisableDeactivation);
                        if (_isSpawned)
                            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
                    }
                }
                SimulationStateChanged?.Invoke(_simulatingPhysics);
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
        public bool Kinematic
        {
            get => _collision == null ? false : _collision.CollisionFlags.HasFlag(CollisionFlags.KinematicObject);
            set
            {
                if (_collision == null)
                    return;

                if (value)
                    _collision.CollisionFlags |= CollisionFlags.KinematicObject;
                else
                    _collision.CollisionFlags &= ~CollisionFlags.KinematicObject;
            }
        }
        public bool Static
        {
            get => _collision == null ? false : _collision.CollisionFlags.HasFlag(CollisionFlags.StaticObject);
            set
            {
                if (_collision == null)
                    return;

                if (value)
                    _collision.CollisionFlags |= CollisionFlags.StaticObject;
                else
                    _collision.CollisionFlags &= ~CollisionFlags.StaticObject;
            }
        }
        public CustomCollisionGroup CollisionGroup
        {
            get => _group;
            set
            {
                if (_group == value)
                    return;
                _group = value;
                if (_collision != null && _collision.IsInWorld)
                    _collision.BroadphaseProxy.CollisionFilterGroup = (CollisionFilterGroups)(short)_group;
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
                    _collision.BroadphaseProxy.CollisionFilterMask = (CollisionFilterGroups)(short)(_collisionEnabled ? _collidesWith : CustomCollisionGroup.None);
            }
        }
        public IPhysicsDrivable Owner => _owner;
        public Vec3 LinearFactor
        {
            get => _previousLinearFactor;
            set => _previousLinearFactor = value;
        }
        public Vec3 AngularFactor
        {
            get => _previousAngularFactor;
            set => _previousAngularFactor = value;
        }
        public bool EnableSleeping
        {
            get => _enableSleeping;
            set
            {
                _enableSleeping = value;
                if (_collision.ActivationState != ActivationState.DisableSimulation)
                {
                    if (_enableSleeping)
                    {
                        if (_collision.ActivationState == ActivationState.DisableDeactivation)
                        {
                            _collision.ActivationState = ActivationState.ActiveTag;
                        }
                    }
                    else
                    {
                        _collision.ActivationState = ActivationState.DisableDeactivation;
                    }
                }
            }
        }

        public void OnSpawned()
        {
            _isSpawned = true;
            if (_collision == null)
                return;

            //if (Engine.World != null)
                Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);

            if (_simulatingPhysics)
                RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
        }
        public void OnDespawned()
        {
            _isSpawned = false;
            if (_collision == null)
                return;

            if (_collision.IsInWorld/* && Engine.World != null*/)
                Engine.World.PhysicsScene.RemoveRigidBody(_collision);

            if (_simulatingPhysics)
                UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
        }
        //internal void AddToWorld()
        //{
        //    Engine.World.PhysicsScene.AddRigidBody(
        //        _collision, 
        //        (short)_group,
        //        _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);
        //}
        internal virtual void SetPhysicsTransform(Matrix4 newTransform)
        {
            _collision.ProceedToTransform(newTransform);
            //Vector3 vel = _collision.LinearVelocity;
            //_collision.LinearVelocity = new Vector3(0.0f);
            //_collision.WorldTransform = newTransform;
            //_collision.CenterOfMassTransform = newTransform;
            //_collision.MotionState.WorldTransform = newTransform;
            //_collision.ClearForces();
            //_collision.LinearVelocity = vel;
        }
        protected internal /*async*/ void Tick(float delta)
        {
            _collision.GetWorldTransform(out Matrix transform);
            /*await Task.Run(() => */TransformChanged?.Invoke(transform)/*)*/;
        }

        internal void InvokeHit(IPhysicsDrivable other, ManifoldPoint cp)
        {
            OnHit?.Invoke(Owner, other, cp);
        }
        internal void InvokeBeginOverlap(IPhysicsDrivable other, ManifoldPoint cp)
        {
            BeginOverlap?.Invoke(Owner, other, cp);
        }
        internal void InvokeEndOverlap(IPhysicsDrivable other, ManifoldPoint cp)
        {
            EndOverlap?.Invoke(Owner, other, cp);
        }

        internal void ContactStarted(PhysicsDriver other, ManifoldPoint cp)
        {
            bool thisCollides = (CollidesWith & other.CollisionGroup) == other.CollisionGroup;
            bool thatCollides = (other.CollidesWith & CollisionGroup) == CollisionGroup;
            if (thisCollides || thatCollides)
            {
                InvokeHit(other.Owner, cp);
                other.InvokeHit(Owner, cp);
            }
            else
            {
                _overlapping.Add(other);
                other._overlapping.Add(this);

                InvokeBeginOverlap(other.Owner, cp);
                other.InvokeBeginOverlap(Owner, cp);
            }
        }

        internal void ContactEnded(PhysicsDriver other)
        {
            OnContactEnded?.Invoke(Owner, other.Owner);
            _overlapping.Remove(other);
            other._overlapping.Remove(this);
        }
    }
}
