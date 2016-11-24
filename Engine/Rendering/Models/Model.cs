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
        private List<Mesh> _meshes = new List<Mesh>();
        private Skeleton _skeleton;
        private bool _simulatePhysics;
        private GenericPrimitiveComponent _linkedComponent;

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
        public override Matrix4 GetWorldMatrix() { return _linkedComponent != null ? _linkedComponent.WorldMatrix : Matrix4.Identity; }
        public override Matrix4 GetInverseWorldMatrix() { return _linkedComponent != null ? _linkedComponent.InverseWorldMatrix : Matrix4.Identity; }

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
