using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class ShapeComponent : TRComponent, I3DRenderable, IRigidBodyCollidable
    {
        [TSerialize]
        [Category(RenderingCategoryName)]
        public RenderInfo3D RenderInfo { get; protected set; } 
            = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);

        public ShapeComponent()
        {
            _rc = new RenderCommandDebug3D(Render);
        }

        [Browsable(false)]
        public abstract Shape CullingVolume { get; }
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        [Category(RenderingCategoryName)]
        public RenderingParameters RenderParams
        {
            get => _renderParams;
            set
            {
                if (value != null)
                    _renderParams = value;
            }
        }

        public void InitPhysicsShape(TRigidBodyConstructionInfo info)
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
            Visible = VisibleByDefault;
            _rigidBodyCollision?.Spawn();
            if (Visible)
                OwningScene3D?.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _rigidBodyCollision?.Despawn();
            if (Visible)
                OwningScene3D?.Remove(this);
            base.OnDespawned();
        }

        [TSerialize(nameof(RenderParams))]
        private RenderingParameters _renderParams = new RenderingParameters();
        [TSerialize(nameof(RigidBodyCollision))]
        protected TRigidBody _rigidBodyCollision;
        [TSerialize(nameof(Visible))]
        protected bool _isVisible;
        [TSerialize(nameof(VisibleByDefault))]
        protected bool _visibleByDefault = true;
        [TSerialize(nameof(VisibleInEditorOnly))]
        protected bool _visibleInEditorOnly;
        [TSerialize(nameof(HiddenFromOwner))]
        protected bool _hiddenFromOwner;
        [TSerialize(nameof(VisibleToOwnerOnly))]
        protected bool _visibleToOwnerOnly;

        [Category(RenderingCategoryName)]
        public bool Visible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                if (IsSpawned && OwningScene != null)
                {
                    if (_isVisible)
                        OwningScene3D?.Add(this);
                    else
                        OwningScene3D?.Remove(this);
                }
            }
        }
        [Category(RenderingCategoryName)]
        public bool VisibleByDefault => _visibleByDefault;

        [Category(PhysicsCategoryName)]
        public TRigidBody RigidBodyCollision => _rigidBodyCollision;

        [Category(RenderingCategoryName)]
        public bool VisibleInEditorOnly
        {
            get => _visibleInEditorOnly;
            set => _visibleInEditorOnly = value;
        }
        [Category(RenderingCategoryName)]
        public bool HiddenFromOwner
        {
            get => _hiddenFromOwner;
            set => _hiddenFromOwner = value;
        }
        [Category(RenderingCategoryName)]
        public bool VisibleToOwnerOnly
        {
            get => _visibleToOwnerOnly;
            set => _visibleToOwnerOnly = value;
        }
        
        protected abstract TCollisionShape GetCollisionShape();

#if EDITOR
        public abstract void Render();
        private RenderCommandDebug3D _rc;
        public virtual void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
#endif
    }
}
