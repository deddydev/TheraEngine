using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class Model : FileObject, IRenderable
    {
        private List<Mesh> _meshes = new List<Mesh>();
        private Skeleton _skeleton;

        public void Render()
        {
            if (_skeleton == null)
                return;

            foreach (Mesh m in _meshes)
                m.Render();
        }

        public override Task Load()
        {
            Task t = new Task(Skill.FbxSDK.IO);
            return base.Load();
        }

        public void SetSkeleton(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }
    }
}
