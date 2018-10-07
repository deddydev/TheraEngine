using TheraEngine.Core.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

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
    public delegate void DelMatrixUpdate(Matrix4 worldMatrix);
    public delegate void SimulationUpdate(bool isSimulating);
    public delegate void PhysicsEndContact(ICollidable me, ICollidable other);
    public delegate void PhysicsOverlap(ICollidable me, ICollidable other, ManifoldPoint point);
    
    public class PhysicsDriver : FileObject
    {
        public event PhysicsOverlap BeginOverlap, EndOverlap, OnHit;
        public event PhysicsEndContact OnContactEnded;
        public event DelMatrixUpdate TransformChanged;
        public event SimulationUpdate SimulationStateChanged;

        public PhysicsDriver(ICollidable owner, TRigidBodyConstructionInfo info)
        {
            _owner = owner;
            _collisionEnabled = info.CollisionEnabled;
            _simulatingPhysics = info.SimulatePhysics;
            _group = info.CollisionGroup;
            _collidesWith = info.CollidesWith;
            CollisionObject = Engine.Physics.NewRigidBody(info);
        }
        public PhysicsDriver(ICollidable owner, TRigidBodyConstructionInfo info, DelMatrixUpdate func)
            : this(owner, info)
            => TransformChanged += func;
        public PhysicsDriver(ICollidable owner, TRigidBodyConstructionInfo info, DelMatrixUpdate mtxFunc, SimulationUpdate simFunc)
            : this(owner, info, mtxFunc)
            => SimulationStateChanged += simFunc;

        public PhysicsDriver(ICollidable owner, TSoftBodyConstructionInfo info)
        {
            _owner = owner;
            _collisionEnabled = info.CollisionEnabled;
            _simulatingPhysics = info.SimulatePhysics;
            _group = info.CollisionGroup;
            _collidesWith = info.CollidesWith;
            CollisionObject = Engine.Physics.NewSoftBody(info);
        }
        public PhysicsDriver(ICollidable owner, TSoftBodyConstructionInfo info, DelMatrixUpdate func)
            : this(owner, info)
            => TransformChanged += func;
        public PhysicsDriver(ICollidable owner, TSoftBodyConstructionInfo info, DelMatrixUpdate mtxFunc, SimulationUpdate simFunc)
            : this(owner, info, mtxFunc)
            => SimulationStateChanged += simFunc;

        private Vec3
            _previousLinearFactor = Vec3.One,
            _previousAngularFactor = Vec3.One;
        bool _sleepingEnabled = true;

        [TSerialize("CollisionEnabled")]
        private bool _collisionEnabled;
        [TSerialize("SimulatingPhysics")]
        private bool _simulatingPhysics;
        private bool _isSpawned;
        [TSerialize("CollisionGroup")]
        private TCollisionGroup _group;
        [TSerialize("CollidesWith")]
        private TCollisionGroup _collidesWith;

        private ICollidable _owner;
        private TRigidBody _collision;
        private ThreadSafeList<PhysicsDriver> _overlapping = new ThreadSafeList<PhysicsDriver>();

        [Browsable(false)]
        public ICollidable Owner => _owner;
        [Browsable(false)]
        public ThreadSafeList<PhysicsDriver> Overlapping => _overlapping;
        [Browsable(false)]
        [TSerialize]
        public TCollisionObject CollisionObject
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
                        Engine.World.PhysicsWorld.RemoveRigidBody(_collision);
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
                        _collision.ForceActivationState(_sleepingEnabled ? ActivationState.ActiveTag : ActivationState.DisableDeactivation);
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
        [CustomXMLDeserializeMethod("CollisionObject")]
        private void CollisionObjectDeserialize(XMLReader reader)
        {
            
        }
        [Category("Physics Driver")]
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
                        _collision.ForceActivationState(_sleepingEnabled ? ActivationState.ActiveTag : ActivationState.DisableDeactivation);
                        if (_isSpawned)
                            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
                    }
                }
                SimulationStateChanged?.Invoke(_simulatingPhysics);
            }
        }
        [Category("Physics Driver")]
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
                    _collision.BroadphaseProxy.CollisionFilterMask = (CollisionFilterGroups)(short)(_collisionEnabled ? _collidesWith : TCollisionGroup.None);
            }
        }
        [Category("Physics Driver")]
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
        [Category("Physics Driver")]
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
        [Category("Physics Driver")]
        public TCollisionGroup CollisionGroup
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
        [Category("Physics Driver")]
        public TCollisionGroup CollidesWith
        {
            get => _collidesWith;
            set
            {
                if (_collidesWith == value)
                    return;
                _collidesWith = value;
                if (_collision != null && _collision.IsInWorld)
                    _collision.BroadphaseProxy.CollisionFilterMask = (CollisionFilterGroups)(short)(_collisionEnabled ? _collidesWith : TCollisionGroup.None);
            }
        }
        [Category("Physics Driver")]
        public Vec3 LinearFactor
        {
            get => _previousLinearFactor;
            set => _previousLinearFactor = value;
        }
        [Category("Physics Driver")]
        public Vec3 AngularFactor
        {
            get => _previousAngularFactor;
            set => _previousAngularFactor = value;
        }
        [Category("Physics Driver")]
        public bool SleepingEnabled
        {
            get => _sleepingEnabled;
            set
            {
                _sleepingEnabled = value;
                if (_collision.ActivationState != ActivationState.DisableSimulation)
                {
                    if (_sleepingEnabled)
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
                Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, _collisionEnabled ? (short)_collidesWith : (short)TCollisionGroup.None);

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

        internal void InvokeHit(ICollidable other, ManifoldPoint cp)
        {
            OnHit?.Invoke(Owner, other, cp);
        }
        internal void InvokeBeginOverlap(ICollidable other, ManifoldPoint cp)
        {
            BeginOverlap?.Invoke(Owner, other, cp);
        }
        internal void InvokeEndOverlap(ICollidable other, ManifoldPoint cp)
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
