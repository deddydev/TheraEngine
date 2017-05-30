using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;
using System.Linq;
using CustomEngine.Files;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CustomEngine.Rendering.Models
{
    [FileClass("SKLM", "Skeletal Mesh")]
    public class SkeletalMesh : FileObject
    {
        public SkeletalMesh() : base()
        {
            _rigidChildren.PostRemoved += RigidChildRemoved;
            _rigidChildren.PostAdded += RigidChildAdded;
            _softChildren.PostRemoved += SoftChildRemoved;
            _softChildren.PostAdded += SoftChildAdded;
        }
        public SkeletalMesh(string name) : this()
        {
            _name = name;
        }

        [Serialize("RigidChildren")]
        protected MonitoredList<SkeletalRigidSubMesh> _rigidChildren = new MonitoredList<SkeletalRigidSubMesh>();
        [Serialize("SoftChildren")]
        protected MonitoredList<SkeletalSoftSubMesh> _softChildren = new MonitoredList<SkeletalSoftSubMesh>();

        public MonitoredList<SkeletalRigidSubMesh> RigidChildren => _rigidChildren;
        public MonitoredList<SkeletalSoftSubMesh> SoftChildren => _softChildren;

        protected virtual void RigidChildAdded(SkeletalRigidSubMesh item)
        {
            item.Model = this;
            //item.SkeletonChanged(_skeleton);
        }
        protected virtual void RigidChildRemoved(SkeletalRigidSubMesh item)
        {
            if (item.Model == this)
            {
                //item.SkeletonChanged(null);
                item.Model = null;
            }
        }
        protected virtual void SoftChildAdded(SkeletalSoftSubMesh item)
        {
            item.Model = this;
            //item.SkeletonChanged(_skeleton);
        }
        protected virtual void SoftChildRemoved(SkeletalSoftSubMesh item)
        {
            if (item.Model == this)
            {
                //item.SkeletonChanged(null);
                item.Model = null;
            }
        }
    }
}
