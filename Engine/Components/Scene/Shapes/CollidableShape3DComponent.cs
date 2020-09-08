using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class CollidableShape3DComponent : Shape3DComponent, IGenericCollidable
    {
        public CollidableShape3DComponent() : base() { }
        //public CollidableShape3DComponent(TRigidBodyConstructionInfo info) : base() 
        //    => GenerateRigidBody(info);
        
        protected TCollisionObject _collisionObject;

        public void RigidBodyUpdated()
        {
            if (IsSpawned && OwningWorld?.PhysicsWorld3D != null && _collisionObject != null)
            {
                OwningWorld.PhysicsWorld3D.RemoveCollisionObject(_collisionObject);
                OwningWorld.PhysicsWorld3D.AddCollisionObject(_collisionObject);
            }
        }
        Matrix4 ICollidable.CollidableWorldMatrix
        {
            get => WorldMatrix;
            set => WorldMatrix = value;
        }
        [Category(PhysicsCategoryName)]
        [TSerialize]
        public virtual TCollisionObject CollisionObject
        {
            get => _collisionObject;
            set
            {
                if (_collisionObject == value)
                    return;
                if (_collisionObject != null)
                {
                    if (IsSpawned)
                        OwningWorld.PhysicsWorld3D?.RemoveCollisionObject(_collisionObject);

                    _collisionObject.Owner = null;
                    _collisionObject.TransformChanged -= BodyMoved;
                    WorldTransformChanged -= ThisMoved;
                    _collisionObject.Dispose();
                }
                _collisionObject = value;
                if (_collisionObject != null)
                {
                    _collisionObject.Owner = this;
                    _collisionObject.TransformChanged += BodyMoved;
                    WorldTransformChanged += ThisMoved;

                    if (IsSpawned)
                        OwningWorld.PhysicsWorld3D?.AddCollisionObject(_collisionObject);
                }
            }
        }

        public abstract TCollisionShape GetCollisionShape();

        public void GenerateCollisionObject(ICollisionObjectConstructionInfo info)
        {
            if (info is null)
            {
                //Engine.LogWarning("A rigid body could not be generated for collidable shape component; construction info is null.");
                CollisionObject = null;
                return;
            }

            info.CollisionShape = GetCollisionShape();
            info.InitialWorldTransform = WorldMatrix;

            if (info.CollisionShape != null)
            {
                switch (info)
                {
                    case TRigidBodyConstructionInfo r:
                        CollisionObject = TRigidBody.New(r);
                        break;
                    case TSoftBodyConstructionInfo s:
                        CollisionObject = TSoftBody.New(s);
                        break;
                    case TGhostBodyConstructionInfo g:
                        CollisionObject = TGhostBody.New(g);
                        break;
                }
            }
            else
            {
                Engine.LogWarning("A rigid body could not be generated for collidable shape component; collision shape is null.");
                CollisionObject = null;
            }
        }

        private void BodyMoved(Matrix4 transform)
            => WorldMatrix = _collisionObject.WorldTransform;

        private void ThisMoved(ISceneComponent comp)
            => _collisionObject.WorldTransform = (WorldMatrix);

        private void PhysicsSimulationStateChanged(bool isSimulating)
        {
            if (isSimulating)
                PhysicsSimulationStarted();
            else
                StopSimulatingPhysics(true);
        }

        protected override void OnSpawned()
        {
            _collisionObject?.Spawn(OwningWorld);
            base.OnSpawned();
        }
        protected override void OnDespawned()
        {
            _collisionObject?.Despawn(OwningWorld);
            base.OnDespawned();
        }
    }
}
