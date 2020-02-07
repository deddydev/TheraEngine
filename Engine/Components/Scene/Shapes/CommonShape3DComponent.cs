using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class CommonShape3DComponent<T> : CollidableShape3DComponent where T : TShape, new()
    {
        protected CommonShape3DComponent()
            : this(null, null) { }

        protected CommonShape3DComponent(T shape)
            : this(shape, null) { }

        protected CommonShape3DComponent(T shape, ICollisionObjectConstructionInfo info)
            : base()
        {
            Shape = shape;
            RenderCommand = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render);
            GenerateCollisionObject(info);
        }


        [Category(RenderingCategoryName)]
        public RenderCommandMethod3D RenderCommand { get; }
        protected T _shape;

        [Category(RenderingCategoryName)]
        public override IRenderInfo3D RenderInfo
        {
            get => base.RenderInfo;
            protected set
            {
                base.RenderInfo = value;
                RenderInfo.CullingVolume = _shape;
                RenderInfo.CastsShadows = false;
                RenderInfo.ReceivesShadows = false;
            }
        }

        [TSerialize]
        [Category(RenderingCategoryName)]
        public virtual T Shape
        {
            get => _shape;
            set
            {
                if (_shape != null)
                    _shape.VolumePropertyChanged -= _shape_VolumePropertyChanged;
                _shape = value ?? new T();
                _shape.VolumePropertyChanged += _shape_VolumePropertyChanged;
                RenderInfo.CullingVolume = _shape;
                _shape_VolumePropertyChanged();
            }
        }

        private void _shape_VolumePropertyChanged()
        {
            if (CollisionObject != null)
            {
                CollisionObject.CollisionShape = GetCollisionShape();
                RigidBodyUpdated();
            }
        }

        protected virtual void Render(bool shadowPass) => _shape?.Render(shadowPass);
        protected override RenderCommand3D GetRenderCommand() => RenderCommand;

        protected override void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            _shape?.SetTransformMatrix(WorldMatrix);
            base.OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }

        public override TCollisionShape GetCollisionShape()
            => _shape?.GetCollisionShape();

        public bool Contains(Vec3 point)
            => _shape.Contains(point);
        public EContainment Contains(BoundingBox box)
            => _shape.Contains(box);
        public EContainment Contains(Box box)
            => _shape.Contains(box);
        public EContainment Contains(Sphere sphere)
            => _shape.Contains(sphere);
        public EContainment ContainedWithin(BoundingBox box)
            => box.Contains(_shape);
        public EContainment ContainedWithin(Box box)
            => box.Contains(_shape);
        public EContainment ContainedWithin(Sphere sphere)
            => sphere.Contains(_shape);
        public EContainment ContainedWithin(Frustum frustum)
            => frustum.Contains(_shape);
    }
}
