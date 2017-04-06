using System;

namespace CustomEngine.Rendering.Models
{
    public class BoneWeight
    {
        public static float ComparisonTolerance = 0.00001f;

        private string _bone;
        private float _weight;
        private bool _locked;

        public string Bone { get { return _bone; } set { _bone = value; } }
        public float Weight { get { return _weight; } set { _weight = value; } }
        public bool Locked { get { return _locked; } set { _locked = value; } }

        public BoneWeight() : this(null, 1.0f) { }
        public BoneWeight(string bone) : this(bone, 1.0f) { }
        public BoneWeight(string bone, float weight) { Bone = bone; Weight = weight; }

        public void ClampWeight() { _weight.Clamp(0.0f, 1.0f); }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoneWeight, ComparisonTolerance);
        }
        public bool Equals(BoneWeight other, float weightTolerance)
        {
            return other != null ? (other.Bone == Bone && Weight.EqualTo(other.Weight, weightTolerance) && Locked == other.Locked) : false;
        }
        public static bool operator ==(BoneWeight left, BoneWeight right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }
        public static bool operator !=(BoneWeight left, BoneWeight right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);
            return !left.Equals(right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
