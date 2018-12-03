using System;
using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene.Shapes
{
    public interface IShape3DComponent : ISceneComponent, I3DRenderable
    {

    }
    public interface ICollidableShape3DComponent : IShape3DComponent
    {
        TRigidBody RigidBodyCollision { get; set; }
    }
    public interface ICommonShape3DComponent : ICollidableShape3DComponent
    {
        Shape Shape { get; }
    }
    public abstract class Shape3DComponent : TRComponent, IShape3DComponent
    {
        public Shape3DComponent() { }

        [Browsable(false)]
        public abstract Shape CullingVolume { get; }
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        [TSerialize]
        [Category(RenderingCategoryName)]
        public RenderInfo3D RenderInfo { get; protected set; }
            = new RenderInfo3D(ERenderPass.OpaqueForward, false, true);

        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            OctreeNode?.ItemMoved(this);
        }
        protected abstract RenderCommand3D GetRenderCommand();
        public virtual void AddRenderables(RenderPasses passes, Camera camera)
            => passes.Add(GetRenderCommand(), RenderInfo.RenderPass);
    }
}
