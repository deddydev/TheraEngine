using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class Influence
    {
        public const int MaxWeightCount = 4;

        public Influence(string bone)
        {
            _weights[0] = new BoneWeight(bone);
            ++_weightCount;
        }
        public Influence(params BoneWeight[] weights) { SetWeights(weights); }

        public int WeightCount { get { return _weightCount; } }
        public BoneWeight[] Weights { get { return _weights; } }

        private int _weightCount = 0;
        private BoneWeight[] _weights = new BoneWeight[MaxWeightCount];

        public void AddWeight(BoneWeight weight)
        {
            if (_weightCount == MaxWeightCount)
            {
                List<BoneWeight> weights = _weights.ToList();
                weights.Add(weight);
                _weights = Optimize(weights, out _weightCount);
                return;
            }
            _weights[_weightCount++] = weight;
        }
        public void SetWeights(params BoneWeight[] weights)
        {
           SetWeights(weights.ToList());
        }
        public void SetWeights(List<BoneWeight> weights)
        {
            _weights = Optimize(weights, out _weightCount);
        }

        //private Matrix4 _matrix;
        //private Matrix4? _invMatrix;

        //public void CalcMatrix()
        //{
        //    if (IsWeighted)
        //    {
        //        _matrix = InfluenceMatrix();
        //        _invMatrix = InverseInfluenceMatrix();
        //    }
        //    else if (Bone != null)
        //    {
        //        _matrix = Bone.FrameMatrix;
        //        _invMatrix = Bone.InverseFrameMatrix;
        //    }
        //    else
        //        _invMatrix = _matrix = Matrix4.Identity;
        //}
        //private Matrix4 InfluenceMatrix()
        //{
        //    Matrix4 value = new Matrix4();
        //    foreach (BoneWeight w in _weights)
        //        if (w != null)
        //            value += (w.Bone.FrameMatrix * w.Bone.InverseBindMatrix) * w.Weight;
        //    return value;
        //}
        //private Matrix4 InverseInfluenceMatrix()
        //{
        //    Matrix4 value = new Matrix4();
        //    foreach (BoneWeight w in _weights)
        //        if (w.Bone != null)
        //            value += (w.Bone.InverseFrameMatrix * w.Bone.BindMatrix) * w.Weight;
        //    return value;
        //}

        public static BoneWeight[] Optimize(List<BoneWeight> weights, out int weightCount)
        {
            BoneWeight[] optimized = new BoneWeight[MaxWeightCount];
            if (weights.Count > 4)
            {
                int[] toRemove = new int[weights.Count - MaxWeightCount];
                for (int i = 0; i < toRemove.Length; ++i)
                    for (int j = 0; j < weights.Count; ++j)
                        if (!toRemove.Contains(j + 1) &&
                            (toRemove[i] == 0 || weights[j].Weight < weights[toRemove[i] - 1].Weight))
                            toRemove[i] = j + 1;
                foreach (int k in toRemove)
                    weights.RemoveAt(k - 1);
            }
            weightCount = weights.Count;
            for (int i = 0; i < weights.Count; ++i)
                optimized[i] = weights[i];
            Normalize(optimized);
            return optimized;
        }
        /// <summary>
        /// Makes sure all weights add up to 1.0f.
        /// Does not modify any locked weights.
        /// </summary>
        public static void Normalize(IEnumerable<BoneWeight> weights, int weightDecimalPlaces = 7)
        {
            float denom = 0.0f, num = 1.0f;
            foreach (BoneWeight b in weights)
                if (b != null)
                {
                    if (b.Locked)
                        num -= b.Weight;
                    else
                        denom += b.Weight;
                }

            //Don't do anything if all weights are locked
            if (denom > 0.0f && num > 0.0f)
                foreach (BoneWeight b in weights)
                    if (b != null && !b.Locked) //Only normalize unlocked weights used in the calculation
                        b.Weight = (float)Math.Round(b.Weight / denom * num, weightDecimalPlaces);
        }

        //public static bool operator ==(Influence left, Influence right)
        //{
        //    return left.Equals(right);
        //}
        //public static bool operator !=(Influence left, Influence right)
        //{
        //    return !left.Equals(right);
        //}
        public override bool Equals(object obj)
        {
            Influence other = obj as Influence;
            if (other == null || WeightCount == other.WeightCount)
                return false;
            
            for (int i = 0; i < WeightCount; ++i)
                if (_weights[i] != other._weights[i])
                    return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
