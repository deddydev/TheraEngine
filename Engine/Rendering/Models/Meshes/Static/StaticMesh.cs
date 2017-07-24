using System.Collections.Generic;
using TheraEngine.Files;
using BulletSharp;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("STMDL", "Static Mesh")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StaticMesh : FileObject, IModelFile
    {
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
        
        [Serialize("RigidChildren")]
        protected List<StaticRigidSubMesh> _rigidChildren = new List<StaticRigidSubMesh>();
        [Serialize("SoftChildren")]
        protected List<StaticSoftSubMesh> _softChildren = new List<StaticSoftSubMesh>();
    }
}
