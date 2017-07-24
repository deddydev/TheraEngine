using System.Collections.Generic;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("SKMDL", "Skeletal Mesh")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SkeletalMesh : FileObject, IModelFile
    {
        public SkeletalMesh() : base() { }
        public SkeletalMesh(string name) : this() { _name = name; }

        [Serialize("RigidChildren")]
        protected List<SkeletalRigidSubMesh> _rigidChildren = new List<SkeletalRigidSubMesh>();
        [Serialize("SoftChildren")]
        protected List<SkeletalSoftSubMesh> _softChildren = new List<SkeletalSoftSubMesh>();
        
        public List<SkeletalRigidSubMesh> RigidChildren => _rigidChildren;
        public List<SkeletalSoftSubMesh> SoftChildren => _softChildren;
    }
}
