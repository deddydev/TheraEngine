using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
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
        TShape Shape { get; }
    }
    public abstract class Shape3DComponent : TRComponent, IShape3DComponent
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(true, true);

        [TSerialize]
        [Category(RenderingCategoryName)]
        public virtual RenderInfo3D RenderInfo
        {
            get => _renderInfo;
            protected set => _renderInfo = value ?? new RenderInfo3D(true, true);
        }
        
        protected abstract RenderCommand3D GetRenderCommand();
        public virtual void AddRenderables(RenderPasses passes, Camera camera)
            => passes.Add(GetRenderCommand());
    }
}
