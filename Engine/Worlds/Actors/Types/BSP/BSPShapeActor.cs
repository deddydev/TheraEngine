using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors.Components.Scene.Mesh;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Worlds.Actors.Types.BSP
{
    public class BSPMeshComponent : TRSComponent, I3DRenderable
    {
        private RenderInfo3D _renderInfo;
        public RenderInfo3D RenderInfo => _renderInfo;
        [Browsable(false)]
        public Shape CullingVolume => _cullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        private Shape _cullingVolume;
        private PrimitiveManager _manager;

        public void Render()
        {
            _manager.Render(WorldMatrix, InverseWorldMatrix.Transposed().GetRotationMatrix3());
        }

        public PrimitiveManager Merge(PrimitiveManager right, IntersectionType intersection)
        {
            PrimitiveManager m = new PrimitiveManager();
            switch (intersection)
            {
                case IntersectionType.Union:

                    break;
                case IntersectionType.Intersection:

                    break;
                case IntersectionType.Subtraction:

                    break;
                case IntersectionType.Merge:

                    break;
                case IntersectionType.Attach:

                    break;
                case IntersectionType.Insert:

                    break;
            }
            return m;
        }
    }
    public enum IntersectionType
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
