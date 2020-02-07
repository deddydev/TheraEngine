using Extensions;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class BoneWeight
    {
        public static float ComparisonTolerance = 0.00001f;

        [TSerialize("Bone", NodeType = ENodeType.Attribute)]
        private string _bone;
        [TSerialize("Weight", NodeType = ENodeType.Attribute)]
        private float _weight;
        [TSerialize("Locked", NodeType = ENodeType.Attribute, Config = false)]
        private bool _locked;

        /// <summary>
        /// The name of the bone to use in the transformation. Names are case-sensitive.
        /// </summary>
        public string Bone
        {
            get => _bone;
            set => _bone = value;
        }

        /// <summary>
        /// How much this bone affects the overall transformation where 0.0f is no influence and 1.0f is total influence.
        /// </summary>
        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        /// <summary>
        /// If true, this weight's value cannot be modified by normalization of the parent influence's weights.
        /// </summary>
        public bool Locked
        {
            get => _locked;
            set => _locked = value;
        }

        public BoneWeight() : this(null, 1.0f) { }
        public BoneWeight(string bone) : this(bone, 1.0f) { }
        public BoneWeight(string bone, float weight) { Bone = bone; Weight = weight; }

        public void ClampWeight() => _weight = _weight.Clamp(0.0f, 1.0f);

        public override bool Equals(object obj)
            => Equals(obj as BoneWeight, ComparisonTolerance);
        public bool Equals(BoneWeight other, float weightTolerance)
            => other != null ? (other.Bone == Bone && Weight.EqualTo(other.Weight, weightTolerance) && Locked == other.Locked) : false;
        public static bool operator ==(BoneWeight left, BoneWeight right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(BoneWeight left, BoneWeight right)
        {
            if (left is null)
                return !(right is null);
            return !left.Equals(right);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
