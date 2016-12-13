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
    public class Model : RenderableObjectContainer<Mesh>
    {
        public Model() : base() { }
        public Model(Skeleton skeleton)
        {
            Skeleton = skeleton;
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
        protected override void ChildAdded(Mesh item)
        {
            item.Model = this;
        }
        protected override void ChildRemoved(Mesh item)
        {
            item.Model = null;
        }
    }
}
