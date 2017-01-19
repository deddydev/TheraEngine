using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using System.Linq;
using CustomEngine.Files;
using System.Collections;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    public class SkeletalMesh : FileObject
    {
        public SkeletalMesh() : this(null) { }
        public SkeletalMesh(Skeleton skeleton) : base()
        {
            Skeleton = skeleton;
            _rigidChildren.Removed += RigidChildRemoved;
            _rigidChildren.Added += RigidChildAdded;
            _softChildren.Removed += SoftChildRemoved;
            _softChildren.Added += SoftChildAdded;
        }

        private Skeleton _skeleton;
        public Skeleton Skeleton
        {
            get { return _skeleton; }
            set
            {
                if (value == _skeleton)
                    return;
                _skeleton = value;
            }
        }

        public void SetAllSimulatingPhysics(bool doSimulation)
        {
            foreach (Bone b in Skeleton)
                if (b.PhysicsDriver != null)
                    b.PhysicsDriver.SimulatingPhysics = doSimulation;
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
    }
}
