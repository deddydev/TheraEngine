using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class Model : FileObject, IRenderable
    {
        private List<ModelComponent> _linkedComponents = new List<ModelComponent>();

        private List<Mesh> _meshes = new List<Mesh>();
        private Skeleton _skeleton;

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
        public override Task Load()
        {
            return base.Load();
        }
        public void SetSkeleton(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }
    }
}
