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
        private Bone _bone;
        private float _weight;
        private bool _locked;
        
        public Bone Bone { get { return _bone; } set { _bone = value; } }
        public float Weight { get { return _weight; } set { _weight = value; } }
        public bool Locked { get { return _locked; } set { _locked = value; } }

        public BoneWeight() : this(null, 1.0f) { }
        public BoneWeight(Bone bone) : this(bone, 1.0f) { }
        public BoneWeight(Bone bone, float weight) { Bone = bone; Weight = weight; }

        public void ClampWeight() { _weight.Clamp(0.0f, 1.0f); }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoneWeight, 0.0001f);
        }
        public bool Equals(BoneWeight other, float weightTolerance)
        {
            return other != null ? (other.Bone == Bone && Weight.CompareEquality(other.Weight, weightTolerance) && Locked == other.Locked) : false;
        }
        public override int GetHashCode()
        {
            return _bone.GetHashCode() ^ _weight.GetHashCode() ^ _locked.GetHashCode();
        }
    }
}
