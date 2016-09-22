using CustomEngine.Rendering.Animation;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models.Skeleton
{
    public class Skeleton
    {
        public Dictionary<string, Bone> _boneCache;

        public Bone _rootBone;
    }
}
