﻿using System.Collections.Generic;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("SKMDL", "Skeletal Mesh", ImportableExtensions = new string[] { "DAE", "OBJ" })]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SkeletalMesh : FileObject, IModelFile
    {
        [ThirdPartyLoader("DAE")]
        public static FileObject LoadDAE(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                ImportAnimations = false,
                ImportModels = true
            };
            return Collada.Import(path, o)?.Models[0].SkeletalModel;
        }
        [ThirdPartyLoader("OBJ")]
        public static FileObject LoadOBJ(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                ImportAnimations = false,
                ImportModels = true
            };
            return OBJ.Import(path, o);
        }

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
