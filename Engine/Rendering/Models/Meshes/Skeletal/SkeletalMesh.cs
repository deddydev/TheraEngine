using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using System.Linq;
using CustomEngine.Files;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalMesh : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.SkeletalMesh; } }
        
        public SkeletalMesh() : base()
        {
            _rigidChildren.Removed += RigidChildRemoved;
            _rigidChildren.Added += RigidChildAdded;
            _softChildren.Removed += SoftChildRemoved;
            _softChildren.Added += SoftChildAdded;
        }

        protected MonitoredList<SkeletalRigidSubMesh> _rigidChildren = new MonitoredList<SkeletalRigidSubMesh>();
        protected MonitoredList<SkeletalSoftSubMesh> _softChildren = new MonitoredList<SkeletalSoftSubMesh>();
        
        public MonitoredList<SkeletalRigidSubMesh> RigidChildren { get { return _rigidChildren; } }
        public MonitoredList<SkeletalSoftSubMesh> SoftChildren { get { return _softChildren; } }
        
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

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
