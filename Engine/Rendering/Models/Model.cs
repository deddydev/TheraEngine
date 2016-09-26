using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models.Meshes;
using CustomEngine.Rendering.Models.Skeleton;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Meshes
{
    public class Model : ObjectBase, IRenderable
    {
        private List<Mesh> _meshes = new List<Mesh>();
        private Skeleton _skeleton;

        public void Render()
        {
            if (_skeleton == null)
                return;
        }

        public void SetSkeleton(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }
    }
}
