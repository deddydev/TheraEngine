﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models
{
    public class CPUSkinInfo
    {
        /// <summary>
        /// Holds transformation information pertaining to a weighted group of up to 4 bones.
        /// </summary>
        public class LiveInfluence
        {
            public int _weightCount;
            public IBone[] _bones = new Bone[4];
            public float[] _weights = new float[4];
            public Matrix4 _positionMatrix;
            public Matrix4 _normalMatrix;
            internal bool _hasChanged;

            public static LiveInfluence FromInfluence(InfluenceDef inf, ISkeleton skel)
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
                _normalMatrix = new Matrix4();
                for (int i = 0; i < _weightCount; ++i)
                {
                    IBone b = _bones[i];
                    float w = _weights[i];
                    _positionMatrix += b.VertexMatrix * w;
                    _normalMatrix += b.VertexMatrix.Inverted().Transposed().GetRotationMatrix4() * w;
                }
            }
        }
        
        Vec3[]
            _basePositions,
            _baseNormals,
            _baseBinormals,
            _baseTangents;

        DataBuffer
            _positions,
            _normals,
            _binormals,
            _tangents;

        TMesh _data;
        internal LiveInfluence[] _influences;
        private int[] _influenceIndices;

        public CPUSkinInfo(TMesh data, ISkeleton skeleton)
        {
            (_positions = data[EBufferType.Position])?.GetData(out _basePositions, false);
            (_normals = data[EBufferType.Normal])?.GetData(out _baseNormals, false);
            (_binormals = data[EBufferType.Binormal])?.GetData(out _baseBinormals, false);
            (_tangents = data[EBufferType.Tangent])?.GetData(out _baseTangents, false);

            _influenceIndices = data.FacePoints.Select(x => x.InfluenceIndex).ToArray();
            _influences = data.Influences.Select(x => LiveInfluence.FromInfluence(x, skeleton)).ToArray();
            _data = data;
        }
        public unsafe void UpdatePNBT(IEnumerable<int> modifiedVertexIndices)
        {
            try
            {
                foreach (int i in modifiedVertexIndices)
                {
                    LiveInfluence inf = _influences[_influenceIndices[i]];
                    if (inf._hasChanged)
                    {
                        inf.CalcMatrix();
                        inf._hasChanged = false;
                    }
                    ((Vec3*)_positions.Address)[i] = _basePositions[i] * inf._positionMatrix;
                    if (_normals != null)
                        ((Vec3*)_normals.Address)[i] = _baseNormals[i] * inf._normalMatrix;
                    if (_binormals != null)
                        ((Vec3*)_binormals.Address)[i] = _baseBinormals[i] * inf._normalMatrix;
                    if (_tangents != null)
                        ((Vec3*)_tangents.Address)[i] = _baseTangents[i] * inf._normalMatrix;
                }
            }
            catch
            {
                //Engine.LogWarning("Modified vertex indices was modified while being evaluated; could not finish updating buffers.");
            }
        }
    }
}