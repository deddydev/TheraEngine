using BulletSharp;
using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering
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
    public class PhysicsDriver : FileObject
    {
        public PhysicsDriver(PhysicsDriverInfo info)
        {
            _collisionEnabled = info.CollisionEnabled;
            _simulatingPhysics = info.SimulatePhysics;
            _group = info.Group;
            _collidesWith = info.CollidesWith;
            UpdateBody(new RigidBody(info.BodyInfo));
        }
        public PhysicsDriver(PhysicsDriverInfo info, MatrixUpdate func) : this(info)
        {
            TransformChanged += func;
        }

        public event MatrixUpdate TransformChanged;

        private bool _collisionEnabled, _simulatingPhysics;
        private CustomCollisionGroup _group, _collidesWith;
        public Matrix4 _worldMatrix;
        private RigidBody _collision;

        public Matrix4 Transform
        {
            get { return _worldMatrix; }
            set { _worldMatrix = value; SetPhysicsTransform(_worldMatrix); }
        }

        public bool SimulatingPhysics
        {
            get { return _simulatingPhysics; }
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
                    }
                    else
                    {
                        _collision.LinearFactor = new Vector3(1.0f);
                        _collision.AngularFactor = new Vector3(1.0f);
                        _collision.ForceActivationState(ActivationState.IslandSleeping);
                    }
                }
            }
        }
        public bool CollisionEnabled
        {
            get { return _collisionEnabled; }
            set
            {
                if (_collisionEnabled == value)
                    return;
                _collisionEnabled = value;
                if (_collision != null && _collision.IsInWorld)
                {
                    Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                    Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, (short)(_collisionEnabled ? _collidesWith : CustomCollisionGroup.None));
                }
            }
        }
        public RigidBody CollisionObject { get { return _collision; } }
        public CustomCollisionGroup CollisionGroup
        {
            get { return _group; }
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
            get { return _collidesWith; }
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
                {
                    if (Engine.World == null)
                        Engine.QueueCollisionSpawn(this);
                    else
                        Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);
                }
                _collision.UserObject = this;
                if (!_simulatingPhysics)
                {
                    _collision.LinearFactor = new Vector3(0.0f);
                    _collision.AngularFactor = new Vector3(0.0f);
                    _collision.ForceActivationState(ActivationState.DisableSimulation);
                }
                else
                {
                    _collision.LinearFactor = new Vector3(1.0f);
                    _collision.AngularFactor = new Vector3(1.0f);
                    //_collision.ForceActivationState(ActivationState.IslandSleeping);
                }
            }
        }
        internal void AddToWorld()
        {
            Engine.World.PhysicsScene.AddRigidBody(_collision, (short)_group, _collisionEnabled ? (short)_collidesWith : (short)CustomCollisionGroup.None);
        }
        internal virtual void SetPhysicsTransform(Matrix4 worldMatrix)
        {
            Matrix conv = worldMatrix;
            _collision.WorldTransform = conv;
            _collision.InterpolationWorldTransform = conv;
            Engine.World.PhysicsScene.UpdateAabbs();
        }
        internal virtual void TransformUpdated()
        {
            Matrix mtx;
            _collision.GetWorldTransform(out mtx);
            _worldMatrix = mtx;
            TransformChanged(_worldMatrix);
        }
    }
}
