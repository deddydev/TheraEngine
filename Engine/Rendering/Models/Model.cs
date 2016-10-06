using CustomEngine.Worlds.Actors.Components;
using System;

namespace CustomEngine.Rendering.Models
{
    public class Model : ObjectBase, IRenderable
    {
        private Skeleton _skeleton;

        public void Render()
        {
            if (_skeleton == null)
                return;

            foreach (Mesh m in Children)
                m.Render();
        }

        public void SetSkeleton(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }
    }
}
