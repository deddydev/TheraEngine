﻿using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models
{
    public partial class TMesh : TFileObject, IDisposable, IEnumerable<DataBuffer>
    {
        public TVertexTriangle GetFace(int index)
        {
            IndexTriangle t = _triangles[index];
            FacePoint fp0 = _facePoints[t.Point0];
            FacePoint fp1 = _facePoints[t.Point1];
            FacePoint fp2 = _facePoints[t.Point2];
            TVertex v0 = new TVertex(fp0, _buffers);
            TVertex v1 = new TVertex(fp1, _buffers);
            TVertex v2 = new TVertex(fp2, _buffers);
            return new TVertexTriangle(v0, v1, v2);
        }
        public void GenerateBinormalTangentBuffers(int positionIndex, int normalIndex, int uvIndex, bool addBinormals, bool addTangents)
        {
            DataBuffer[] pBuffs = GetAllBuffersOfType(EBufferType.Position);
            if (pBuffs.Length == 0)
            {
                Engine.LogWarning("No position buffers found.");
                return;
            }
            if (!pBuffs.IndexInRange(positionIndex))
            {
                Engine.LogWarning("Position index out of range of available position buffers.");
                return;
            }
            DataBuffer[] nBuffs = GetAllBuffersOfType(EBufferType.Normal);
            if (nBuffs.Length == 0)
            {
                Engine.LogWarning("No normal buffers found.");
                return;
            }
            if (!nBuffs.IndexInRange(normalIndex))
            {
                Engine.LogWarning("Normal index out of range of available normal buffers.");
                return;
            }
            DataBuffer[] tBuffs = GetAllBuffersOfType(EBufferType.TexCoord);
            if (tBuffs.Length == 0)
            {
                Engine.LogWarning("No texcoord buffers found.");
                return;
            }
            if (!tBuffs.IndexInRange(uvIndex))
            {
                Engine.LogWarning("UV index out of range of available texcoord buffers.");
                return;
            }

            Vec3 pos1, pos2, pos3;
            //Vec3 n0, n1, n2;
            Vec2 uv1, uv2, uv3;

            DataBuffer pBuff = pBuffs[positionIndex];
            //VertexBuffer nBuff = pBuffs[normalIndex];
            DataBuffer tBuff = pBuffs[uvIndex];
            int pointCount = _triangles.Count * 3;
            List<Vec3> binormals = new List<Vec3>(pointCount);
            List<Vec3> tangents = new List<Vec3>(pointCount);
            
            for (int i = 0; i < _triangles.Count; ++i)
            {
                IndexTriangle t = _triangles[i];

                FacePoint fp0 = _facePoints[t.Point0];
                FacePoint fp1 = _facePoints[t.Point1];
                FacePoint fp2 = _facePoints[t.Point2];

                pos1 = pBuff.Get<Vec3>(fp0.BufferIndices[pBuff.Index] * 12);
                pos2 = pBuff.Get<Vec3>(fp1.BufferIndices[pBuff.Index] * 12);
                pos3 = pBuff.Get<Vec3>(fp2.BufferIndices[pBuff.Index] * 12);
                
                uv1 = tBuff.Get<Vec2>(fp0.BufferIndices[tBuff.Index] * 8);
                uv2 = tBuff.Get<Vec2>(fp1.BufferIndices[tBuff.Index] * 8);
                uv3 = tBuff.Get<Vec2>(fp2.BufferIndices[tBuff.Index] * 8);
                
                Vec3 deltaPos1 = pos2 - pos1;
                Vec3 deltaPos2 = pos3 - pos1;

                Vec2 deltaUV1 = uv2 - uv1;
                Vec2 deltaUV2 = uv3 - uv1;

                Vec3 tangent;
                Vec3 binormal;

                float m = deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X;
                if (m == 0.0f)
                {
                    tangent = Vec3.Up;
                    binormal = Vec3.Up;
                }
                else
                {
                    float r = 1.0f / m;
                    tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                    binormal = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;
                }

                binormals.Add(binormal);
                binormals.Add(binormal);
                binormals.Add(binormal);

                tangents.Add(tangent);
                tangents.Add(tangent);
                tangents.Add(tangent);
            }

            AddBuffer(binormals, new VertexAttribInfo(EBufferType.Binormal));
            _bufferInfo.HasBinormals = true;

            AddBuffer(tangents, new VertexAttribInfo(EBufferType.Tangent));
            _bufferInfo.HasTangents = true;

            OnBufferInfoChanged();
        }
        private void SetInfluences(params InfluenceDef[] influences)
        {
            Remapper remap = new Remapper();
            remap.Remap(influences);
            for (int i = 0; i < remap.RemapTable.Length; ++i)
                _facePoints[i].InfluenceIndex = remap.RemapTable[i];
            _influences = new InfluenceDef[remap.ImplementationLength];
            for (int i = 0; i < remap.ImplementationLength; ++i)
                _influences[i] = influences[remap.ImplementationTable[i]];

            HashSet<string> utilized = new HashSet<string>();
            foreach (InfluenceDef inf in _influences)
                if (inf != null)
                    for (int i = 0; i < inf.WeightCount; ++i)
                        utilized.Add(inf.Weights[i].Bone);
            _utilizedBones = utilized.ToArray();
            if (_utilizedBones.Length == 1)
            {
                SingleBindBone = _utilizedBones[0];
                _utilizedBones = null;
            }
        }

        private void CreateFacePoints(int pointCount)
        {
            _facePoints = new List<FacePoint>();
            for (int i = 0; i < pointCount; ++i)
                _facePoints.Add(new FacePoint(i, this));
        }

        #region Buffers
        public DataBuffer this[EBufferType type, int index]
        {
            get
            {
                var matches = _buffers.Where(x => x.BufferType == type).ToArray();
                if (matches.IndexInRange(index))
                    return matches[index];
                throw new IndexOutOfRangeException();
            }
            set
            {
                value.BufferType = type;

                var matches = _buffers.Where(x => x.BufferType == type).ToArray();
                if (matches.IndexInRange(index))
                {
                    var buf = matches[index];
                    value.Index = buf.Index;
                    buf.Dispose();
                    _buffers[buf.Index] = value;
                }
                else
                    throw new IndexOutOfRangeException();
            }
        }
        public DataBuffer this[EBufferType type]
        {
            get => _buffers.FirstOrDefault(x => x.BufferType == type);
            set
            {
                value.BufferType = type;
                var buf = _buffers.FirstOrDefault(x => x.BufferType == type);
                if (buf != null)
                {
                    value.Index = buf.Index;
                    _buffers[buf.Index] = value;
                }
                else
                {
                    value.Index = _buffers.Count;
                    _buffers.Add(value);
                }
            }
        }
        public DataBuffer this[string name]
        {
            get => _buffers.FirstOrDefault(x => x.Name == name);
            set
            {
                var buf = _buffers.FirstOrDefault(x => x.Name == name);
                if (buf != null)
                {
                    value.Index = buf.Index;
                    _buffers[buf.Index] = value;
                }
                else
                {
                    value.Index = _buffers.Count;
                    _buffers.Add(value);
                }
            }
        }
        public DataBuffer this[int index]
        {
            get
            {
                if (index < 0 || index >= _buffers.Count)
                    throw new IndexOutOfRangeException();
                return _buffers[index];
            }
            set
            {
                if (index >= 0 && index < _buffers.Count)
                    _buffers[index] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }
        public DataBuffer[] GetAllBuffersOfType(EBufferType type)
            => _buffers.Where(x => x.BufferType == type).ToArray();
        public int[] GenerateBuffers(int vaoId)
        {
            List<int> bindingIds = new List<int>();
            foreach (DataBuffer b in _buffers)
            {
                if (b is null)
                {
                    //bindingIds.Add(0);
                    continue;
                }
                b._vaoId = vaoId;
                int id = b.Generate();
                bindingIds.Add(id);
            }
            return bindingIds.ToArray();
        }
        public DataBuffer FindBuffer(string name)
        {
            if (_buffers != null)
                foreach (DataBuffer b in _buffers)
                    if (b.Name == name)
                        return b;
            return null;
        }
        public void AddBufferNumeric<T>(
            IList<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            bool isMapped = false,
            int instanceDivisor = 0,
            EBufferTarget target = EBufferTarget.ArrayBuffer) where T : struct
        {
            if (_buffers is null)
                _buffers = new List<DataBuffer>();

            int bufferIndex = _buffers.Count;
            DataBuffer buffer = new DataBuffer(bufferIndex, info, target, integral)
            {
                InstanceDivisor = instanceDivisor,
                MapData = isMapped
            };
            if (remap)
            {
                Remapper remapper = buffer.SetDataNumeric(bufferData, true);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetDataNumeric(bufferData);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices.Add(i);
            }
            _buffers.Add(buffer);
        }
        public void ReplaceBufferNumeric<T>(
            IList<T> bufferData,
            int bufferIndex,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            bool isMapped = false,
            int instanceDivisor = 0,
            EBufferTarget target = EBufferTarget.ArrayBuffer) where T : struct
        {
            if (_buffers is null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            DataBuffer buffer = new DataBuffer(bufferIndex, info, target, integral)
            {
                InstanceDivisor = instanceDivisor,
                MapData = isMapped
            };
            if (remap)
            {
                Remapper remapper = buffer.SetDataNumeric(bufferData, true);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetDataNumeric(bufferData);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
        }
        public DataBuffer AddBuffer<T>(
            IList<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            bool isMapped = false,
            int instanceDivisor = 0,
            EBufferTarget target = EBufferTarget.ArrayBuffer) where T : unmanaged, IBufferable
        {
            if (_buffers is null)
                _buffers = new List<DataBuffer>();

            int bufferIndex = _buffers.Count;
            DataBuffer buffer = new DataBuffer(bufferIndex, info, target, integral)
            {
                InstanceDivisor = instanceDivisor,
                MapData = isMapped
            };
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetData(bufferData);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices.Add(i);
            }
            _buffers.Add(buffer);
            return buffer;
        }
        public DataBuffer ReplaceBuffer<T>(
            IList<T> bufferData,
            int bufferIndex,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            bool isMapped = false,
            int instanceDivisor = 0,
            EBufferTarget target = EBufferTarget.ArrayBuffer) where T : unmanaged, IBufferable
        {
            if (_buffers is null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            DataBuffer buffer = new DataBuffer(bufferIndex, info, target, integral)
            {
                InstanceDivisor = instanceDivisor,
                MapData = isMapped
            };
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetData(bufferData);
                if (instanceDivisor == 0)
                    for (int i = 0; i < bufferData.Count; ++i)
                        _facePoints[i].BufferIndices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
            return buffer;
        }

        public Remapper GetBuffer<T>(int bufferIndex, out T[] array, bool remap = false) where T : unmanaged, IBufferable
        {
            if (_buffers is null || bufferIndex < 0 || bufferIndex >= _buffers.Count)
            {
                array = null;
                return null;
            }
            return _buffers[bufferIndex].GetData(out array, remap);
        }
        #endregion

        #region Indices
        public int[] GetIndices()
        {
            int[] indices = _type switch
            {
                EPrimitiveType.Triangles => _triangles?.SelectMany(x => new int[] { x.Point0, x.Point1, x.Point2 }).ToArray(),
                EPrimitiveType.Lines => _lines?.SelectMany(x => new int[] { x.Point0, x.Point1 }).ToArray(),
                EPrimitiveType.Points => _points?.Select(x => (int)x).ToArray(),
                _ => null,
            };
            if (indices is null)
                throw new InvalidOperationException($"{_type} mesh has no face indices.");
            return indices;
        }
        private Remapper SetTriangleIndices(List<TVertex> vertices, bool remap = true)
        {
            if (vertices.Count % 3 != 0)
                throw new Exception("Vertex list needs to be a multiple of 3.");

            _triangles = new List<IndexTriangle>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _triangles.Add(new IndexTriangle(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _triangles.Add(new IndexTriangle(i++, i++, i++));
                return null;
            }
        }
        private Remapper SetLineIndices(List<TVertex> vertices, bool remap = true)
        {
            if (vertices.Count % 2 != 0)
                throw new Exception("Vertex list needs to be a multiple of 2.");

            _lines = new List<IndexLine>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _lines.Add(new IndexLine(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _lines.Add(new IndexLine(i++, i++));
                return null;
            }
        }
        private Remapper SetPointIndices(List<TVertex> vertices, bool remap = true)
        {
            _points = new List<IndexPoint>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                    _points.Add(new IndexPoint(remapper.RemapTable[i++]));
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _points.Add(new IndexPoint(i++));
                return null;
            }
        }
        #endregion

        public void Dispose()
        {
            _buffers?.ForEach(x => x.Dispose());
        }

        public IEnumerator<DataBuffer> GetEnumerator() => ((IEnumerable<DataBuffer>)_buffers).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<DataBuffer>)_buffers).GetEnumerator();
    }
}
