using System;
using System.ComponentModel;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class CollidableShape3DComponent : Shape3DComponent, IRigidBodyCollidable
    {
        public CollidableShape3DComponent() : base() { }
        //public CollidableShape3DComponent(TRigidBodyConstructionInfo info) : base() 
        //    => GenerateRigidBody(info);
        
        protected TRigidBody _rigidBodyCollision;

        [Category(PhysicsCategoryName)]
        [TSerialize]
        public TRigidBody RigidBodyCollision
        {
            get => _rigidBodyCollision;
            set
            {
                if (_rigidBodyCollision == value)
                    return;
                if (_rigidBodyCollision != null)
                {
                    if (IsSpawned)
                        OwningWorld.PhysicsWorld?.RemoveCollisionObject(_rigidBodyCollision);

                    _rigidBodyCollision.Owner = null;
                    _rigidBodyCollision.TransformChanged -= BodyMoved;
                    WorldTransformChanged -= ThisMoved;
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += BodyMoved;
                    WorldTransformChanged += ThisMoved;

                    if (IsSpawned)
                        OwningWorld.PhysicsWorld?.AddCollisionObject(_rigidBodyCollision);
                }
            }
        }

        public abstract TCollisionShape GetCollisionShape();

        public void GenerateRigidBody(TRigidBodyConstructionInfo info)
        {
            if (info == null)
            {
                Engine.LogWarning("A rigid body could not be generated for collidable shape component; construction info is null.");
                RigidBodyCollision = null;
                return;
            }

            info.CollisionShape = GetCollisionShape();
            info.InitialWorldTransform = WorldMatrix;

            if (info.CollisionShape != null)
                RigidBodyCollision = TRigidBody.New(info);
            else
            {
                Engine.LogWarning("A rigid body could not be generated for collidable shape component; collision shape is null.");
                RigidBodyCollision = null;
            }
        }

        private void BodyMoved(Matrix4 transform)
            => WorldMatrix = _rigidBodyCollision.WorldTransform;

        private void ThisMoved()
            => _rigidBodyCollision.ProceedToTransform(WorldMatrix);

        private void PhysicsSimulationStateChanged(bool isSimulating)
        {
            if (isSimulating)
                PhysicsSimulationStarted();
            else
                StopSimulatingPhysics(true);
        }

        public override void OnSpawned()
        {
            _rigidBodyCollision?.Spawn(OwningWorld);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _rigidBodyCollision?.Despawn(OwningWorld);
            base.OnDespawned();
        }
    }
}
