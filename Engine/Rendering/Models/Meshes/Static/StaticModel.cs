using System.Collections.Generic;
using TheraEngine.Files;
using System.ComponentModel;
using System;
using System.Xml;
using TheraEngine.Physics;
using TheraEngine.Core.Shapes;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models
{
    [File3rdParty(new string[] { "dae", "obj" }, null)]
    [FileExt("stmdl")]
    [FileDef("Static Model")]
    public class StaticModel : TFileObject, IModelFile
    {
        [ThirdPartyLoader("dae", true)]
        public static async Task<TFileObject> LoadDAEAsync(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags =
                Core.Files.IgnoreFlags.Extra |
                Core.Files.IgnoreFlags.Controllers |
                Core.Files.IgnoreFlags.Cameras |
                Core.Files.IgnoreFlags.Lights
            };
            return (await Collada.ImportAsync(path, o))?.Models[0].StaticModel;
        }
        [ThirdPartyLoader("obj")]
        public static TFileObject LoadOBJ(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                
            };
            return OBJ.Import(path, o);
        }

        public StaticModel() : base()
        {

        }
        public StaticModel(string name)
        {
            _name = name;
        }

        private TCollisionShape _collisionShape;

        public List<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public List<StaticSoftSubMesh> SoftChildren => _softChildren;

        [TSerialize]
        public TCollisionShape CollisionShape
        {
            get => _collisionShape;
            set => _collisionShape = value;
        }

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
    }
}
