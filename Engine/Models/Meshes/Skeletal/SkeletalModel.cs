using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files;
using TheraEngine.Core.Shapes;
using TheraEngine.ThirdParty.PMX;

namespace TheraEngine.Rendering.Models
{
    [TFileDef("Skeletal Model")]
    [TFileExt("skmdl", new string[] { "dae" }, new string[] { "pmx" })]
    public class SkeletalModel : TFileObject, IModelFile
    {
        public SkeletalModel() : base() { }
        public SkeletalModel(string name) : this() { _name = name; }
        
        [TSerialize("Skeleton")]
        public GlobalFileRef<Skeleton> _skeleton = new GlobalFileRef<Skeleton>();
        [TSerialize("RigidChildren")]
        public List<SkeletalRigidSubMesh> _rigidChildren = new List<SkeletalRigidSubMesh>();
        [TSerialize("SoftChildren")]
        public List<SkeletalSoftSubMesh> _softChildren = new List<SkeletalSoftSubMesh>();

        public GlobalFileRef<Skeleton> SkeletonRef => _skeleton;
        public List<SkeletalRigidSubMesh> RigidChildren => _rigidChildren;
        public List<SkeletalSoftSubMesh> SoftChildren => _softChildren;

        public BoundingBox CalculateBindPoseCullingAABB()
        {
            BoundingBox aabb = new BoundingBox();
            foreach (var s in RigidChildren)
                if (s.RenderInfo.CullingVolume != null)
                    aabb.Expand(s.RenderInfo.CullingVolume.GetAABB());
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
        public static async Task<SkeletalModel> LoadDAEAsync(
            string path, IProgress<float> progress, CancellationToken cancel)
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {
                IgnoreFlags =
                Collada.EIgnoreFlags.Extra |
                Collada.EIgnoreFlags.Cameras |
                Collada.EIgnoreFlags.Lights,
            };
            Collada.ImportResult data = await Collada.ImportAsync(path, o, progress, cancel);
            if (data != null && data.Models != null && data.Models.Count > 0)
            {
                ModelScene scene = data.Models[0];
                return scene.SkeletalModel;
            }
            return null;
        }
        [ThirdPartyExporter("pmx", false)]
        public static void ExportPMX(SkeletalModel model, string path)
        {
            PMXExporter e = new PMXExporter(model);
            e.Export(path);
        }
        public override void ManualWrite3rdParty(string filePath)
        {
            string ext = filePath.GetExtensionLowercase();
            switch (ext)
            {
                case "pmx":
                    ExportPMX(this, filePath);
                    break;
                default:
                    Engine.LogWarning("Unable to resolve extension at path " + filePath);
                    break;
            }
        }
        public override void ManualRead3rdParty(string filePath)
        {
            base.ManualRead3rdParty(filePath);
        }
        public override async Task ManualRead3rdPartyAsync(string filePath)
        {
            await base.ManualRead3rdPartyAsync(filePath);
        }
        public override async Task ManualWrite3rdPartyAsync(string filePath)
        {
            await base.ManualWrite3rdPartyAsync(filePath);
        }
    }
}
