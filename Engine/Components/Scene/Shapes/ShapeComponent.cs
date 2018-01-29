using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class ShapeComponent : TRComponent, I3DRenderable, IRigidCollidable
    {
        [Category("Rendering")]
        public RenderInfo3D RenderInfo { get; } 
            = new RenderInfo3D(ERenderPass3D.OpaqueForward, null, false);

        [Browsable(false)]
        public abstract Shape CullingVolume { get; }
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        [Category("Rendering")]
        public RenderingParameters RenderParams
        {
            get => _renderParams;
            set
            {
                if (value != null)
                    _renderParams = value;
            }
        }

        public void InitPhysics(TRigidBodyConstructionInfo info)
        {
            if (info != null)
            {
                info.CollisionShape = GetCollisionShape();
                info.InitialWorldTransform = WorldMatrix;
                _rigidBodyCollision = TRigidBody.New(this, info);
                _rigidBodyCollision.TransformChanged += _rigidBodyCollision_TransformChanged;
                WorldTransformChanged += ShapeComponent_WorldTransformChanged;
            }
            else
                _rigidBodyCollision = null;
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
            _rigidBodyCollision?.Spawn();
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _rigidBodyCollision?.Despawn();
            base.OnDespawned();
        }

        [TSerialize("RenderParams")]
        private RenderingParameters _renderParams = new RenderingParameters();
        [TSerialize("CollisionObject")]
        protected TRigidBody _rigidBodyCollision;
        [TSerialize("IsVisible")]
        protected bool _isVisible;
        [TSerialize("VisibleByDefault")]
        protected bool _visibleByDefault;
        [TSerialize("VisibleInEditorOnly")]
        protected bool _visibleInEditorOnly;
        [TSerialize("HiddenFromOwner")]
        protected bool _hiddenFromOwner;
        [TSerialize("VisibleToOwnerOnly")]
        protected bool _visibleToOwnerOnly;
        
        [Category("Rendering")]
        public bool Visible
        {
            get => _isVisible;
            set => _isVisible = value;
        }
        [Category("Rendering")]
        public bool VisibleByDefault => _visibleByDefault;

        [Category("Physics")]
        public TRigidBody RigidBodyCollision => _rigidBodyCollision;

        [Category("Rendering")]
        public bool VisibleInEditorOnly
        {
            get => _visibleInEditorOnly;
            set => _visibleInEditorOnly = value;
        }
        [Category("Rendering")]
        public bool HiddenFromOwner
        {
            get => _hiddenFromOwner;
            set => _hiddenFromOwner = value;
        }
        [Category("Rendering")]
        public bool VisibleToOwnerOnly
        {
            get => _visibleToOwnerOnly;
            set => _visibleToOwnerOnly = value;
        }

        public abstract void Render();
        protected abstract TCollisionShape GetCollisionShape();

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (IsSpawned)
            {
                if (selected)
                    OwningActor.OwningWorld.Scene.Add(this);
                else
                    OwningActor.OwningWorld.Scene.Remove(this);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
