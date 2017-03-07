using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class CPUSkinInfo
    {
        public class LiveInfluence
        {
            public int _weightCount;
            public Bone[] _bones = new Bone[4];
            public float[] _weights = new float[4];
            public Matrix4 _positionMatrix;
            public Matrix3 _normalMatrix;
            internal bool _hasChanged;

            public static LiveInfluence FromInfluence(Influence inf, Skeleton skel)
            {
                LiveInfluence f = new LiveInfluence()
                {
                    _weightCount = inf.WeightCount
                };
                for (int i = 0; i < inf.WeightCount; ++i)
                {
                    BoneWeight w = inf.Weights[i];
                    f._weights[i] = w.Weight;
                    f._bones[i] = skel[w.Bone];
                }
                return f;
            }

            public void CalcMatrix()
            {
                _positionMatrix = new Matrix4();
                _normalMatrix = new Matrix3();
                for (int i = 0; i < _weightCount; ++i)
                {
                    Bone b = _bones[i];
                    float w = _weights[i];
                    _positionMatrix += b.VertexMatrix * w;
                    _normalMatrix += b.VertexMatrixIT.GetRotationMatrix3() * w;
                }
            }
        }
        
        Vec3[]
            _basePositions,
            _baseNormals,
            _baseBinormals,
            _baseTangents;

        VertexBuffer
            _positions,
            _normals,
            _binormals,
            _tangents;

        PrimitiveData _data;
        internal LiveInfluence[] _influences;
        private int[] _influenceIndices;

        public CPUSkinInfo(PrimitiveData data, Skeleton skeleton)
        {
            (_positions = data[BufferType.Position])?.GetData(out _basePositions, false);
            (_normals = data[BufferType.Normal])?.GetData(out _baseNormals, false);
            (_binormals = data[BufferType.Binormal])?.GetData(out _baseBinormals, false);
            (_tangents = data[BufferType.Tangent])?.GetData(out _baseTangents, false);

            _influenceIndices = data._facePoints.Select(x => x._influenceIndex).ToArray();
            _influences = data._influences.Select(x => LiveInfluence.FromInfluence(x, skeleton)).ToArray();
            _data = data;
        }
        
        public unsafe void UpdatePNBT(IEnumerable<int> modifiedVertexIndices)
        {
            foreach (int i in modifiedVertexIndices)
            {
                LiveInfluence inf = _influences[_influenceIndices[i]];
                if (inf._hasChanged)
                {
                    inf.CalcMatrix();
                    inf._hasChanged = false;
                }
                ((Vec3*)_positions.Data)[i] = _basePositions[i] * inf._positionMatrix;
                if (_normals != null)
                    ((Vec3*)_normals.Data)[i] = inf._normalMatrix * _baseNormals[i];
                if (_binormals != null)
                    ((Vec3*)_binormals.Data)[i] = _baseBinormals[i] * inf._normalMatrix;
                if (_tangents != null)
                    ((Vec3*)_tangents.Data)[i] = _baseTangents[i] * inf._normalMatrix;
            }
        }
    }
}