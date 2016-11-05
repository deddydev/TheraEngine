using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class Model : FileObject, IPrimitive
    {
        private List<ModelComponent> _linkedComponents = new List<ModelComponent>();

        private List<Mesh> _meshes = new List<Mesh>();
        private Skeleton _skeleton;
        private bool _simulatePhysics;

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
        public override void Load()
        {
            base.Load();
        }
        public void AddMesh(Mesh m)
        {
            _meshes.Add(m);
        }

        public List<PrimitiveData> GetPrimitives()
        {
            return _meshes.Select(x => x._manager.Data).ToList();
        }
    }
}
