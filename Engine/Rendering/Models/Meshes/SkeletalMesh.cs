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
    public class SkeletalMesh : FileObject, IEnumerable<SkeletalSubMesh>
    {
        public SkeletalMesh() : this(null) { }
        public SkeletalMesh(Skeleton skeleton) : base()
        {
            Skeleton = skeleton;
            _children.Removed += ChildRemoved;
            _children.Added += ChildAdded;
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
                //foreach (Mesh m in _children)
                //    m._manager.SkeletonChanged(_skeleton);
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
        protected MonitoredList<SkeletalSubMesh> _children = new MonitoredList<SkeletalSubMesh>();

        public MonitoredList<SkeletalSubMesh> Children { get { return _children; } }
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
        protected virtual void ChildAdded(SkeletalSubMesh item)
        {
            item.Model = this;
        }
        protected virtual void ChildRemoved(SkeletalSubMesh item)
        {
            if (item.Model == this)
                item.Model = null;
        }
        public virtual void OnSpawned()
        {
            _children.ForEach(x => x.OnSpawned());
        }
        public virtual void OnDespawned()
        {
            _children.ForEach(x => x.OnDespawned());
        }
        public List<SkeletalSubMesh> GetChildren(bool visibleByDefaultOnly)
        {
            if (visibleByDefaultOnly)
                return _children.Where(x => x.VisibleByDefault).ToList();
            else
                return _children;
        }
        public SkeletalSubMesh[] GetVisibleChildren()
        {
            return _children.Where(x => x.Visible).ToArray();
        }
        public SkeletalSubMesh[] GetHiddenChildren()
        {
            return _children.Where(x => !x.Visible).ToArray();
        }
        public IEnumerator<SkeletalSubMesh> GetEnumerator() { return ((IEnumerable<SkeletalSubMesh>)_children).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<SkeletalSubMesh>)_children).GetEnumerator(); }
    }
}
