using BulletSharp;
using System;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class ShapeComponent : TRComponent, IRenderable, IPhysicsDrivable
    {
        public ShapeComponent(PhysicsDriverInfo info)
        {
            if (info != null)
            {
                info.BodyInfo.CollisionShape = GetCollisionShape();
                _physics = new PhysicsDriver(this, info, PhysicsTransformChanged, PhysicsSimulationStateChanged);
            }
        }

        private void PhysicsSimulationStateChanged(bool isSimulating)
        {
            if (isSimulating)
                PhysicsSimulationStarted();
            else
                StopSimulatingPhysics(true);
        }

        protected RenderOctree.Node _renderNode;
        protected PhysicsDriver _physics;
        protected bool
            _isRendering,
            _isVisible,
            _visibleByDefault,
            _visibleInEditorOnly,
            _hiddenFromOwner,
            _visibleToOwnerOnly;

        public CustomCollisionGroup CollisionGroup
        {
            get => _physics.CollisionGroup;
            set => _physics.CollisionGroup = value;
        }
        public CustomCollisionGroup CollidesWith
        {
            get => _physics.CollidesWith;
            set => _physics.CollidesWith = value;
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
        public PhysicsDriver PhysicsDriver => _physics;
        public RenderOctree.Node RenderNode
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
            foreach (SceneComponent c in _children)
                c.RecalcGlobalTransform();
        }
    }
}
