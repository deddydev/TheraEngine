using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public partial class PrimitiveData : FileObject, IDisposable
    {
        [CustomXMLSerializeMethod("Triangles")]
        private void SerializeTriangles(XmlWriter writer)
        {
            if (_triangles == null) return;
            string str = string.Join(" ", _triangles.SelectMany(x => x.Points.Select(y => y.VertexIndex.ToString())));
            writer.WriteElementString("Triangles", str);
        }
        [CustomXMLSerializeMethod("Lines")]
        private void SerializeLines(XmlWriter writer)
        {
            if (_lines == null) return;
            string str = string.Join(" ", _lines.SelectMany(x => new string[] { x.Point0.VertexIndex.ToString(), x.Point1.VertexIndex.ToString() }));
            writer.WriteElementString("Lines", str);
        }
        [CustomXMLSerializeMethod("Points")]
        private void SerializePoints(XmlWriter writer)
        {
            if (_points == null) return;
            string str = string.Join(" ", _points.Select(x => x.VertexIndex.ToString()));
            writer.WriteElementString("Points", str);
        }
        [CustomXMLDeserializeMethod("Triangles")]
        private void DeserializeTriangles(XMLReader reader)
        {
            string str = reader.ReadElementString();
            _triangles = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).SelectEvery(3, x => new IndexTriangle(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2]))).ToList();
        }
        [CustomXMLDeserializeMethod("Lines")]
        private void DeserializeLines(XMLReader reader)
        {
            string str = reader.ReadElementString();
            _lines = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).SelectEvery(2, x => new IndexLine(int.Parse(x[0]), int.Parse(x[1]))).ToList();
        }
        [CustomXMLDeserializeMethod("Points")]
        private void DeserializePoints(XMLReader reader)
        {
            string str = reader.ReadElementString();
            _points = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new IndexPoint(int.Parse(x))).ToList();
        }
        [CustomXMLSerializeMethod("FacePoints")]
        private bool SerializeFacePoints(XmlWriter writer)
        {
            writer.WriteStartElement("FacePoints");
            writer.WriteAttributeString("Count", _facePoints.Count.ToString());
            {
                bool hasInfs = _influences != null && _influences.Length > 0;
                foreach (FacePoint p in _facePoints)
                {
                    if (hasInfs)
                        writer.WriteString(p._influenceIndex.ToString() + " ");
                    foreach (int i in p.BufferIndices)
                        writer.WriteString(i.ToString() + " ");
                }
            }
            writer.WriteEndElement();
            return true;
        }
        [CustomXMLDeserializeMethod("FacePoints")]
        private bool DeserializeFacePoints(XMLReader reader)
        {
            int count = 0;
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("Count", true))
                    count = int.Parse(reader.Value);
            }
            bool hasInfs = _influences != null && _influences.Length > 0;
            int bufferCount = _buffers.Count;
            int valuesPerPoint = bufferCount + (hasInfs ? 1 : 0);
            string values = reader.ReadElementString();
            int[] points = values.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            _facePoints = new List<FacePoint>(points.Length / valuesPerPoint);
            for (int i = 0, x = 0; x < points.Length; ++i)
            {
                FacePoint p = new FacePoint(i, this);
                if (hasInfs)
                    p._influenceIndex = points[x++];
                for (int r = 0; r < bufferCount; ++r)
                    p.BufferIndices.Add(points[x++]);
                _facePoints.Add(p);
            }
            return true;
        }
        [CustomXMLSerializeMethod("Influences")]
        private bool CustomInfluencesSerialize(XmlWriter writer)
        {
            if (_influences != null)
            {
                writer.WriteStartElement("Influences");
                writer.WriteAttributeString("Count", _influences.Length.ToString());
                {
                    writer.WriteStartElement("Counts");
                    foreach (InfluenceDef inf in _influences)
                    {
                        writer.WriteString(inf.WeightCount.ToString() + " ");
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("Indices");
                    foreach (InfluenceDef inf in _influences)
                    {
                        for (int i = 0; i < inf.WeightCount; ++i)
                        {
                            writer.WriteString(_utilizedBones.IndexOf(inf.Weights[i].Bone).ToString() + " ");
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("Weights");
                    foreach (InfluenceDef inf in _influences)
                    {
                        for (int i = 0; i < inf.WeightCount; ++i)
                        {
                            writer.WriteString(inf.Weights[i].Weight.ToString() + " ");
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            return true;
        }
        [CustomXMLDeserializeMethod("Influences")]
        private bool CustomInfluencesDeserialize(XMLReader reader)
        {
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("Count", true))
                {
                    int count = int.Parse(reader.Value);
                    _influences = new InfluenceDef[count];
                }
            }
            int[] counts = null, indices = null;
            float[] weights = null;
            string s;
            while (reader.BeginElement())
            {
                s = reader.ReadElementString();
                if (reader.Name.Equals("Counts", true))
                    counts = s.Split(' ').Select(x => int.Parse(x)).ToArray();
                else if (reader.Name.Equals("Indices", true))
                    indices = s.Split(' ').Select(x => int.Parse(x)).ToArray();
                else if (reader.Name.Equals("Weights", true))
                    weights = s.Split(' ').Select(x => float.Parse(x)).ToArray();
                reader.EndElement();
            }
            int k = 0;
            for (int i = 0; i < _influences.Length; ++i)
            {
                InfluenceDef inf = new InfluenceDef();
                for (int j = 0; j < counts[i]; ++j, ++k)
                    inf.AddWeight(new BoneWeight(_utilizedBones[indices[k]], weights[k]));
                _influences[i] = inf;
            }
            return true;
        }
    }
}
