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
    public class Model : RenderableObjectContainer, IEnumerable<Mesh>
    {
        public Model()
        {
            _meshes.Added += _meshes_Added;
            _meshes.Removed += _meshes_Removed;
        }
        public Model(Skeleton skeleton)
        {
            _meshes.Added += _meshes_Added;
            _meshes.Removed += _meshes_Removed;
            Skeleton = skeleton;
        }
        private void _meshes_Removed(Mesh item) { item.Model = null; }
        private void _meshes_Added(Mesh item) { item.Model = this; }

        private MonitoredList<Mesh> _meshes = new MonitoredList<Mesh>();
        private Skeleton _skeleton;
        private bool _simulatePhysics = false, _collisionEnabled = true;

        public MonitoredList<Mesh> Meshes { get { return _meshes; } }
        public Skeleton Skeleton
        {
            get { return _skeleton; }
            set
            {
                if (value == _skeleton)
                    return;
                _skeleton = value;
                foreach (Mesh m in _meshes)
                    m._manager.SkeletonChanged(_skeleton);
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
        public override Matrix4 GetWorldMatrix() { return LinkedComponent != null ? LinkedComponent.WorldMatrix : Matrix4.Identity; }
        public override Matrix4 GetInverseWorldMatrix() { return LinkedComponent != null ? LinkedComponent.InverseWorldMatrix : Matrix4.Identity; }

        public override List<PrimitiveData> GetPrimitives()
        {
            return _meshes.Select(x => x.GetPrimitiveData()).ToList();
        }
        public void OnSpawned()
        {
            _meshes.ForEach(x => x.OnSpawned());
        }
        public void OnDespawned()
        {
            _meshes.ForEach(x => x.OnDespawned());
        }

        public IEnumerator<Mesh> GetEnumerator() { return ((IEnumerable<Mesh>)_meshes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Mesh>)_meshes).GetEnumerator(); }
    }
}
