using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class Model : FileObject, IRenderable
    {
        private List<ModelComponent> _linkedComponents = new List<ModelComponent>();

        public List<Mesh> _meshes = new List<Mesh>();
        public Skeleton _skeleton;

        public void Render(float delta)
        {
            if (_skeleton == null)
                return;
            foreach (Mesh m in _meshes)
                m.Render(delta);
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
        public void SetSkeleton(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }
        public void AddMesh(Mesh m)
        {
            _meshes.Add(m);
        }
    }
}
