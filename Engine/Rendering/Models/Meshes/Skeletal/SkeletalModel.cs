using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Files;
using TheraEngine.ThirdParty.PMX;

namespace TheraEngine.Rendering.Models
{
    [File3rdParty(new string[] { "dae", "obj" }, new string[] { "pmx" })]
    [FileExt("skmdl")]
    [FileDef("Skeletal Model")]
    public class SkeletalModel : TFileObject, IModelFile
    {
        public SkeletalModel() : base() { }
        public SkeletalModel(string name) : this() { _name = name; }
        
        [TSerialize("Skeleton")]
        protected GlobalFileRef<Skeleton> _skeleton = new GlobalFileRef<Skeleton>();
        [TSerialize("RigidChildren")]
        protected List<SkeletalRigidSubMesh> _rigidChildren = new List<SkeletalRigidSubMesh>();
        [TSerialize("SoftChildren")]
        protected List<SkeletalSoftSubMesh> _softChildren = new List<SkeletalSoftSubMesh>();

        public GlobalFileRef<Skeleton> SkeletonRef => _skeleton;
        public List<SkeletalRigidSubMesh> RigidChildren => _rigidChildren;
        public List<SkeletalSoftSubMesh> SoftChildren => _softChildren;

        public BoundingBox CalculateBindPoseCullingAABB()
        {
            BoundingBox aabb = new BoundingBox();
            foreach (var s in RigidChildren)
                if (s.CullingVolume != null)
                    aabb.Expand(s.CullingVolume.GetAABB());
            //foreach (var s in SoftChildren)
            //    if (s.CullingVolume != null)
            //        aabb.Expand(s.CullingVolume.GetAABB());
            return aabb;
        }

        public BaseSubMesh[] CollectAllMeshes()
        {
            BaseSubMesh[] meshes = new BaseSubMesh[RigidChildren.Count + SoftChildren.Count];
            for (int i = 0; i < RigidChildren.Count; ++i)
                meshes[i] = RigidChildren[i];
            for (int i = 0; i < SoftChildren.Count; ++i)
                meshes[RigidChildren.Count + i] = SoftChildren[i];
            return meshes;
        }

        [ThirdPartyLoader("dae", true)]
        public static async Task<TFileObject> LoadDAEAsync(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags =
                Collada.EIgnoreFlags.Extra |
                Collada.EIgnoreFlags.Cameras |
                Collada.EIgnoreFlags.Lights,
            };
            Collada.Data data = await Collada.ImportAsync(path, o);
            if (data != null && data.Models != null && data.Models.Count > 0)
            {
                ModelScene scene = data.Models[0];
                return scene.SkeletalModel;
            }
            return null;
        }
        [ThirdPartyLoader("obj")]
        public static TFileObject LoadOBJ(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {

            };
            return OBJ.Import(path, o);
        }
        [ThirdPartyExporter("pmx")]
        public static void ExportPMX(object obj, string path)
        {
            SkeletalModel m = obj as SkeletalModel;
            PMXExporter e = new PMXExporter(m);
            e.Export(path);
        }
    }
}
