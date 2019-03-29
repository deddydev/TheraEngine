using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public abstract class BaseSubMesh : TFileObject, IBaseSubMesh
    {
        [TSerialize(IsAttribute = true)]
        public ERenderPass RenderPass { get; set; } = ERenderPass.OpaqueDeferredLit;

        [TSerialize(Order = 0)]
        public RenderInfo3D RenderInfo { get; set; }
        
        [DisplayName("Levels Of Detail")]
        [Browsable(false)]
        [TSerialize(Order = 2)]
        public List<LOD> LODs { get; set; }

        public BaseSubMesh() { }
        public BaseSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            PrimitiveData primitives,
            TMaterial material)
        {
            _name = name;
            RenderInfo = renderInfo ?? new RenderInfo3D() { CastsShadows = true, ReceivesShadows = true };
            RenderPass = renderPass;
            LODs = new List<LOD>() { new LOD(material, primitives, 0.0f) };
        }
        public BaseSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            List<LOD> lods)
        {
            _name = name;
            RenderInfo = renderInfo ?? new RenderInfo3D() { CastsShadows = true, ReceivesShadows = true };
            RenderPass = renderPass;
            LODs = lods ?? new List<LOD>();
        }
        public BaseSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            params LOD[] lods)
        {
            _name = name;
            RenderInfo = renderInfo ?? new RenderInfo3D() { CastsShadows = true, ReceivesShadows = true };
            RenderPass = renderPass;
            LODs = lods.ToList();
        }
    }
}
