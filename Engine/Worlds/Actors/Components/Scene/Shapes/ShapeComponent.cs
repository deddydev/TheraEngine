using BulletSharp;
using System;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors
{
    public abstract class ShapeComponent : TRComponent, IRenderable, IPhysicsDrivable
    {
        public void InitPhysics(PhysicsConstructionInfo info)
        {
            if (info != null)
            {
                info.CollisionShape = GetCollisionShape();
                info.MotionState = new DefaultMotionState(WorldMatrix);
                _physicsDriver = new PhysicsDriver(this, info, PhysicsTransformChanged, PhysicsSimulationStateChanged);
            }
            else
                _physicsDriver = null;
        }

        private void PhysicsSimulationStateChanged(bool isSimulating)
        {
            if (isSimulating)
                PhysicsSimulationStarted();
            else
                StopSimulatingPhysics(true);
        }

        public override void OnSpawned()
        {
            _physicsDriver?.OnSpawned();
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _physicsDriver?.OnDespawned();
            base.OnDespawned();
        }

        protected Octree.Node _renderNode;
        protected PhysicsDriver _physicsDriver;
        protected bool
            _isRendering,
            _isVisible,
            _visibleByDefault,
            _visibleInEditorOnly,
            _hiddenFromOwner,
            _visibleToOwnerOnly;

        public CustomCollisionGroup CollisionGroup
        {
            get => _physicsDriver.CollisionGroup;
            set => _physicsDriver.CollisionGroup = value;
        }
        public CustomCollisionGroup CollidesWith
        {
            get => _physicsDriver.CollidesWith;
            set => _physicsDriver.CollidesWith = value;
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }
        public bool Visible
        {
            get => _isVisible;
            set => _isVisible = value;
        }
        public abstract Shape CullingVolume { get; }
        public bool VisibleByDefault => _visibleByDefault;
        public PhysicsDriver PhysicsDriver => _physicsDriver;
        public Octree.Node RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }

        public bool VisibleInEditorOnly
        {
            get => _visibleInEditorOnly;
            set => _visibleInEditorOnly = value;
        }
        public bool HiddenFromOwner
        {
            get => _hiddenFromOwner;
            set => _hiddenFromOwner = value;
        }
        public bool VisibleToOwnerOnly
        {
            get => _visibleToOwnerOnly;
            set => _visibleToOwnerOnly = value;
        }

        public abstract void Render();
        protected abstract CollisionShape GetCollisionShape();

        protected virtual void PhysicsTransformChanged(Matrix4 worldMatrix)
        {
            WorldMatrix = worldMatrix;
        }
    }
}
