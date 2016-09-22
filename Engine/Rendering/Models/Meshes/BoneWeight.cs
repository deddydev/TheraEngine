using CustomEngine.Rendering.Models.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Meshes
{
    public class BoneWeight
    {
        Bone _bone;
        float _weight;
        bool _locked;
        
        public Bone Bone { get { return _bone; } set { _bone = value; } }
        public float Weight { get { return _weight; } set { _weight = value; } }
        public bool Locked { get { return _locked; } set { _locked = value; } }

        public BoneWeight() : this(null, 1.0f) { }
        public BoneWeight(Bone bone) : this(bone, 1.0f) { }
        public BoneWeight(Bone bone, float weight) { Bone = bone; Weight = weight; }

        public void ClampWeight() { _weight.Clamp(0.0f, 1.0f); }
    }
}
