using System.Collections.Generic;
using TheraEngine.Files;
using BulletSharp;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("STMDL", "Static Mesh", ImportableExtensions = new string[] { "DAE", "OBJ" })]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StaticMesh : FileObject, IModelFile
    {
        [ThirdPartyLoader("DAE")]
        public static FileObject LoadDAE(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags =
                Core.Files.IgnoreFlags.Extra |
                Core.Files.IgnoreFlags.Controllers |
                Core.Files.IgnoreFlags.Cameras |
                Core.Files.IgnoreFlags.Lights
            };
            return Collada.Import(path, o)?.Models[0].StaticModel;
        }
        [ThirdPartyLoader("OBJ")]
        public static FileObject LoadOBJ(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                
            };
            return OBJ.Import(path, o);
        }

        public StaticMesh() : base()
        {

        }
        public StaticMesh(string name)
        {
            _name = name;
        }

        ConvexShape _collision;

        public List<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public List<StaticSoftSubMesh> SoftChildren => _softChildren;

        public ConvexShape Collision { get => _collision; set => _collision = value; }
        
        [TSerialize("RigidChildren")]
        protected List<StaticRigidSubMesh> _rigidChildren = new List<StaticRigidSubMesh>();
        [TSerialize("SoftChildren")]
        protected List<StaticSoftSubMesh> _softChildren = new List<StaticSoftSubMesh>();
    }
}
