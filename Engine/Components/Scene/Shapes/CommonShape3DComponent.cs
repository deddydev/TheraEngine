using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;

namespace TheraEngine.Components.Scene.Shapes
{
    public abstract class CommonShape3DComponent<T> : CollidableShape3DComponent where T : Shape, new()
    {
        public CommonShape3DComponent()
            : this(null, null) { }
        public CommonShape3DComponent(T shape)
            : this(shape, null) { }
        public CommonShape3DComponent(T shape, TRigidBodyConstructionInfo info)
            : base()
        {
            Shape = shape;
            RenderCommand = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render);
            GenerateRigidBody(info);
        }

        public RenderCommandMethod3D RenderCommand { get; }
        protected T _shape;

        [TSerialize]
        [Category(RenderingCategoryName)]
        public T Shape
        {
            get => _shape;
            set => _shape = value ?? new T();
        }

        [Browsable(false)]
        public override Shape CullingVolume => _shape;

        protected virtual void Render() => _shape?.Render();
        protected override RenderCommand3D GetRenderCommand() => RenderCommand;

        protected override void OnWorldTransformChanged()
        {
            _shape?.SetTransformMatrix(WorldMatrix);
            base.OnWorldTransformChanged();
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
