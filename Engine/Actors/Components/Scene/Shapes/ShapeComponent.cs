using System;
using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class ShapeComponent : TRComponent, I3DRenderable, IRigidBodyCollidable
    {
        protected TRigidBody _rigidBodyCollision;

        public ShapeComponent()
        {
            _rc = new RenderCommandMethod3D(Render);
        }

        [Browsable(false)]
        public abstract Shape CullingVolume { get; }
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [TSerialize]
        [Category(RenderingCategoryName)]
        public RenderInfo3D RenderInfo { get; protected set; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, true);
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
                    _rigidBodyCollision.TransformChanged -= _rigidBodyCollision_TransformChanged;
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += _rigidBodyCollision_TransformChanged;

                    if (IsSpawned)
                        OwningWorld.PhysicsWorld?.AddCollisionObject(_rigidBodyCollision);
                }
            }
        }

        public void InitPhysicsShape(TRigidBodyConstructionInfo info)
        {
            if (info != null)
            {
                info.CollisionShape = GetCollisionShape();
                info.InitialWorldTransform = WorldMatrix;
                RigidBodyCollision = TRigidBody.New(info);
                WorldTransformChanged += ShapeComponent_WorldTransformChanged;
            }
            else
                RigidBodyCollision = null;
        }

        private void _rigidBodyCollision_TransformChanged(Matrix4 transform)
            => WorldMatrix = _rigidBodyCollision.WorldTransform;
        private void ShapeComponent_WorldTransformChanged()
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

        protected abstract TCollisionShape GetCollisionShape();

#if EDITOR
        public abstract void Render();
        private RenderCommandMethod3D _rc;
        public virtual void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
#endif
    }
}
