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
        private bool _simulatePhysics = false, _collisionEnabled = true;

        public Skeleton Skeleton
        {
            get { return _skeleton; }
            set
            {
                if (value == _skeleton)
                    return;
                _skeleton = value;
                foreach (SkeletalRigidSubMesh m in RigidChildren)
                    m.SkeletonChanged(_skeleton);
                foreach (SkeletalSoftSubMesh m in SoftChildren)
                    m.SkeletonChanged(_skeleton);
            }
        }
        public bool SimulatePhysics
        {
            get { return _simulatePhysics; }
            set { _simulatePhysics = value; }
        }
        public bool CollisionEnabled
        {
            get { return _collisionEnabled; }
            set { _collisionEnabled = value; }
        }
        protected SkeletalMeshComponent _linkedComponent;
        protected MonitoredList<SkeletalRigidSubMesh> _rigidChildren = new MonitoredList<SkeletalRigidSubMesh>();
        protected MonitoredList<SkeletalSoftSubMesh> _softChildren = new MonitoredList<SkeletalSoftSubMesh>();
        
        public MonitoredList<SkeletalRigidSubMesh> RigidChildren { get { return _rigidChildren; } }
        public MonitoredList<SkeletalSoftSubMesh> SoftChildren { get { return _softChildren; } }

        public SkeletalMeshComponent LinkedComponent
        {
            get { return _linkedComponent; }
            set
            {
                if (_linkedComponent == value)
                    return;
                SkeletalMeshComponent oldComp = _linkedComponent;
                _linkedComponent = value;
                if (oldComp != null)
                    oldComp.Model = null;
                if (_linkedComponent != null)
                    _linkedComponent.Model = this;
            }
        }
        protected virtual void RigidChildAdded(SkeletalRigidSubMesh item)
        {
            item.Model = this;
            item.SkeletonChanged(_skeleton);
        }
        protected virtual void RigidChildRemoved(SkeletalRigidSubMesh item)
        {
            if (item.Model == this)
            {
                item.SkeletonChanged(null);
                item.Model = null;
            }
        }
        protected virtual void SoftChildAdded(SkeletalSoftSubMesh item)
        {
            item.Model = this;
            item.SkeletonChanged(_skeleton);
        }
        protected virtual void SoftChildRemoved(SkeletalSoftSubMesh item)
        {
            if (item.Model == this)
            {
                item.SkeletonChanged(null);
                item.Model = null;
            }
        }
        public virtual void OnSpawned()
        {
            _rigidChildren.ForEach(x => x.OnSpawned());
            _softChildren.ForEach(x => x.OnSpawned());
        }
        public virtual void OnDespawned()
        {
            _rigidChildren.ForEach(x => x.OnDespawned());
            _softChildren.ForEach(x => x.OnSpawned());
        }
    }
}
