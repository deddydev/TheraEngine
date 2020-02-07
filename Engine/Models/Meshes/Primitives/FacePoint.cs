using Extensions;
using System.Collections.Generic;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class FacePoint : TObject
    {
        public int Index { get; set; }

        [TSerialize]
        public List<int> BufferIndices { get; } = new List<int>();
        [TSerialize]
        internal int InfluenceIndex { get; set; }

        internal PrimitiveData Owner { get; }

        public FacePoint(int index, PrimitiveData data)
        {
            Index = index;
            Owner = data;
        }

        public InfluenceDef GetInfluence()
            =>Owner?.Influences != null && Owner.Influences.IndexInRange(InfluenceIndex) ? Owner.Influences[InfluenceIndex] : null;
        
        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString()
        {
            string fp = "FP" + Index;
            if (BufferIndices.Count > 0)
                fp += ": ";
            for (int i = 0; i < BufferIndices.Count; ++i)
                fp += "(" + i + ": " + BufferIndices[i] + ")";
            return fp;
        }
    }
}
