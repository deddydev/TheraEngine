using CustomEngine.Rendering.Models.Skeleton;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models.Meshes
{
    public class Influence : IEnumerable<BoneWeight>
    {
        private List<BoneWeight> _weights = new List<BoneWeight>();
        private Matrix4 _matrix;
        private Matrix4? _invMatrix;

        public Bone Bone { get { return _weights.Count == 1 ? _weights[0].Bone : null; } }
        public float Weight { get { return _weights.Count == 1 ? _weights[0].Weight : -1.0f; } }
        public bool IsSingleBound { get { return _weights.Count == 1; } }
        public bool IsWeighted { get { return _weights.Count > 1; } }

        public void AddWeight(BoneWeight weight)
        {
            _weights.Add(weight);
            Normalize();
        }
        public void CalcMatrix()
        {
            if (IsWeighted)
            {
                _matrix = InfluenceMatrix();
                _invMatrix = InverseInfluenceMatrix();
            }
            else if (Bone != null)
            {
                _matrix = Bone.FrameMatrix;
                _invMatrix = Bone.InverseFrameMatrix;
            }
            else
                _invMatrix = _matrix = Matrix4.Identity;
        }
        private Matrix4 InfluenceMatrix()
        {
            Matrix4 value = new Matrix4();
            foreach (BoneWeight w in _weights)
                if (w.Bone != null)
                    value += (w.Bone.FrameMatrix * w.Bone.InverseBindMatrix) * w.Weight;
            return value;
        }
        private Matrix4 InverseInfluenceMatrix()
        {
            Matrix4 value = new Matrix4();
            foreach (BoneWeight w in _weights)
                if (w.Bone != null)
                    value += (w.Bone.InverseFrameMatrix * w.Bone.BindMatrix) * w.Weight;
            return value;
        }
        public void Optimize(int maxWeights)
        {
            if (maxWeights == 0 || maxWeights >= _weights.Count)
                return;
            int[] toRemove = new int[_weights.Count - maxWeights];
            for (int i = 0; i < toRemove.Length; ++i)
                for (int j = 0; j < _weights.Count; ++j)
                    if (!toRemove.Contains(j + 1) &&
                        (toRemove[i] == 0 || _weights[j].Weight < _weights[toRemove[i] - 1].Weight))
                        toRemove[i] = j + 1;
            foreach (int k in toRemove)
                _weights.RemoveAt(k - 1);
            Normalize();
        }
        /// <summary>
        /// Makes sure all weights add up to 1.0f.
        /// Does not modify any locked weights.
        /// </summary>
        public void Normalize(int weightDecimalPlaces = 7)
        {
            float denom = 0.0f, num = 1.0f;
            foreach (BoneWeight b in _weights)
                if (b.Locked)
                    num -= b.Weight;
                else
                    denom += b.Weight;

            //Don't do anything if all weights are locked
            if (denom > 0.0f && num > 0.0f)
                foreach (BoneWeight b in _weights)
                    if (!b.Locked) //Only normalize unlocked weights used in the calculation
                        b.Weight = (float)Math.Round(b.Weight / denom * num, weightDecimalPlaces);
        }

        public IEnumerator<BoneWeight> GetEnumerator() { return ((IEnumerable<BoneWeight>)_weights).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<BoneWeight>)_weights).GetEnumerator(); }
    }
}
