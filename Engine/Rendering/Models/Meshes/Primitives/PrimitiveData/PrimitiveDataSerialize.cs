using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Rendering.Models
{
    public partial class PrimitiveData : TFileObject, IDisposable
    {
        //[CustomSerializeMethod(nameof(Triangles))]
        //private void SerializeTriangles(MemberTreeNode node)
        //{
        //    node.SetElementContent(_triangles?.SelectMany(x => x.Points.Select(y => y.VertexIndex))?.ToArray());
        //}
        //[CustomSerializeMethod(nameof(Lines))]
        //private void SerializeLines(MemberTreeNode node)
        //{
        //    node.SetElementContent(_lines?.SelectMany(x => new int[] { x.Point0.VertexIndex, x.Point1.VertexIndex })?.ToArray());
        //}
        //[CustomSerializeMethod(nameof(Points))]
        //private void SerializePoints(MemberTreeNode node)
        //{
        //    node.SetElementContent(_points?.Select(x => x.VertexIndex)?.ToArray());
        //}
        //[CustomDeserializeMethod(nameof(Triangles))]
        //private void DeserializeTriangles(MemberTreeNode node)
        //{
        //    if (node.GetElementContentAs(out int[] value))
        //        _triangles = value.SelectEvery(3, x => new IndexTriangle(x[0], x[1], x[2])).ToList();
        //    else
        //        _triangles = null;
        //}
        //[CustomDeserializeMethod(nameof(Lines))]
        //private void DeserializeLines(MemberTreeNode node)
        //{
        //    if (node.GetElementContentAs(out int[] value))
        //        _lines = value.SelectEvery(3, x => new IndexTriangle(x[0], x[1], x[2])).ToList();
        //    else
        //        _lines = null;

        //    string str = reader.ReadElementString();
        //    _lines = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).SelectEvery(2, x => new IndexLine(int.Parse(x[0]), int.Parse(x[1]))).ToList();
        //}
        //[CustomDeserializeMethod(nameof(Points))]
        //private void DeserializePoints(XMLReader reader)
        //{
        //    string str = reader.ReadElementString();
        //    _points = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new IndexPoint(int.Parse(x))).ToList();
        //}
        [CustomSerializeMethod(nameof(FacePoints))]
        private void SerializeFacePoints(MemberTreeNode node)
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
        [CustomDeserializeMethod(nameof(FacePoints))]
        private void DeserializeFacePoints(MemberTreeNode node)
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
        [CustomSerializeMethod(nameof(Influences))]
        private void CustomInfluencesSerialize(MemberTreeNode node)
        {
            bool valid = string.IsNullOrEmpty(_singleBindBone) && _utilizedBones != null;
            node.AddAttribute("Count", valid ? _influences.Length : 0);

            if (!valid)
                return;

            MemberTreeNode countsNode = new MemberTreeNode();
            MemberTreeNode indicesNode = new MemberTreeNode();
            MemberTreeNode weightsNode = new MemberTreeNode();

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

            node.ChildElementMembers.Add(countsNode);
            node.ChildElementMembers.Add(indicesNode);
            node.ChildElementMembers.Add(weightsNode);
        }
        [CustomDeserializeMethod(nameof(Influences))]
        private bool CustomInfluencesDeserialize(MemberTreeNode node)
        {
            if (!node.GetAttributeValue("Count", out int count))
                count = 0;

            _influences = new InfluenceDef[count];

            MemberTreeNode countsNode = node.GetChildElement("Counts");
            MemberTreeNode indicesNode = node.GetChildElement("Indices");
            MemberTreeNode weightsNode = node.GetChildElement("Weights");

            countsNode.GetElementContentAs(out int[] boneCounts);
            indicesNode.GetElementContentAs(out int[] indices);
            weightsNode.GetElementContentAs(out float[] weights);
            
            int k = 0;
            for (int i = 0; i < _influences.Length; ++i)
            {
                InfluenceDef inf = new InfluenceDef();
                for (int boneIndex = 0; boneIndex < boneCounts[i]; ++boneIndex, ++k)
                    inf.AddWeight(new BoneWeight(_utilizedBones[indices[k]], weights[k]));
                _influences[i] = inf;
            }
            
            return true;
        }
    }
}
