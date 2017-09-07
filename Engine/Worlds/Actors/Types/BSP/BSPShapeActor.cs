using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models;

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
    }
    public abstract class BSPShapeActor : Actor<StaticMeshComponent>
    {

    }
}
