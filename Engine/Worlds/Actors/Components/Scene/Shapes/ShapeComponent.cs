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
                _physics = new PhysicsDriver(info, PhysicsTransformChanged, PhysicsSimulationStateChanged);
            }
        }

        private void PhysicsSimulationStateChanged(bool isSimulating)
        {
            if (isSimulating)
                PhysicsSimulationStarted();
            else
                StopSimulatingPhysics(true);
        }

        protected PhysicsDriver _physics;
        protected bool _isRendering, _isVisible, _visibleByDefault;

        public CustomCollisionGroup CollisionGroup
        {
            get { return _physics.CollisionGroup; }
            set { _physics.CollisionGroup = value; }
        }
        public CustomCollisionGroup CollidesWith
        {
            get { return _physics.CollidesWith; }
            set { _physics.CollidesWith = value; }
        }
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public bool Visible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        public abstract Shape CullingVolume { get; }
        public bool VisibleByDefault { get { return _visibleByDefault; } }
        public PhysicsDriver PhysicsDriver { get { return _physics; } }

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
