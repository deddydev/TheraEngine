using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    [FileExt("skrmesh")]
    [FileDef("Skeletal Rigid Sub Mesh")]
    public class SkeletalRigidSubMesh : TFileObject, ISkeletalSubMesh
    {
        public SkeletalRigidSubMesh() { _name = "SkeletalRigidSubMesh"; }
        public SkeletalRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material)
        {
            _name = name;
            _visibleByDefault = visibleByDefault;
            _cullingVolume.File = cullingVolume;
            _lods.Add(new LOD(material, primitives, 0.0f));
        }
        public SkeletalRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            List<LOD> lods)
        {
            _name = name;
            _visibleByDefault = visibleByDefault;
            _cullingVolume.File = cullingVolume;
            _lods = lods ?? new List<LOD>();
        }
        public SkeletalRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            params LOD[] lods)
        {
            _name = name;
            _visibleByDefault = visibleByDefault;
            _cullingVolume.File = cullingVolume;
            _lods = lods.ToList();
        }

        protected List<LOD> _lods = new List<LOD>();
        protected bool _visibleByDefault = true;
        [TSerialize(nameof(CullingVolume), Order = 1)]
        protected GlobalFileRef<Shape> _cullingVolume = new GlobalFileRef<Shape>();

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
        [TSerialize(Order = 0)]
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null);
        [TSerialize(Order = 2)]
        public List<LOD> LODs
        {
            get => _lods;
            set => _lods = value;
        } 
        //[Browsable(false)]
        public GlobalFileRef<Shape> CullingVolumeRef => _cullingVolume;
        [Browsable(false)]
        public Shape CullingVolume => _cullingVolume.File;
    }
}
