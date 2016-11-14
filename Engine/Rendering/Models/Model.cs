using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using System.Linq;
using CustomEngine.Files;
using System.Collections;

namespace CustomEngine.Rendering.Models
{
    public class Model : FileObject, IPrimitive, IEnumerable<Mesh>
    {
        private List<ModelComponent> _linkedComponents = new List<ModelComponent>();

        private List<Mesh> _meshes = new List<Mesh>();
        private Skeleton _skeleton;
        private bool _simulatePhysics;

        public List<Mesh> Meshes { get { return _meshes; } }

        public Skeleton Skeleton
        {
            get { return _skeleton; }
            set { _skeleton = value; }
        }
        public bool SimulatePhysics
        {
            get { return _simulatePhysics; }
            set { _simulatePhysics = value; }
        }
        public void LinkComponent(ModelComponent comp)
        {
            if (!_linkedComponents.Contains(comp))
                _linkedComponents.Add(comp);
        }
        public void UnlinkComponent(ModelComponent comp)
        {
            if (_linkedComponents.Contains(comp))
                _linkedComponents.Remove(comp);
        }
        public List<PrimitiveData> GetPrimitives()
        {
            return _meshes.Select(x => x._manager.Data).ToList();
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
