using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Rendering.Models
{
    [TFileDef("Static Model")]
    [TFileExt("stmdl", new string[] { "dae", "obj" }, null)]
    public class StaticModel : TFileObject, IModelFile
    {
        public StaticModel() : base() { }
        public StaticModel(string name) { _name = name; }
        
        public EventList<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public EventList<StaticSoftSubMesh> SoftChildren => _softChildren;

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
        protected EventList<StaticRigidSubMesh> _rigidChildren = new EventList<StaticRigidSubMesh>();
        [TSerialize(nameof(SoftChildren))]
        protected EventList<StaticSoftSubMesh> _softChildren = new EventList<StaticSoftSubMesh>();
        
        /// <summary>
        /// Calculates the fully-encompassing aabb for this model based on each child mesh's aabb.
        /// </summary>
        public BoundingBox CalculateCullingAABB()
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
