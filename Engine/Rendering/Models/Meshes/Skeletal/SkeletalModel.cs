using System.Collections.Generic;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [File3rdParty(new string[] { "dae", "obj" }, null)]
    [FileExt("skmdl")]
    [FileDef("Skeletal Model")]
    public class SkeletalModel : FileObject, IModelFile
    {
        [ThirdPartyLoader("dae")]
        public static FileObject LoadDAE(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags =
                Core.Files.IgnoreFlags.Extra |
                Core.Files.IgnoreFlags.Cameras |
                Core.Files.IgnoreFlags.Lights
            };
            return Collada.Import(path, o)?.Models[0].SkeletalModel;
        }
        [ThirdPartyLoader("obj")]
        public static FileObject LoadOBJ(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {

            };
            return OBJ.Import(path, o);
        }

        public SkeletalModel() : base() { }
        public SkeletalModel(string name) : this() { _name = name; }
        
        [TSerialize("Skeleton")]
        protected GlobalFileRef<Skeleton> _skeleton = new GlobalFileRef<Skeleton>();
        [TSerialize("RigidChildren")]
        protected List<SkeletalRigidSubMesh> _rigidChildren = new List<SkeletalRigidSubMesh>();
        [TSerialize("SoftChildren")]
        protected List<SkeletalSoftSubMesh> _softChildren = new List<SkeletalSoftSubMesh>();

        public GlobalFileRef<Skeleton> Skeleton => _skeleton;
        public List<SkeletalRigidSubMesh> RigidChildren => _rigidChildren;
        public List<SkeletalSoftSubMesh> SoftChildren => _softChildren;
    }
}
