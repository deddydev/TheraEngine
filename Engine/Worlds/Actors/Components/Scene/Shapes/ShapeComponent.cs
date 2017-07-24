using BulletSharp;
using System;
using System.ComponentModel;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public abstract class ShapeComponent : TRComponent, I3DRenderable, IPhysicsDrivable
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(RenderPassType3D.OpaqueForward, null, false);
        public RenderInfo3D RenderInfo => _renderInfo;
        [Browsable(false)]
        public abstract Shape CullingVolume { get; }
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

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
        
        protected PhysicsDriver _physicsDriver;
        protected bool
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
        public bool Visible
        {
            get => _isVisible;
            set => _isVisible = value;
        }
        public bool VisibleByDefault => _visibleByDefault;
        public PhysicsDriver PhysicsDriver => _physicsDriver;

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
