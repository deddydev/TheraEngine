using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;
using System.Linq;
using CustomEngine.Files;
using System.Collections;
using System.Collections.ObjectModel;
using CustomEngine.Rendering.Models.Materials;
using System.IO;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public class StaticMesh : FileObject
    {
        public StaticMesh() : base()
        {
            _rigidChildren.Removed += RigidChildRemoved;
            _rigidChildren.Added += RigidChildAdded;
            _softChildren.Removed += SoftChildRemoved;
            _softChildren.Added += SoftChildAdded;
        }
        public StaticMesh(string name)
        {
            _name = name;
        }

        public MonitoredList<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public MonitoredList<StaticSoftSubMesh> SoftChildren => _softChildren;

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
