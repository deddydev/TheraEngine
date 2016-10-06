using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : ObjectBase
    {
        public Dictionary<string, Bone> _boneCache;

        public Bone _rootBone;
    }
}
