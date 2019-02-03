using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Rendering.Models
{
    public partial class PrimitiveData
    {
        [CustomMemberSerializeMethod(nameof(Triangles))]
        private void SerializeTriangles(SerializeElement node)
        {
            node.SetElementContent(_triangles?.SelectMany(x => x.Points.Select(y => y.VertexIndex)).ToArray());
        }
        [CustomMemberSerializeMethod(nameof(Lines))]
        private void SerializeLines(SerializeElement node)
        {
            node.SetElementContent(_lines?.SelectMany(x => new[] { x.Point0.VertexIndex, x.Point1.VertexIndex }).ToArray());
        }
        [CustomMemberSerializeMethod(nameof(Points))]
        private void SerializePoints(SerializeElement node)
        {
            node.SetElementContent(_points?.Select(x => x.VertexIndex).ToArray());
        }
        [CustomMemberDeserializeMethod(nameof(Triangles))]
        private void DeserializeTriangles(SerializeElement node)
        {
            _triangles = node.GetElementContentAs(out int[] value) ? value.SelectEvery(3, x => new IndexTriangle(x[0], x[1], x[2])).ToList() : null;
        }
        [CustomMemberDeserializeMethod(nameof(Lines))]
        private void DeserializeLines(SerializeElement node)
        {
            _lines = node.GetElementContentAs(out int[] value) ? value.SelectEvery(2, x => new IndexLine(x[0], x[1])).ToList() : null;
        }
        [CustomMemberDeserializeMethod(nameof(Points))]
        private void DeserializePoints(SerializeElement node)
        {
            _points = node.GetElementContentAs(out int[] value) ? value.Select(x => new IndexPoint(x)).ToList() : null;
        }
        [CustomMemberSerializeMethod(nameof(FacePoints))]
        public void SerializeFacePoints(SerializeElement node)
        {
            node.AddAttribute("Count", _facePoints.Count);

            bool hasInfs = _influences != null && _influences.Length > 0;
            int indexCount = _buffers.Count + (hasInfs ? 1 : 0);
            int[] indices = new int[indexCount * _facePoints.Count];
            int x = 0;

            if (hasInfs)
            {
                foreach (FacePoint p in _facePoints)
                {
                    indices[x++] = p.InfluenceIndex;
                    foreach (int i in p.BufferIndices)
                        indices[x++] = i;
                }
            }
            else
            {
                foreach (FacePoint p in _facePoints)
                    foreach (int i in p.BufferIndices)
                        indices[x++] = i;
            }
            
            node.SetElementContent(indices);
        }
        [CustomMemberDeserializeMethod(nameof(FacePoints))]
        private void DeserializeFacePoints(SerializeElement node)
        {
            if (!node.GetAttributeValue("Count", out int count))
                count = 0;

            _facePoints = new List<FacePoint>(count);

            bool hasInfs = _influences != null && _influences.Length > 0;

            int bufferCount = _buffers.Count;
            if (node.GetElementContentAs(out int[] indices))
            {
                for (int i = 0, m = 0; i < count; ++i)
                {
                    FacePoint p = new FacePoint(i, this);

                    if (hasInfs)
                        p.InfluenceIndex = indices[m++];

                    for (int r = 0; r < bufferCount; ++r)
                        p.BufferIndices.Add(indices[m++]);

                    _facePoints.Add(p);
                }
            }
        }
        [CustomMemberSerializeMethod(nameof(Influences))]
        public void CustomInfluencesSerialize(SerializeElement node)
        {
            bool valid = string.IsNullOrEmpty(_singleBindBone) && _utilizedBones != null;
            node.AddAttribute("Count", valid ? _influences.Length : 0);

            if (!valid)
                return;

            SerializeElement countsNode = new SerializeElement("Counts");
            SerializeElement indicesNode = new SerializeElement("Indices");
            SerializeElement weightsNode = new SerializeElement("Weights");

            int[] counts = new int[_influences.Length];
            List<int> indices = new List<int>(_influences.Length * 4);
            List<float> weights = new List<float>(_influences.Length * 4);

            int x = 0;
            foreach (InfluenceDef inf in _influences)
            {
                counts[x++] = inf.WeightCount;
                for (int i = 0; i < inf.WeightCount; ++i)
                {
                    indices.Add(_utilizedBones.IndexOf(inf.Weights[i].Bone));
                    weights.Add(inf.Weights[i].Weight);
                }
            }

            countsNode.SetElementContent(counts);
            indicesNode.SetElementContent(indices.ToArray());
            weightsNode.SetElementContent(weights.ToArray());

            node.ChildElements.Add(countsNode);
            node.ChildElements.Add(indicesNode);
            node.ChildElements.Add(weightsNode);
        }
        [CustomMemberDeserializeMethod(nameof(Influences))]
        public void CustomInfluencesDeserialize(SerializeElement node)
        {
            if (!node.GetAttributeValue("Count", out int count))
                count = 0;

            _influences = new InfluenceDef[count];
            int[] boneCounts;
            int[] indices;
            float[] weights;

            SerializeElement countsNode = node.GetChildElement("Counts");
            SerializeElement indicesNode = node.GetChildElement("Indices");
            SerializeElement weightsNode = node.GetChildElement("Weights");

            if (countsNode != null)
                countsNode.GetElementContentAs(out boneCounts);
            else
                return;

            if (indicesNode != null)
                indicesNode.GetElementContentAs(out indices);
            else
                return;

            if (weightsNode != null)
                weightsNode.GetElementContentAs(out weights);
            else
                return;

            int k = 0;
            for (int i = 0; i < _influences.Length; ++i)
            {
                InfluenceDef inf = new InfluenceDef();
                for (int boneIndex = 0; boneIndex < boneCounts[i]; ++boneIndex, ++k)
                    inf.AddWeight(new BoneWeight(_utilizedBones[indices[k]], weights[k]));
                _influences[i] = inf;
            }
        }
    }
}
