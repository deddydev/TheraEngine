﻿using System.Collections.Generic;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using System;
using TheraEngine.Core.Shapes;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models
{
    [File3rdParty(new string[] { "dae", "obj" }, null)]
    [FileExt("skmdl")]
    [FileDef("Skeletal Model")]
    public class SkeletalModel : TFileObject, IModelFile
    {
        [ThirdPartyLoader("dae", true)]
        public static async Task<TFileObject> LoadDAEAsync(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                //IgnoreFlags =
                //Core.Files.IgnoreFlags.Extra |
                //Core.Files.IgnoreFlags.Cameras |
                //Core.Files.IgnoreFlags.Lights,
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
    }
}
