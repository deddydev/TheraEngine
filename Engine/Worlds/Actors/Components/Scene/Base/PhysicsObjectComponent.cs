using BulletSharp;
using CustomEngine.Rendering;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class PhysicsObjectComponent : SceneComponent
    {
        protected RigidBody _collision;
        protected bool _collisionEnabled, _simulatingPhysics;
        public bool SimulatingPhysics
        {
            get { return _simulatingPhysics; }
            set
            {
                if (_simulatingPhysics == value)
                    return;
                _simulatingPhysics = value;
                if (_collision != null && Engine.World != null)
                {
                    if (_simulatingPhysics)
                    {
                        Engine.World.PhysicsScene.AddRigidBody(_collision);
                        _collision.UserObject = this;
                    }
                    else
                    {
                        Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                        _collision.UserObject = null;
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
            }
        }
        public RigidBody CollisionObject { get { return _collision; } }
        public void SetCollisionObject(RigidBody body, CustomCollisionGroup group, CustomCollisionGroup groupsToCollideWith)
        {
            if (_collision != null)
            {
                if (_collisionEnabled && Engine.World != null)
                    Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                _collision.UserObject = null;
            }
            _collision = body;
            if (_collision != null)
            {
                SetPhysicsTransform(WorldMatrix);
                if (_collisionEnabled && Engine.World != null)
                    Engine.World.PhysicsScene.AddRigidBody(_collision, (short)group, (short)groupsToCollideWith);
                _collision.UserObject = this;
            }
        }
        internal virtual void SetPhysicsTransform(Matrix4 worldMatrix)
        {
            Matrix conv = worldMatrix;
            _collision.WorldTransform = conv;
            _collision.InterpolationWorldTransform = conv;
            Engine.World.PhysicsScene.UpdateAabbs();
        }
        internal virtual void UpdateTransform()
        {
            Matrix mtx;
            _collision.GetWorldTransform(out mtx);
            _localTransform = mtx;
        }
    }
}
