using BulletSharp;
using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using BulletSharp.SoftBody;
using System.IO;
using System.Xml;

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
    }
    public class PhysicsDriverInfo
    {
        public bool CollisionEnabled;
        public bool SimulatePhysics;
        public CustomCollisionGroup Group;
        public CustomCollisionGroup CollidesWith;
        public RigidBodyConstructionInfo BodyInfo;
    }
    public delegate void PhysicsOverlap(IPhysicsDrivable other, ManifoldPoint point);
    public class PhysicsDriver : FileObject
    {
        public PhysicsOverlap BeginOverlap, EndOverlap, OnHit;

        IPhysicsDrivable _owner;

        public PhysicsDriver(IPhysicsDrivable owner, PhysicsDriverInfo info)
        {
            _owner = owner;
            _collisionEnabled = info.CollisionEnabled;
            _simulatingPhysics = info.SimulatePhysics;
            _group = info.Group;
            _collidesWith = info.CollidesWith;
            UpdateBody(new RigidBody(info.BodyInfo));
        }

        public PhysicsDriver(IPhysicsDrivable owner, PhysicsDriverInfo info, MatrixUpdate func)
            : this(owner, info)
            => TransformChanged += func;
        public PhysicsDriver(IPhysicsDrivable owner, PhysicsDriverInfo info, MatrixUpdate mtxFunc, SimulationUpdate simFunc)
            : this(owner, info, mtxFunc)
            => SimulationStateChanged += simFunc;
        
        public event MatrixUpdate TransformChanged;
        public event SimulationUpdate SimulationStateChanged;

        private bool _collisionEnabled, _simulatingPhysics;
        private CustomCollisionGroup _group, _collidesWith;
        public Vec3 _prevVelocity, _velocity, _acceleration;
        public Matrix4 _prevWorldMatrix, _worldMatrix;
        private RigidBody _collision;
        
        public Vec3 PreviousPosition => _prevWorldMatrix.GetPoint();
        public Vec3 Position => _worldMatrix.GetPoint();
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
        
        public Matrix4 WorldTransform
        {
            get => _worldMatrix;
            set => SetPhysicsTransform(_worldMatrix);
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
                        SimulationStateChanged?.Invoke(false);
                        UnregisterTick();
                    }
                    else
                    {
                        _collision.LinearFactor = new Vector3(1.0f);
                        _collision.AngularFactor = new Vector3(1.0f);
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
                if (Engine.World == null)
                    Engine.QueueCollisionSpawn(this);
                else
                    Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);

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
                    //_collision.CollisionFlags |= CollisionFlags.StaticObject;
                    //_collision.CollisionFlags &= ~CollisionFlags.KinematicObject;
                    _collision.ForceActivationState(ActivationState.DisableSimulation);
                    UnregisterTick();
                }
                else
                {
                    _collision.LinearFactor = new Vector3(1.0f);
                    _collision.AngularFactor = new Vector3(1.0f);
                    //_collision.CollisionFlags |= CollisionFlags.KinematicObject;
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
            _worldMatrix = newTransform;
            _collision.WorldTransform = _worldMatrix;
            _collision.InterpolationWorldTransform = _worldMatrix;
            Engine.World.PhysicsScene.UpdateAabbs();
        }
        internal override void Tick(float delta)
        {
            _prevVelocity = _velocity;
            _velocity = _collision.LinearVelocity;
            _acceleration = (_velocity - _prevVelocity) / delta;

            _collision.GetWorldTransform(out Matrix transform);
            _worldMatrix = transform;
            TransformChanged(_worldMatrix);
        }
    }
}
