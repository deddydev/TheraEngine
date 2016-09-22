using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models.Meshes;
using CustomEngine.Rendering.Models.Skeleton;
using CustomEngine.System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Meshes
{
    public class Model : ObjectBase
    {
        private List<Mesh> _meshes;
        private Skeleton _skeleton;

        public void Render()
        {

        }

        public void ApplyAnimation(PropertyAnimation animation, float frame)
        {
            if (animation is BoneAnimation)
                _skeleton.ApplyAnimation((BoneAnimation)animation);
            else
            {

            }
        }

        public void SetSkeleton(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }
    }
}
