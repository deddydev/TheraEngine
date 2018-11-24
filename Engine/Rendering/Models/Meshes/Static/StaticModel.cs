using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Rendering.Models
{
    [TFile3rdParty(new string[] { "dae", "obj" }, null)]
    [TFileExt("stmdl")]
    [TFileDef("Static Model")]
    public class StaticModel : TFileObject, IModelFile
    {
        public StaticModel() : base()
        {

        }
        public StaticModel(string name)
        {
            _name = name;
        }

        public List<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public List<StaticSoftSubMesh> SoftChildren => _softChildren;

        [TSerialize]
        public TCollisionShape CollisionShape { get; set; }

        //[CustomXMLSerializeMethod(nameof(Collision))]
        //private void SerializeConvexShape(XmlWriter writer)
        //{
        //    if (_collision == null)
        //        return;

        //    //TODO: serialize convex shape collision using bullet serializer
        //    //int size = _collision.CalculateSerializeBufferSize();
        //}

        [TSerialize(nameof(RigidChildren))]
        protected List<StaticRigidSubMesh> _rigidChildren = new List<StaticRigidSubMesh>();
        [TSerialize(nameof(SoftChildren))]
        protected List<StaticSoftSubMesh> _softChildren = new List<StaticSoftSubMesh>();
        
        /// <summary>
        /// Calculates the fully-encompassing aabb for this model based on each child mesh's aabb.
        /// </summary>
        public BoundingBox CalculateCullingAABB()
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
        [ThirdPartyLoader("dae", true)]
        public static async Task<StaticModel> LoadDAEAsync(
            string path, IProgress<float> progress, CancellationToken cancel)
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {
                IgnoreFlags =
                Collada.EIgnoreFlags.Extra |
                Collada.EIgnoreFlags.Controllers |
                Collada.EIgnoreFlags.Cameras |
                Collada.EIgnoreFlags.Lights
            };
            return (await Collada.ImportAsync(path, o))?.Models[0].StaticModel;
        }
        [ThirdPartyLoader("obj", false)]
        public static StaticModel LoadOBJ(string path)
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {

            };
            return OBJ.Import(path, o);
        }
    }
}
