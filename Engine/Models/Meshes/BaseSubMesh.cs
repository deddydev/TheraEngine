using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public abstract class BaseSubMesh : TFileObject, IBaseSubMesh
    {
        [TSerialize(IsAttribute = true)]
        public ERenderPass RenderPass { get; set; } = ERenderPass.OpaqueDeferredLit;

        [TSerialize(Order = 0)]
        public IRenderInfo3D RenderInfo { get; set; }
        
        [DisplayName("Levels Of Detail")]
        [Browsable(false)]
        [TSerialize(Order = 2)]
        public IEventList<ILOD> LODs { get; set; }

        public BaseSubMesh() { }
        public BaseSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            PrimitiveData primitives,
            TMaterial material)
        {
            _name = name;
            RenderInfo = renderInfo ?? new RenderInfo3D() { CastsShadows = true, ReceivesShadows = true };
            RenderPass = renderPass;
            LODs = new EventList<ILOD>() { new LOD(material, primitives, 0.0f) };
        }
        public BaseSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            IEventList<ILOD> lods)
        {
            _name = name;
            RenderInfo = renderInfo ?? new RenderInfo3D() { CastsShadows = true, ReceivesShadows = true };
            RenderPass = renderPass;
            LODs = lods ?? new EventList<ILOD>();
        }
        public BaseSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            params ILOD[] lods)
        {
            _name = name;
            RenderInfo = renderInfo ?? new RenderInfo3D() { CastsShadows = true, ReceivesShadows = true };
            RenderPass = renderPass;
            LODs = new EventList<ILOD>(lods);
        }
    }
}
