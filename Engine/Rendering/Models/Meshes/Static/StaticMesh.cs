using System.Collections.Generic;
using TheraEngine.Files;
using BulletSharp;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("STM", "Static Mesh")]
    public class StaticMesh : FileObject, IModelFile
    {
        public StaticMesh() : base()
        {
            _rigidChildren.PostRemoved += RigidChildRemoved;
            _rigidChildren.PostAdded += RigidChildAdded;
            _softChildren.PostRemoved += SoftChildRemoved;
            _softChildren.PostAdded += SoftChildAdded;
        }
        public StaticMesh(string name)
        {
            _name = name;
        }

        ConvexShape _collision;

        public MonitoredList<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public MonitoredList<StaticSoftSubMesh> SoftChildren => _softChildren;

        public ConvexShape Collision { get => _collision; set => _collision = value; }

        [Serialize("RigidChildren")]
        protected MonitoredList<StaticRigidSubMesh> _rigidChildren = new MonitoredList<StaticRigidSubMesh>();
        [Serialize("SoftChildren")]
        protected MonitoredList<StaticSoftSubMesh> _softChildren = new MonitoredList<StaticSoftSubMesh>();

        protected virtual void RigidChildAdded(StaticRigidSubMesh item)
        {
            item.Model = this;
        }
        protected virtual void RigidChildRemoved(StaticRigidSubMesh item)
        {
            if (item.Model == this)
                item.Model = null;
        }
        protected virtual void SoftChildAdded(StaticSoftSubMesh item)
        {
            item.Model = this;
        }
        protected virtual void SoftChildRemoved(StaticSoftSubMesh item)
        {
            if (item.Model == this)
                item.Model = null;
        }
    }
}
