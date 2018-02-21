using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public partial class PrimitiveData : TFileObject, IDisposable
    {
        public VertexTriangle GetFace(int index)
        {
            IndexTriangle t = _triangles[index];
            FacePoint fp0 = _facePoints[t.Point0];
            FacePoint fp1 = _facePoints[t.Point1];
            FacePoint fp2 = _facePoints[t.Point2];
            Vertex v0 = new Vertex(fp0, _buffers);
            Vertex v1 = new Vertex(fp1, _buffers);
            Vertex v2 = new Vertex(fp2, _buffers);
            return new VertexTriangle(v0, v1, v2);
        }
        public void GenerateBinormalTangentBuffers(int positionIndex, int normalIndex, int uvIndex)
        {
            VertexBuffer[] pBuffs = GetAllBuffersOfType(BufferType.Position);
            if (pBuffs.Length == 0)
            {
                Engine.PrintLine("No position buffers found.");
                return;
            }
            if (!pBuffs.IndexInRange(positionIndex))
            {
                Engine.PrintLine("Position index out of range of available position buffers.");
                return;
            }
            VertexBuffer[] nBuffs = GetAllBuffersOfType(BufferType.Normal);
            if (nBuffs.Length == 0)
            {
                Engine.PrintLine("No normal buffers found.");
                return;
            }
            if (!nBuffs.IndexInRange(normalIndex))
            {
                Engine.PrintLine("Normal index out of range of available normal buffers.");
                return;
            }
            VertexBuffer[] tBuffs = GetAllBuffersOfType(BufferType.TexCoord);
            if (tBuffs.Length == 0)
            {
                Engine.PrintLine("No texcoord buffers found.");
                return;
            }
            if (!tBuffs.IndexInRange(uvIndex))
            {
                Engine.PrintLine("UV index out of range of available texcoord buffers.");
                return;
            }

            Vec3 v1, v2, v3;//, n0, n1, n2;
            Vec2 w1, w2, w3;

            VertexBuffer pBuff = pBuffs[positionIndex];
            VertexBuffer nBuff = pBuffs[normalIndex];
            VertexBuffer tBuff = pBuffs[uvIndex];
            int pointCount = _triangles.Count * 3;
            List<Vec3> binormals = new List<Vec3>(pointCount);
            List<Vec3> tangents = new List<Vec3>(pointCount);
            
            for (int i = 0; i < _triangles.Count; ++i)
            {
                IndexTriangle t = _triangles[i];

                FacePoint fp0 = _facePoints[t.Point0];
                FacePoint fp1 = _facePoints[t.Point1];
                FacePoint fp2 = _facePoints[t.Point2];

                v1 = pBuff.Get<Vec3>(fp0.BufferIndices[pBuff.Index] * 12);
                v2 = pBuff.Get<Vec3>(fp1.BufferIndices[pBuff.Index] * 12);
                v3 = pBuff.Get<Vec3>(fp2.BufferIndices[pBuff.Index] * 12);

                //n0 = nBuff.Get<Vec3>(fp0.BufferIndices[nBuff.Index] * 12);
                //n1 = nBuff.Get<Vec3>(fp1.BufferIndices[nBuff.Index] * 12);
                //n2 = nBuff.Get<Vec3>(fp2.BufferIndices[nBuff.Index] * 12);

                w1 = tBuff.Get<Vec2>(fp0.BufferIndices[tBuff.Index] * 8);
                w2 = tBuff.Get<Vec2>(fp1.BufferIndices[tBuff.Index] * 8);
                w3 = tBuff.Get<Vec2>(fp2.BufferIndices[tBuff.Index] * 8);

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vec3 sdir = new Vec3(
                    (t2 * x1 - t1 * x2) * r, 
                    (t2 * y1 - t1 * y2) * r,
                    (t2 * z1 - t1 * z2) * r);
                Vec3 tdir = new Vec3(
                    (s1 * x2 - s2 * x1) * r, 
                    (s1 * y2 - s2 * y1) * r,
                    (s1 * z2 - s2 * z1) * r);
                
                //tan1[i1] += sdir;
                //tan1[i2] += sdir;
                //tan1[i3] += sdir;

                //tan2[i1] += tdir;
                //tan2[i2] += tdir;
                //tan2[i3] += tdir;

            //fp0.BufferIndices.Add(i);
            //    fp1.BufferIndices.Add(i);
            //    fp2.BufferIndices.Add(i);

            //    Vec3 deltaPos1 = v2 - v1;
            //    Vec3 deltaPos2 = v3 - v1;
                
            //    Vec2 deltaUV1 = w2 - w1;
            //    Vec2 deltaUV2 = w3 - w1;

            //    float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
            //    Vec3 tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
            //    Vec3 bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;

                //binormals.Add(bitangent);
                //tangents.Add(tangent);
            }

            //AddBuffer(binormals, new VertexAttribInfo(BufferType.Binormal));
            //AddBuffer(tangents, new VertexAttribInfo(BufferType.Tangent));
        }
        private void SetInfluences(params InfluenceDef[] influences)
        {
            Remapper remap = new Remapper();
            remap.Remap(influences);
            for (int i = 0; i < remap.RemapTable.Length; ++i)
                _facePoints[i]._influenceIndex = remap.RemapTable[i];
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
        public VertexBuffer this[BufferType type]
        {
            get => _buffers.FirstOrDefault(x => x.BufferType == type);
            set
            {
                //value.Name = _type.ToString();
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
        public VertexBuffer this[string name]
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
        public VertexBuffer this[int index]
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
        public VertexBuffer[] GetAllBuffersOfType(BufferType type)
            => _buffers.Where(x => x.BufferType == type).ToArray();
        public int[] GenerateBuffers(int vaoId)
        {
            List<int> bindingIds = new List<int>();
            foreach (VertexBuffer b in _buffers)
            {
                if (b == null)
                {
                    //bindingIds.Add(0);
                    continue;
                }
                b._vaoId = vaoId;
                bindingIds.Add(b.Generate());
            }
            return bindingIds.ToArray();
        }
        public VertexBuffer FindBuffer(string name)
        {
            if (_buffers != null)
                foreach (VertexBuffer b in _buffers)
                    if (b.Name == name)
                        return b;
            return null;
        }
        public void AddBufferNumeric<T>(
            IList<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : struct
        {
            if (_buffers == null)
                _buffers = new List<VertexBuffer>();

            int bufferIndex = _buffers.Count;
            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetDataNumeric(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetDataNumeric(bufferData);
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
            EBufferTarget target = EBufferTarget.DataArray) where T : struct
        {
            if (_buffers == null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetDataNumeric(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetDataNumeric(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
        }
        public VertexBuffer AddBuffer<T>(
            IList<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : IBufferable
        {
            if (_buffers == null)
                _buffers = new List<VertexBuffer>();

            int bufferIndex = _buffers.Count;
            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetData(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(i);
            }
            _buffers.Add(buffer);
            return buffer;
        }
        public VertexBuffer ReplaceBuffer<T>(
            IList<T> bufferData,
            int bufferIndex,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : IBufferable
        {
            if (_buffers == null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetData(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
            return buffer;
        }

        public Remapper GetBuffer<T>(int bufferIndex, out T[] array, bool remap = false) where T : IBufferable
        {
            if (_buffers == null || bufferIndex < 0 || bufferIndex >= _buffers.Count)
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
            switch (_type)
            {
                case EPrimitiveType.Triangles:
                    return _triangles?.SelectMany(x => new int[] { x.Point0, x.Point1, x.Point2 }).ToArray();
                case EPrimitiveType.Lines:
                    return _lines?.SelectMany(x => new int[] { x.Point0, x.Point1 }).ToArray();
                case EPrimitiveType.Points:
                    return _points?.Select(x => (int)x).ToArray();
            }
            return null;
        }
        private Remapper SetTriangleIndices(List<Vertex> vertices, bool remap = true)
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
        private Remapper SetLineIndices(List<Vertex> vertices, bool remap = true)
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
        private Remapper SetPointIndices(List<Vertex> vertices, bool remap = true)
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
    }
}
