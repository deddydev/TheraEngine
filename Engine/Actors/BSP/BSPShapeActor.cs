using System;
using TheraEngine.Rendering.Models;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Actors.Types.BSP
{
    public class BSPMeshComponent : TransformComponent, I3DRenderable
    {
        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D();
        
        private MeshRenderer _manager;

        public void Render()
        {
            _manager.Render(WorldMatrix.Value, InverseWorldMatrix.Value.Transposed().GetRotationMatrix3());
        }

        public MeshRenderer Merge(MeshRenderer right, EIntersectionType intersection)
        {
            MeshRenderer m = new MeshRenderer();
            switch (intersection)
            {
                case EIntersectionType.Union:

                    break;
                case EIntersectionType.Intersection:

                    break;
                case EIntersectionType.Subtraction:

                    break;
                case EIntersectionType.Merge:

                    break;
                case EIntersectionType.Attach:

                    break;
                case EIntersectionType.Insert:

                    break;
            }
            return m;
        }

        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            throw new NotImplementedException();
        }
    }
    public enum EIntersectionType
    {
        Union,
        Intersection,
        Subtraction,
        Merge,
        Attach,
        Insert,
    }
    public abstract class BSPShapeActor : Actor<BSPMeshComponent>
    {

    }
}
